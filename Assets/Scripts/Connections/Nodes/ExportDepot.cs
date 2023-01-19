using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Resources;
using Zavala.Tiles;
using static Zavala.Functionalities.Produces;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Tile))]
    [RequireComponent(typeof(Produces))]
    public class ExportDepot : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private Tile m_tileComponent;
        private Produces m_producesComponent;


        private void OnEnable() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_tileComponent = this.GetComponent<Tile>();
            m_producesComponent = this.GetComponent<Produces>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired;
        }

        private void SellExport() {
            // produce money for selling export
            List<ProductBundle> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }
        }

        #region Handlers

        private void HandleRequestFulfilled(object sender, ResourceEventArgs args) {
            Debug.Log("[ExportDepot] Request fulfilled");
        }

        private void HandleRequestExpired(object sender, EventArgs args) {
            Debug.Log("[ExportDepot] Request expired");
        }

        #endregion // Handlers
    }
}