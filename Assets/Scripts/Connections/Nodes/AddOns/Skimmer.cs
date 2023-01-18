using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Resources;
using Zavala.Settings;
using Zavala.Tiles;
using static Zavala.Functionalities.Produces;

namespace Zavala
{
    [RequireComponent(typeof(AddOn))]
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    [RequireComponent(typeof(Inspectable))]
    public class Skimmer : MonoBehaviour, IAllVars
    {
        private ConnectionNode m_connectionNodeComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private GeneratesPhosphorus m_generatesComponent;
        private Inspectable m_inspectComponent;

        [SerializeField] private int m_skimAmt;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();
            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
            m_storesComponent.StorageExpired += HandleStorageExpired;

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;
            ProcessUpdatedVars(SettingsMgr.Instance.GetCurrAllVars());
        }

        private void Start() {
            m_inspectComponent.Init();
        }

        private void OnDisable() {
            if (m_cyclesComponent != null) {
                m_cyclesComponent.CycleCompleted -= HandleCycleCompleted;
            }
            if (m_storesComponent != null) {
                m_storesComponent.StorageExpired -= HandleStorageExpired;
            }

            EventMgr.Instance.AllVarsUpdated -= HandleAllVarsUpdated;
        }

        private bool TrySkimLakes() {
            bool skimmedAny = false;

            Tile tileUnderneath = RegionMgr.Instance.CurrRegion.GridMgr.TileAtPos(this.transform.position);
            List<Tile> neighborTiles = RegionMgr.Instance.CurrRegion.GridMgr.GetAdjTiles(tileUnderneath);

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

        private void LeakBack(Resources.Type resourceType) {
            Tile tileUnderneath = RegionMgr.Instance.CurrRegion.GridMgr.TileAtPos(this.transform.position);
            m_generatesComponent.GeneratePipBatch(tileUnderneath, resourceType);
        }

        private void StraightToStorage() {
            List<ProductBundle> newProducts = m_producesComponent.Produce();
            if (newProducts == null) {
                return;
            }

            for (int i = 0; i < newProducts.Count; i++) {
                if (!m_storesComponent.TryAddToStorage(newProducts[i].Type, newProducts[i].Units)) {
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

        private void HandleStorageExpired(object sender, ResourceEventArgs args) {
            Debug.Log("[Skimmer] storage expired");
            LeakBack(args.ResourceType);
        }

        #endregion // Handlers

        #region All Vars Settings

        public void SetRelevantVars(ref AllVars defaultVars) {
            defaultVars.SkimmerSkimAmt = m_skimAmt;
            defaultVars.SkimmerCycleTime = this.GetComponent<Cycles>().CycleTime;
            defaultVars.SkimmerExpiredRunoffAmt = this.GetComponent<GeneratesPhosphorus>().GetAmtForResource(Resources.Type.Fertilizer);
            if (defaultVars.SkimmerExpiredRunoffAmt == -1) {
                Debug.Log("[Skimmer] Skimmer does not produce phosph for fertilizer.");
            }
        }

        public void HandleAllVarsUpdated(object sender, AllVarsEventArgs args) {
            ProcessUpdatedVars(args.UpdatedVars);
        }

        #endregion // All Vars Settings


        #region AllVars Helpers

        private void ProcessUpdatedVars(AllVars updatedVars) {
            m_skimAmt = updatedVars.SkimmerSkimAmt;
            this.GetComponent<Cycles>().CycleTime = updatedVars.SkimmerCycleTime;
            this.GetComponent<GeneratesPhosphorus>().SetAmtForResource(Resources.Type.Fertilizer, updatedVars.SkimmerExpiredRunoffAmt);
        }

        #endregion // AllVars Helpers
    }
}
