using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Functionalities
{
    // requests are usually created on a cycle
    // once the request is created, individual requests have individual timers
    public class Requests : MonoBehaviour
    {
        public List<Resources.Type> RequestTypes;

        public event EventHandler RequestFulfilled;
        public event EventHandler RequestUnfulfilled;

        private List<UIRequest> m_activeRequests;

        public void QueueRequest(Resources.Type resourceType) {
            DisplayRequest(resourceType);
        }

        private void DisplayRequest(Resources.Type resourceType) {
            UIRequest newRequest = Instantiate(GameDB.Instance.UIRequestPrefab, this.transform).GetComponent<UIRequest>();
            newRequest.Init(resourceType);
        }
    }
}
