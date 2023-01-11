using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala.Functionalities
{
    [RequireComponent(typeof(Cycles))]
    public class BloomAffectable : MonoBehaviour
    {
        private Cycles m_cyclesComponent;

        [SerializeField] private int m_bloomTolerance; // how many cycles before bloom affects this tile
        private int m_bloomTally; // how many cycles it has been next to a bloom

        public event EventHandler BloomEffect;

        private void OnEnable() {
            m_cyclesComponent = this.GetComponent<Cycles>();

            m_cyclesComponent.PreCycleCompleted += HandlePreCycleCompleted;
        }

        private void OnDisable() {
            m_cyclesComponent.PreCycleCompleted -= HandlePreCycleCompleted;
        }

        private bool BloomAdjacent() {
            Tile tileUnderneath = RegionMgr.Instance.GetRegionByPos(this.transform.position).GridMgr.TileAtPos(this.transform.position);
            List<Tile> neighborTiles = RegionMgr.Instance.GetRegionByPos(tileUnderneath.transform.position).GridMgr.GetAdjTiles(tileUnderneath);

            for (int n = 0; n < neighborTiles.Count; n++) {
                Water waterComp = neighborTiles[n].GetComponent<Water>();
                if (waterComp != null) {
                    if (waterComp.IsInBloom()) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void IncrementBloomTally() {
            m_bloomTally++;

            if (m_bloomTally == m_bloomTolerance) {
                Debug.Log("[BloomAffectable] Hit bloom tolerance");

                BloomEffect?.Invoke(this, EventArgs.Empty);
                ResetBloomTally();
            }
        }

        private void ResetBloomTally() {
            m_bloomTally = 0;
        }

        #region Handlers

        private void HandlePreCycleCompleted(object sender, EventArgs args) {
            Debug.Log("[BloomAffectable] Cycle completed, checking for bloom...");

            if (BloomAdjacent()) {
                Debug.Log("[BloomAffectable] Adjacent to bloom");
                IncrementBloomTally();
            }
            else {
                Debug.Log("[BloomAffectable] Not adjacent to bloom");
                ResetBloomTally();
            }
        }

        #endregion // Handlers

        #region AllVars Gets & Sets

        public void SetBloomTolerance(int newVal) {
            m_bloomTolerance = newVal;
        }

        public int GetBloomTolerance() {
            return m_bloomTolerance;
        }

        #endregion // AllVars Gets & Sets
    }
}