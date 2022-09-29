using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Zavala.Functionalities
{
    // requests are usually created on a cycle
    // once the request is created, individual requests have individual timers
    [RequireComponent(typeof(ConnectionNode))]
    public class Requests : MonoBehaviour
    {
        public List<Resources.Type> RequestTypes;

        [SerializeField] private bool m_hasTimeout;
        [SerializeField] private float m_requestTimeout;

        [SerializeField] private float m_iconOffsetZ = 0.25f;

        public event EventHandler RequestFulfilled;
        public event EventHandler RequestExpired;

        private List<UIRequest> m_activeRequests;

        private Vector3 m_initialQueuePos;

        private ConnectionNode m_connectionNodeComponent;

        private void Awake() {
            m_activeRequests = new List<UIRequest>();
            m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;

            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
        }

        public void QueueRequest(Resources.Type resourceType) {
            // init and display
            UIRequest newRequest = Instantiate(GameDB.Instance.UIRequestPrefab, this.transform).GetComponent<UIRequest>();
            if (m_hasTimeout) {
                newRequest.Init(resourceType, m_requestTimeout);
            }
            else {
                newRequest.Init(resourceType);
            }

            // add to requests
            m_activeRequests.Add(newRequest);
            newRequest.RequestExpired += HandleRequestExpired;
            RedistributeQueue();

            QueryRoadForProduct(newRequest.GetResourceType());
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

        private void QueryRoadForProduct(Resources.Type resourceType) {
            List<Road> connectedRoads = m_connectionNodeComponent.GetConnectedRoads();
            Debug.Log("[Requests] Querying road... (" + connectedRoads.Count + " roads connected)");

            // find first available
            for (int i = 0; i < connectedRoads.Count; i++) {
                if (connectedRoads[i].ResourceOnRoad(resourceType)) {
                    // TODO: summon a truck from road fleet with this as recipient

                    // TEMP: send resouce immediately
                    Debug.Log("[Requests] Resource on road! Sending to recipient...");
                    // remove from supplier
                    if (connectedRoads[i].GetSupplierOnRoad(resourceType).TryRemoveFromStorage(resourceType)) {
                        // send to recipient
                        ReceiveRequestedProduct(resourceType);
                    }
                }
            }
        }

        private void ReceiveRequestedProduct(Resources.Type resourceType) {
            for (int i = 0; i < m_activeRequests.Count; i++) {
                if (m_activeRequests[i].GetResourceType() == resourceType) {
                    UIRequest toFulfill = m_activeRequests[i];
                    m_activeRequests.RemoveAt(i);
                    Destroy(toFulfill.gameObject);
                    // trigger request fulfilled event
                    Debug.Log("[Requests] Request fulfilled!");
                    break;
                }
            }
        }

        #region Handlers

        private void HandleRequestExpired(object sender, EventArgs e) {
            Debug.Log("[Requests] request expired");
            // TODO: trigger request expired event

            // TODO: try pay for import cost

            m_activeRequests.Remove((UIRequest)sender);
            Destroy(((UIRequest)sender).gameObject);
            RedistributeQueue();
        }

        #endregion // Handlers
    }
}
