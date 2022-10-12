﻿using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using Zavala.Resources;

namespace Zavala.Functionalities
{ 
    // requests are usually created on a cycle
    // once the request is created, individual requests have individual timers
    [RequireComponent(typeof(ConnectionNode))]
    public class Requests : MonoBehaviour
    {
        public List<Resources.Type> RequestTypes;

        [SerializeField] private bool m_hasTimeout;
        [SerializeField] private int m_requestTimeout; // num Cycles

        [SerializeField] private float m_iconOffsetZ = 0.25f;

        public event EventHandler<ResourceEventArgs> RequestFulfilled;
        public event EventHandler<ResourceEventArgs> RequestExpired;

        private List<UIRequest> m_activeRequests;

        private Vector3 m_initialQueuePos;

        private ConnectionNode m_connectionNodeComponent;

        private void Awake() {
            m_activeRequests = new List<UIRequest>();

            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_connectionNodeComponent.NodeEconomyUpdated += HandleNodeEconomyUpdated;

            m_initialQueuePos = Vector3.zero;
        }

        private void Start() {
            m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;
        }

        public void QueueRequest() {
            if (m_initialQueuePos == Vector3.zero) {
                m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;
            }

            for (int i = 0; i < RequestTypes.Count; i++) {
                Resources.Type resourceType = RequestTypes[i];

                // init and display
                Debug.Log("[Instantiate] Instantiating UIRequest prefab");
                UIRequest newRequest = Instantiate(GameDB.Instance.UIRequestPrefab, this.transform).GetComponent<UIRequest>();
                if (m_hasTimeout) {
                    newRequest.Init(resourceType, m_requestTimeout, this.GetComponent<Cycles>());
                }
                else {
                    newRequest.Init(resourceType);
                }

                // add to requests
                m_activeRequests.Add(newRequest);
                newRequest.TimerExpired += HandleTimerExpired;
                RedistributeQueue();

                m_connectionNodeComponent.UpdateNodeEconomy();
            }
        }

        public void CancelLastRequest(List<Resources.Type> resourceTypes) {
            if (m_activeRequests.Count == 0) { return; }

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

                List<Road> connectedRoads = m_connectionNodeComponent.GetConnectedRoads();
                Debug.Log("[Requests] Querying road... (" + connectedRoads.Count + " roads connected)");

                // find first available
                for (int roadIndex = 0; roadIndex < connectedRoads.Count; roadIndex++) {
                    if (connectedRoads[roadIndex].ResourceOnRoad(resourceType, this.gameObject)) {
                        // summon a truck from road fleet with this as recipient
                        // remove from supplier
                        Resources.Type foundResourceType; // the specific type provided by this supplier
                        StoresProduct supplier = connectedRoads[roadIndex].GetSupplierOnRoad(resourceType, out foundResourceType);

                        if (connectedRoads[roadIndex].TrySummonTruck(foundResourceType, supplier, this)) {
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

        public void ReceiveRequestedProduct(Resources.Type resourceType) {
            for (int i = 0; i < m_activeRequests.Count; i++) {
                if (m_activeRequests[i].GetResourceType() == resourceType) {
                    CloseRequest(i, resourceType, true);
                    return;
                }
                // handle SoilEnricher case (Manure OR Fertilizer)
                else if (m_activeRequests[i].GetResourceType() == Resources.Type.SoilEnricher) {
                    if (resourceType == Resources.Type.Manure || resourceType == Resources.Type.Fertilizer) {
                        CloseRequest(i, resourceType, true);
                        return;
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
                RequestFulfilled?.Invoke(this, new ResourceEventArgs(resourceType));
                Debug.Log("[Requests] Request for " + resourceType + " fulfilled!");
            }
        }

        public int GetNumActiveRequests() {
            return m_activeRequests.Count;
        }

        #region Handlers

        private void HandleTimerExpired(object sender, EventArgs e) {
            UIRequest expiredRequest = (UIRequest)sender;
            Debug.Log("[Requests] request expired");
            RequestExpired?.Invoke(this, new ResourceEventArgs(expiredRequest.GetResourceType()));

            m_activeRequests.Remove(expiredRequest);
            Destroy(expiredRequest.gameObject);
            RedistributeQueue();
        }

        private void HandleNodeEconomyUpdated(object sender, EventArgs args) {
            QueryRoadForProducts();
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
