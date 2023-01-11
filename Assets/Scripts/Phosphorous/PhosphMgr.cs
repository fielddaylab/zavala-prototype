using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Interact;
using Zavala.Tiles;

namespace Zavala
{
    public class PhosphMgr : MonoBehaviour
    {
        [SerializeField] private float m_movePercent; // what proportion of pips should move

        [SerializeField] private float m_tickTime; // time between simualtion ticks
        private float m_tickTimer;

        private bool m_waterIteration;

        public void Init() {
            m_tickTimer = 0;
            m_waterIteration = false;
        }

        public void SimulateRunoff() {
            if (m_tickTimer == 0) {
                Debug.Log("[PhosphMgr] Start Runoff Tick");
                List<Tile> allTiles = RegionMgr.Instance.GetAllRegionsAllTiles();

                if (m_waterIteration) {
                    Debug.Log("[PhosphMgr] Tick type is Water");
                    List<Tile> waterTiles = allTiles.FindAll(t => t.GetComponent<Water>() != null);

                    // iterate through all tiles and allocate pip movement according to elevations
                    StageMovement(waterTiles);

                    // apply pip movement
                    ApplyMovement(waterTiles);
                }
                else {
                    Debug.Log("[PhosphMgr] Tick type is All");

                    // iterate through all tiles and allocate pip movement according to elevations
                    StageMovement(allTiles);

                    // apply pip movement
                    ApplyMovement(allTiles);
                }
                m_tickTimer = m_tickTime;

                // toggle between all and only water
                m_waterIteration = !m_waterIteration;
            }
            else {
                m_tickTimer = Mathf.Max(m_tickTimer - Time.deltaTime, 0);
            }
        }

        private void StageMovement(List<Tile> allTiles) {
            Debug.Log("[PhosphMgr] Staging Movement");
            for (int t = 0; t < allTiles.Count; t++) {
                // allocate pips that can move
                Tile tileToStage = allTiles[t];
                if (tileToStage.GetPipCount() == 0) {
                    continue;
                }

                Debug.Log("[PhosphMgr] Staging movement for tile " + t);

                int numToAllocate = (int)(tileToStage.GetPipCount() * (m_movePercent * .01));

                for (int p = 0; p < numToAllocate; p++) {
                    PhosphPip pipToTransfer = tileToStage.StageRemovePip(p);

                    // get lowest neighbor(s)
                    Tile lowestNeighbor = FindLowestNeighbor(tileToStage);
                    if (lowestNeighbor.GetElevation() <= tileToStage.GetElevation()) {
                        lowestNeighbor.StageAddPip(pipToTransfer);
                        Debug.Log("[PhosphMgr] Found a lower neighbor");
                    }
                    else {
                        // local min -- add back to source pool
                        tileToStage.StageAddPip(pipToTransfer);
                        Debug.Log("[PhosphMgr] Pip is at a local min");
                    }
                    
                }
            }
        }

        private void ApplyMovement(List<Tile> allTiles) {
            Debug.Log("[PhosphMgr] Applying Movement");
            for (int t = 0; t < allTiles.Count; t++) {
                allTiles[t].ApplyStagedTransfer();
            }
        }

        private Tile FindLowestNeighbor(Tile centerTile) {
            float centerElevation = centerTile.GetElevation();
            List<Tile> surroundingNeighbors = centerTile.GetNeighbors();

            float lowestElevation = Mathf.Infinity;
            Tile lowestNeighbor = null;

            // random start index to disperse the phosphorus more or less uniformly
            int startIndex = Random.Range(0, surroundingNeighbors.Count);

            // second half
            for (int n = startIndex; n < surroundingNeighbors.Count; n++) {
                float neighborElevation = surroundingNeighbors[n].GetElevation();
                if (neighborElevation < lowestElevation) {
                    lowestElevation = neighborElevation;
                    lowestNeighbor = surroundingNeighbors[n];
                }
            }
            // first half
            for (int n = 0; n < startIndex; n++) {
                float neighborElevation = surroundingNeighbors[n].GetElevation();
                if (neighborElevation < lowestElevation) {
                    lowestElevation = neighborElevation;
                    lowestNeighbor = surroundingNeighbors[n];
                }
            }

            return lowestNeighbor;
        }
    }
}
