using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Resources;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Inspectable))]
    public class Storage : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private StoresProduct m_storesComponent;
        private Inspectable m_inspectComponent;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_storesComponent.RemovedStorage += HandleRemovedStorage;
        }

        private void Start() {
            m_inspectComponent.Init();
            m_requestsComponent.QueueRequest();
        }

        private void OnDisable() {
            if (m_requestsComponent != null) {
                m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            }
            if (m_storesComponent != null) {
                m_storesComponent.RemovedStorage += HandleRemovedStorage;
            }
        }

        private void StraightToStorage(Resources.Type resourceType, int units) {
            if (!m_storesComponent.TryAddToStorage(resourceType, units)) {
                Debug.Log("[Storage] Request fulfilled, but storage full!");
            }
            else {
                m_connectionNodeComponent.UpdateNodeEconomy();
            }
        }

        #region Handlers
        private void HandleRequestFulfilled(object sender, ResourceEventArgs e) {
            Debug.Log("[Storage] Request fulfilled");

            StraightToStorage(e.ResourceType, e.Units);

            // add new request if storage not full
            if (m_requestsComponent.GetNumActiveRequests() == 0 && !m_storesComponent.IsStorageFull()) {
                m_requestsComponent.QueueRequest();
            }
        }

        private void HandleRemovedStorage(object sender, ResourceEventArgs e) {
            Debug.Log("[Storage] Resource removed");

            // add request
            if (m_requestsComponent.GetNumActiveRequests() == 0) {
                m_requestsComponent.QueueRequest();
            }
        }

        #endregion // Handlers
    }
}