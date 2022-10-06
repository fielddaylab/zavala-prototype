using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(AddOn))]
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    public class Skimmer : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private GeneratesPhosphorus m_generatesComponent;

        [SerializeField] private int m_skimAmt;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();
            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();

            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
            m_storesComponent.StorageExpired += HandleStorageExpired;
        }

        private bool TrySkimLakes() {
            bool skimmedAny = false;

            Tile tileUnderneath = GridMgr.TileAtPos(this.transform.position);
            List<Tile> neighborTiles = GridMgr.GetAdjTiles(tileUnderneath);

            for (int n = 0; n < neighborTiles.Count; n++) {
                Water waterComp = neighborTiles[n].GetComponent<Water>();
                if (waterComp != null) {
                    if (waterComp.TrySkim(m_skimAmt)) {
                        //Debug.Log("[Skimmer] Skimmed tile!");

                        skimmedAny = true;
                    }
                }
            }

            // skim phosphorus
            return skimmedAny;
        }

        private void LeakBack() {
            Tile tileUnderneath = GridMgr.TileAtPos(this.transform.position);
            m_generatesComponent.GeneratePipBatch(tileUnderneath);
        }

        private void StraightToStorage() {
            // produce money per population
            List<Resources.Type> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }

            for (int i = 0; i < newProducts.Count; i++) {
                if (!m_storesComponent.TryAddToStorage(newProducts[i])) {
                    Debug.Log("[Skimmer] storage full! Not creating more fertilizer");
                }
                else {
                    Debug.Log("[Skimmer] added fert to storage");
                    m_connectionNodeComponent.UpdateNodeEconomy();
                }
            }
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs args) {
            if (TrySkimLakes()) {
                StraightToStorage();
            }
        }

        private void HandleStorageExpired(object sender, EventArgs args) {
            Debug.Log("[Skimmer] storage expired");
            LeakBack();
        }

        #endregion // Handlers
    }
}
