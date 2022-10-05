﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    [RequireComponent(typeof(Tile))]
    public class DairyFarm : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private GeneratesPhosphorus m_generatesComponent;
        private Tile m_tileComponent;

        private bool m_firstCycle; // whether this is first cycle. Produces product for free after first cycle

        [SerializeField] private int m_importCost;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();
            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();
            m_tileComponent = this.GetComponent<Tile>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;

            m_firstCycle = true;
        }

        private void StraightToStorage() {
            // produce and add to storage
            List<Resources.Type> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }

            for (int i = 0; i < newProducts.Count; i++) {
                if (!m_storesComponent.TryAddToStorage(newProducts[i])) {
                    Debug.Log("[DairyFarm] Request fulfilled, but storage full!");
                }
                else {
                    m_connectionNodeComponent.UpdateNodeEconomy();
                }
            }
        }


        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[DairyFarm] Cycle completed");

            // Dairy Farms request 1 grain
            m_requestsComponent.QueueRequest();

            if (m_firstCycle) {
                StraightToStorage();
                m_firstCycle = false;
            }

            // Dairy farms produce phosph
            m_generatesComponent.GeneratePipBatch(m_tileComponent);
        }

        private void HandleRequestFulfilled(object sender, EventArgs e) {
            Debug.Log("[DiaryFarm] Request fulfilled");

            StraightToStorage();
        }

        private void HandleRequestExpired(object sender, EventArgs e) {
            Debug.Log("[DairyFarm] Request expired");
            Debug.Log("[DairyFarm] Attempting to purchase import...");

            if (ShopMgr.Instance.TryPurchaseImport(m_importCost)) {
                Debug.Log("[DiaryFarm] Import purchased successfully");
            }
            else {
                Debug.Log("[DairyFarm] Couldn't purchase import!");
            }
        }

        #endregion // Handlers
    }
}