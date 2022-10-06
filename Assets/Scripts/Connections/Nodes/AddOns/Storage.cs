using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Requests))]
    public class Storage : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private StoresProduct m_storesComponent;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_storesComponent = this.GetComponent<StoresProduct>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_storesComponent.RemovedStorage += HandleRemovedStorage;
        }

        private void Start() {
            m_requestsComponent.QueueRequest();
        }

        private void StraightToStorage() {
            if (!m_storesComponent.TryAddToStorage(m_requestsComponent.RequestTypes[0])) {
                Debug.Log("[Storage] Request fulfilled, but storage full!");
            }
            else {
                m_connectionNodeComponent.UpdateNodeEconomy();
            }
        }

        #region Handlers
        private void HandleRequestFulfilled(object sender, EventArgs e) {
            Debug.Log("[Storage] Request fulfilled");

            StraightToStorage();

            // add new request if storage not full
            if (m_requestsComponent.GetNumActiveRequests() == 0 && !m_storesComponent.IsStorageFull()) {
                m_requestsComponent.QueueRequest();
            }
        }

        private void HandleRemovedStorage(object sender, EventArgs e) {
            Debug.Log("[Storage] Resource removed");

            // add request
            if (m_requestsComponent.GetNumActiveRequests() == 0) {
                m_requestsComponent.QueueRequest();
            }
        }

        #endregion // Handlers
    }
}