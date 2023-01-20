using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using Zavala.Events;
using Zavala.Resources;
using Zavala.Roads;

namespace Zavala.Functionalities
{
    // requests are usually created on a cycle
    // once the request is created, individual requests have individual timers
    [RequireComponent(typeof(ConnectionNode))]
    public class Requests : MonoBehaviour
    {
        [Serializable]
        public struct RequestBundle
        {
            public Resources.Type Type;
            public int Units;
            public bool Continuous;

            public RequestBundle(Resources.Type type, int units, bool continuous = false) {
                Type = type;
                Units = units;
                Continuous = continuous;
            }
        }

        public List<RequestBundle> RequestBundles;

        [SerializeField] private bool m_hasTimeout;
        [SerializeField] private int m_requestTimeout; // num Cycles

        [SerializeField] private float m_iconOffsetZ = 0.25f;

        public event EventHandler<ResourceEventArgs> RequestFulfilled;
        public event EventHandler<ResourceEventArgs> RequestExpired;

        private List<UIRequest> m_activeRequests;

        private Vector3 m_initialQueuePos;

        private ConnectionNode m_connectionNodeComponent;

        private void OnEnable() {
            m_activeRequests = new List<UIRequest>();

            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            EventMgr.Instance.EconomyUpdated += HandleEconomyUpdated;

            m_initialQueuePos = Vector3.zero;
        }

        private void OnDisable() {
            EventMgr.Instance.EconomyUpdated -= HandleEconomyUpdated;
        }

        private void Start() {
            m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;

            RegionMgr.Instance.GetRegionByPos(this.transform.position).RegisterWithClearingHouse(this);
        }

        public void QueueRequest() {
            Debug.Log("[Requests] Queueing new reqeust");

            if (m_initialQueuePos == Vector3.zero) {
                m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;
            }

            for (int i = 0; i < RequestBundles.Count; i++) {
                Resources.Type resourceType = RequestBundles[i].Type;
                int units = RequestBundles[i].Units;

                // init and display
                Debug.Log("[Instantiate] Instantiating UIRequest prefab");
                UIRequest newRequest = Instantiate(GameDB.Instance.UIRequestPrefab, this.transform).GetComponent<UIRequest>();
                if (m_hasTimeout) {
                    newRequest.Init(resourceType, m_requestTimeout, this.GetComponent<Cycles>(), units);
                }
                else {
                    newRequest.Init(resourceType, units);
                }

                // add to requests
                m_activeRequests.Add(newRequest);
                newRequest.TimerExpired += HandleTimerExpired;
                RedistributeQueue();

                m_connectionNodeComponent.UpdateNodeEconomy();
            }
        }

        public void CancelLastRequest(List<RequestBundle> resourceBundles) {
            if (m_activeRequests.Count == 0) { return; }

            List<Resources.Type> resourceTypes = new List<Resources.Type>();
            for (int i = 0; i < resourceBundles.Count; i++) {
                if (!resourceTypes.Contains(resourceBundles[i].Type)) {
                    resourceTypes.Add(resourceBundles[i].Type);
                }
            }

            for (int i = 0; i < resourceTypes.Count; i++) {
                CloseRequest(m_activeRequests.Count - 1, resourceTypes[i], false);
            }
        }

        private void RedistributeQueue() {
            for (int i = 0; i < m_activeRequests.Count; i++) {
                // order requests with newer on the left and older on the right
                UIRequest request = m_activeRequests[i];
                request.transform.localPosition = new Vector3(
                    m_initialQueuePos.x,
                    m_initialQueuePos.y,
                    m_initialQueuePos.z - (m_activeRequests.Count - i) * m_iconOffsetZ
                    );
            }
        }

        private void QueryRoadForProducts() {
            for (int requestIndex = 0; requestIndex < m_activeRequests.Count; requestIndex++) {
                if (m_activeRequests[requestIndex].IsEnRoute()) {
                    continue;
                }

                Resources.Type resourceType = m_activeRequests[requestIndex].GetResourceType();
                int desiredUnits = m_activeRequests[requestIndex].GetUnits();

                List<RoadSegment> connectedRoads = m_connectionNodeComponent.GetConnectedRoads();
                Debug.Log("[Requests] Querying road... (" + connectedRoads.Count + " roads connected)");

                // find first available
                for (int roadIndex = 0; roadIndex < connectedRoads.Count; roadIndex++) {
                    // TODO: get paths from each and pick shortest (see ClearingHouse implementation)

                    // query the road
                    List<RoadSegment> path;
                    StoresProduct supplier;
                    Resources.Type foundResourceType;
                    int foundUnits;
                    if (RoadMgr.Instance.QueryRoadForResource(this.gameObject, connectedRoads[roadIndex], resourceType, desiredUnits, out path, out supplier, out foundResourceType, out foundUnits)) {
                        // found resource -- try summon truck

                        Debug.Log("[Requests] Query was successful. length of path: " + path.Count + ". Units found: " + foundUnits);
                        if (RoadMgr.Instance.TrySummonTruck(foundResourceType, foundUnits, path, supplier, this)) {
                            Debug.Log("[Requests] Truck summoned successfully");

                            // set request to en-route
                            m_activeRequests[requestIndex].SetEnRoute();
                        }
                        else {
                            Debug.Log("[Requests] Truck not summoned");
                        }
                    }
                }
            }
        }

        public void ReceiveRequestedProduct(Resources.Type resourceType, int units) {
            for (int i = 0; i < m_activeRequests.Count; i++) {
                if (m_activeRequests[i].GetResourceType() == resourceType) {
                    // check if right number of units
                    if (units >= m_activeRequests[i].GetUnits()) {
                        // fully fulfilled
                        CloseRequest(i, resourceType, true);
                        return;
                    }
                    else {
                        // partially fulfilled
                        PartialCompleteRequest(i, resourceType, units);
                        return;
                    }
                }
                // handle SoilEnricher case (Manure OR Fertilizer)
                else if (m_activeRequests[i].GetResourceType() == Resources.Type.SoilEnricher) {
                    if (resourceType == Resources.Type.Manure || resourceType == Resources.Type.Fertilizer) {
                        // check if right number of units
                        if (units >= m_activeRequests[i].GetUnits()) {
                            // fully fulfilled
                            CloseRequest(i, resourceType, true);
                            return;
                        }
                        else {
                            // partially fulfilled
                            PartialCompleteRequest(i, resourceType, units);
                            return;
                        }
                    }
                }
            }
        }

        private void CloseRequest(int requestIndex, Resources.Type resourceType, bool fulfilled) {
            UIRequest toClose = m_activeRequests[requestIndex];
            toClose.TimerExpired -= HandleTimerExpired;
            m_activeRequests.RemoveAt(requestIndex);
            Destroy(toClose.gameObject);
            if (fulfilled) {
                // trigger request fulfilled event
                RequestFulfilled?.Invoke(this, new ResourceEventArgs(resourceType, toClose.GetInitialUnits()));
                Debug.Log("[Requests] Request for " + resourceType + " fulfilled!");
            }
        }

        private void PartialCompleteRequest(int requestIndex, Resources.Type resourceType, int units) {
            UIRequest toModify = m_activeRequests[requestIndex];
            toModify.ModifyUnits(-units);

            // trigger request fulfilled event
            Debug.Log("[Requests] Request for " + resourceType + " partially fulfilled!");
        }

        public int GetNumActiveRequests() {
            return m_activeRequests.Count;
        }

        public int GetNumActiveRequests(Resources.Type queryType) {
            int count = 0;

            for (int i = 0; i < m_activeRequests.Count; i++) {
                if (m_activeRequests[i].GetResourceType() == queryType) {
                    count += m_activeRequests[i].GetUnits();
                }
            }

            return count;
        }

        public bool RequestBundlesContains(Resources.Type resourceType) {
            for (int i = 0; i < RequestBundles.Count; i++) {
                if (RequestBundles[i].Type == resourceType) {
                    return true;
                }
            }

            return false;
        }

        public int RequestSingleBundleUnits(Resources.Type resourceType) {
            for (int i = 0; i < RequestBundles.Count; i++) {
                if (RequestBundles[i].Type == resourceType && RequestBundles[i].Units > 0) {
                    return RequestBundles[i].Units;
                }
            }

            return 0;
        }

        #region Handlers

        private void HandleTimerExpired(object sender, EventArgs e) {
            UIRequest expiredRequest = (UIRequest)sender;
            Debug.Log("[Requests] request expired");
            RequestExpired?.Invoke(this, new ResourceEventArgs(expiredRequest.GetResourceType(), expiredRequest.GetUnits()));

            m_activeRequests.Remove(expiredRequest);
            Destroy(expiredRequest.gameObject);
            RedistributeQueue();
        }

        private void HandleEconomyUpdated(object sender, EventArgs args) {
            // QueryRoadForProducts();
        }

        #endregion // Handlers

        #region AllVars Gets & Sets

        public void SetRequestTimeout(int newVal) {
            m_requestTimeout = newVal;
        }

        public int GetRequestTimeout() {
            return m_requestTimeout;
        }

        #endregion // AllVars Gets & Sets
    }
}
