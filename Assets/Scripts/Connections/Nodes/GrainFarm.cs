using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Resources;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(Tile))]
    [RequireComponent(typeof(Inspectable))]

    public class GrainFarm : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private GeneratesPhosphorus m_generatesComponent;
        private Tile m_tileComponent;
        private Inspectable m_inspectComponent;

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
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;

            m_firstCycle = true;
        }

        private void Start() {
            m_inspectComponent.Init();
        }

        private void OnDisable() {
            if (m_requestsComponent != null) {
                m_requestsComponent.RequestFulfilled -= HandleRequestFulfilled;
                m_requestsComponent.RequestExpired -= HandleRequestExpired;
            }
            if (m_cyclesComponent != null) {
                m_cyclesComponent.CycleCompleted -= HandleCycleCompleted;
            }
        }

        private void StraightToStorage() {
            // produce and add to storage
            List<Resources.Type> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }

            for (int i = 0; i < newProducts.Count; i++) {
                if (!m_storesComponent.TryAddToStorage(newProducts[i])) {
                    Debug.Log("[GrainFarm] Request fulfilled, but storage full!");
                }
                else {
                    m_connectionNodeComponent.UpdateNodeEconomy();
                }
            }
        }

        private void GenerateRunoff(Resources.Type resourceType) {
            m_generatesComponent.GeneratePipBatch(m_tileComponent, resourceType);
        }


        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[GrainFarm] Cycle completed");

            // Grain Farms request 1 soilEnricher (Manure OR Fertilizer)
            m_requestsComponent.QueueRequest();

            if (m_firstCycle) {
                StraightToStorage();
                m_firstCycle = false;
            }
        }

        private void HandleRequestFulfilled(object sender, ResourceEventArgs args) {
            Debug.Log("[GrainFarm] Request fulfilled");

            StraightToStorage();

            GenerateRunoff(args.ResourceType);
        }

        private void HandleRequestExpired(object sender, ResourceEventArgs args) {
            Debug.Log("[GrainFarm] Request expired");
            Debug.Log("[GrainFarm] Attempting to purchase import...");

            if (ShopMgr.Instance.TryPurchaseImport(m_importCost)) {
                Debug.Log("[GrainFarm] Import purchased successfully");
            }
            else {
                Debug.Log("[GrainFarm] Couldn't purchase import!");
            }
        }

        #endregion // Handlers
    }
}
