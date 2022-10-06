using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;

namespace Zavala
{
    [RequireComponent(typeof(AddOn))]
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    public class Digester : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Requests m_requestsComponent;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_requestsComponent = this.GetComponent<Requests>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
        }

        private void Start() {
            m_requestsComponent.QueueRequest();
        }

        private void StraightToStorage() {
            List<Resources.Type> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }

            for (int i = 0; i < newProducts.Count; i++) {
                if (!m_storesComponent.TryAddToStorage(newProducts[i])) {
                    Debug.Log("[Digester] storage full! Not creating new product");
                }
                else {
                    m_connectionNodeComponent.UpdateNodeEconomy();
                }
            }
        }

        #region Handlers

        private void HandleRequestFulfilled(object sender, EventArgs e) {
            Debug.Log("[Digester] Request fulfilled");

            StraightToStorage();

            // add new request if storage not full
            if (m_requestsComponent.GetNumActiveRequests() == 0 && !m_storesComponent.IsStorageFull()) {
                m_requestsComponent.QueueRequest();
            }
        }

        #endregion // Handlers

    }
}
