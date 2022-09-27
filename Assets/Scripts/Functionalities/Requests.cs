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

        private void Awake() {
            m_activeRequests = new List<UIRequest>();
            m_initialQueuePos = GameDB.Instance.UIRequestPrefab.transform.localPosition;
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

        #region Handlers

        private void HandleRequestExpired(object sender, EventArgs e) {
            Debug.Log("[Requests] request expired");
            m_activeRequests.Remove((UIRequest)sender);
            Destroy(((UIRequest)sender).gameObject);
            RedistributeQueue();
        }

        #endregion // Handlers
    }
}
