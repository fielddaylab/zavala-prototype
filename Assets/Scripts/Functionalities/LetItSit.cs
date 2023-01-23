using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Resources;

namespace Zavala.Functionalities
{
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    // [RequireComponent(typeof(Inspectable))]
    public class LetItSit : MonoBehaviour
    {
        [HideInInspector] public Requests RequestsComp;

        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private GeneratesPhosphorus m_generatesComponent;
        //private Inspectable m_inspectComponent;

        public void Init() {
            RequestsComp = this.GetComponent<Requests>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();
            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();
            //m_inspectComponent = this.GetComponent<Inspectable>();

            RequestsComp.RequestFulfilled += HandleRequestFulfilled;
            RequestsComp.RequestExpired += HandleRequestExpired;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
            m_storesComponent.RemovedStorage += HandleStorageRemoved;

            RegionMgr.Instance.GetRegionByPos(this.transform.position).RegisterWithClearingHouse(this.RequestsComp);

            RequestsComp.QueueRequest();
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[LetItSit] Cycle completed");
        }

        private void HandleRequestFulfilled(object sender, ResourceEventArgs args) {
            Debug.Log("[LetItSit] Request fulfilled");
        }

        private void HandleRequestExpired(object sender, EventArgs args) {
            Debug.Log("[LetItSit] Request expired");
        }

        private void HandleStorageRemoved(object sender, ResourceEventArgs args) {

        }

        #endregion // Handlers
    }
}