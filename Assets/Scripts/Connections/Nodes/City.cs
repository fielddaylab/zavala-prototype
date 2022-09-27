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
    public class City : MonoBehaviour
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

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired ;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
        }


        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[City] Cycle completed");

            // Cities request 1 milk / population
            m_requestsComponent.QueueRequest(Resources.Type.Milk);
        }

        private void HandleRequestFulfilled(object sender, EventArgs e) {
            Debug.Log("[City] Request fulfilled");
        }

        private void HandleRequestExpired(object sender, EventArgs e) {
            Debug.Log("[City] Request expired");
        }

        #endregion // Handlers

    }
}
