﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(TriggersEvents))]
    public class PhosphMgr : MonoBehaviour
    {
        private TriggersEvents m_triggersEventsComponent;

        [SerializeField] private float m_movePercent; // what proportion of pips should move

        [SerializeField] private float m_tickTime; // time between simulation ticks
        [SerializeField] private float m_ticksPerDensityRecalculation;

        [SerializeField] private LevelRegion[] m_trackingRegions;
        [SerializeField] private List<UnlockThreshold> m_unlockThresholds;

        private Dictionary<LevelRegion, NutrientDensity> m_densityMap;

        public struct NutrientDensity {
            public float Avg_3; // average density over last three ticks
            public int Density_0; // density now
            public int Density_1; // density last tick group
            public int Density_2; // density 2 tick groups ago

            public NutrientDensity(int initDensity = -1) {
                Avg_3 = initDensity;
                Density_0 = initDensity;
                Density_1 = initDensity;
                Density_2 = initDensity;
            }
        }

        [Serializable]
        public struct UnlockThreshold {
            public List<LevelRegion> Regions;
            public float Threshold;

            public List<int> Unlocks;
        }

        private float m_tickTimer;
        private int m_tickCount;

        private bool m_waterIteration;

        public void Init() {
            m_triggersEventsComponent = this.GetComponent<TriggersEvents>();

            m_tickTimer = 0;
            m_tickCount = 0;
            m_waterIteration = false;

            m_densityMap = new Dictionary<LevelRegion, NutrientDensity>();

            Debug.Log("[Density] num regions to track: " + m_trackingRegions.Length);
            Debug.Log("[Density] num thresholds to track: " + m_unlockThresholds.Count);
            for (int i = 0; i < m_trackingRegions.Length; i++) {
                m_densityMap.Add(m_trackingRegions[i], new NutrientDensity(-1));
            }
        }

        private void Start() {
            EventMgr.Instance.PipsGenerated += HandlePipsGenerated;
        }

        public void SimulateRunoff() {
            if (m_tickTimer == 0) {
                m_tickCount++;

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

            if (m_tickCount == m_ticksPerDensityRecalculation) {
                RecalculateDensities();
                m_tickCount = 0;
                CheckForUnlocks();
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

            EventMgr.Instance.TriggerEvent(ID.PipMovementCompleted, EventArgs.Empty);
        }

        private Tile FindLowestNeighbor(Tile centerTile) {
            float centerElevation = centerTile.GetElevation();
            List<Tile> surroundingNeighbors = centerTile.GetNeighbors();

            float lowestElevation = Mathf.Infinity;
            Tile lowestNeighbor = null;

            // random start index to disperse the phosphorus more or less uniformly
            int startIndex = UnityEngine.Random.Range(0, surroundingNeighbors.Count);

            // second half
            for (int n = startIndex; n < surroundingNeighbors.Count; n++) {
                float neighborElevation = surroundingNeighbors[n].GetElevation();
                if (neighborElevation < lowestElevation) {
                    lowestElevation = neighborElevation;
                    lowestNeighbor = surroundingNeighbors[n];
                }
                else if (neighborElevation == lowestElevation) {
                    if (UnityEngine.Random.Range(0, 2) == 0) {
                        lowestElevation = neighborElevation;
                        lowestNeighbor = surroundingNeighbors[n];
                    }
                }
            }
            // first half
            for (int n = 0; n < startIndex; n++) {
                float neighborElevation = surroundingNeighbors[n].GetElevation();
                if (neighborElevation < lowestElevation) {
                    lowestElevation = neighborElevation;
                    lowestNeighbor = surroundingNeighbors[n];
                }
                else if (neighborElevation == lowestElevation) {
                    if (UnityEngine.Random.Range(0, 2) == 0) {
                        lowestElevation = neighborElevation;
                        lowestNeighbor = surroundingNeighbors[n];
                    }
                }
            }

            return lowestNeighbor;
        }

        private void RecalculateDensities() {
            Debug.Log("[PhosphMgr] [Density] Recalculating densities for " + m_trackingRegions.Length + " regions...");

            foreach (LevelRegion region in m_trackingRegions) {
                NutrientDensity entry = m_densityMap[region];

                bool calcAvg = false;
                if (entry.Density_2 != -1) {
                    calcAvg = true;
                }

                if (calcAvg) {
                    entry.Avg_3 = (entry.Density_0 + entry.Density_1 + entry.Density_2) / 3.0f;
                }
                else {
                    entry.Avg_3 = -1;
                }

                Debug.Log("[PhosphMgr] [Density] " + region.gameObject.name + ": avg: " + entry.Avg_3 + " || now: " + entry.Density_0);

                entry.Density_2 = entry.Density_1;
                entry.Density_1 = entry.Density_0;
                entry.Density_0 = region.IsRegionActive() ? 0 : -1;

                m_densityMap[region] = entry;
            }
        }
        private void CheckForUnlocks() {
            Debug.Log("[PhosphMgr] [Density] checking for unlocks...");

            List<UnlockThreshold> toRemove = new List<UnlockThreshold>();

            for (int i = 0; i < m_unlockThresholds.Count; i++) {
                UnlockThreshold threshold = m_unlockThresholds[i];
                bool thresholdMet = true;
                for (int r = 0; r < threshold.Regions.Count; r++) {
                    if (m_densityMap[threshold.Regions[r]].Avg_3 == -1) {
                        thresholdMet = false;
                        break;
                    }
                    else if (m_densityMap[threshold.Regions[r]].Avg_3 > threshold.Threshold) {
                        thresholdMet = false;
                    }
                }

                if (thresholdMet) {
                    Debug.Log("[PhosphMgr] [Density] threshold met.");
                    // unlock relevant regions
                    for (int unlockIndex = 0; unlockIndex < threshold.Unlocks.Count; unlockIndex++) {
                        int levelIndex = threshold.Unlocks[unlockIndex];
                        Debug.Log("[PhosphMgr] [Density] Unlocking level " + levelIndex + 1);

                        EventMgr.Instance.TriggerEvent(ID.RegionToggled, new RegionToggleEventArgs(levelIndex));

                        if (levelIndex == 2) {
                            // unlock import taxes
                            if (!TriggerTracker.Instance.IsTriggerExpended(SimEventType.ImportTax)) {
                                List<string> unlockList = new List<string>();

                                List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ImportTax);

                                foreach (CardData data in unlockCards) {
                                    unlockList.Add(data.CardID);
                                }

                                EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("You can buy digesters now. Let's use import taxes to keep them competitive.", Advisors.AdvisorID.Economic, unlockList));
                                TriggerTracker.Instance.SetTriggerExpended(SimEventType.ImportTax);
                            }
                        }
                        if (levelIndex == 4) {
                            if (!TriggerTracker.Instance.IsTriggerExpended(SimEventType.ExportTax)) {
                                List<string> unlockList = new List<string>();

                                List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ExportTax);

                                foreach (CardData data in unlockCards) {
                                    unlockList.Add(data.CardID);
                                }

                                EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("You now have access to export depots. We'll need to manage them using export taxes.", Advisors.AdvisorID.Economic, unlockList));
                                TriggerTracker.Instance.SetTriggerExpended(SimEventType.ExportTax);
                            }
                        }
                    }

                    toRemove.Add(threshold);
                }
            }

            foreach (var thresh in toRemove) {
                m_unlockThresholds.Remove(thresh);
            }
        }

        #region Handlers

        private void HandlePipsGenerated(object sender, PipsGeneratedEventArgs args) {
            if (!m_densityMap.ContainsKey(args.Region)) {
                Debug.Log("[PhosphMgr] Pips generated in a region not tracked by PhosphMgr!");
                return;
            }

            Debug.Log("[PhosphMgr] [Density] pips generated: " + args.Quantity);

            NutrientDensity entry = m_densityMap[args.Region];
            if (entry.Density_0 == -1) {
                entry.Density_0 = args.Quantity;
            }
            else {
                entry.Density_0 += args.Quantity;
            }

            m_densityMap[args.Region] = entry;

            Debug.Log("[PhosphMgr] [Density] pips saved: " + m_densityMap[args.Region].Density_0);
        }

        #endregion // Handlers
    }
}
