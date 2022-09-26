using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    public class GrainFarm : MonoBehaviour
    {
        private Requests m_requestsComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;

        private void Awake() {
            m_requestsComponent = this.GetComponent<Requests>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();

            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[GrainFarm] Cycle completed");

            // Grain Farms request 1 Fertilizer (or 1 Manure?)
            m_requestsComponent.QueueRequest(Resources.Type.Manure);
        }

        #endregion // Handlers
    }
}
