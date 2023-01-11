using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Settings;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(Cycles))]
    public class TileGenerator : MonoBehaviour, IAllVars
    {
        private Cycles m_cyclesComponent;

        [SerializeField] private GameObject[] m_generateTilePrefabs;
        private int m_prefabIndex;

        public enum GenerateMode {
            Replace,
            Boundary
        }

        [SerializeField] private GenerateMode m_generateMode;

        public void Init() {
            m_cyclesComponent = this.GetComponent<Cycles>();

            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;

            m_prefabIndex = 0;

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;
        }

        #region Helpers

        private void GenerateReplaceTile() {
            /*
            Debug.Log("[TileGenerator] Generating replacement tile...");
            Tile toReplace = GridMgr.GetRandomTile(true);
            if (toReplace == null) {
                Debug.Log("[TileGenerator] Generation failed: GridMgr yielded a null tile");
                return;
            }
            GameObject newPrefab = SelectNextPrefab();
            Tile newTile = Instantiate(newPrefab).GetComponent<Tile>();
            GridMgr.ReplaceTile(toReplace, newTile);
            Debug.Log("[TileGenerator] Finished generating replacement tile");
            */
        }

        private void GenerateBoundaryTile() {
            /*
            Debug.Log("[TileGenerator] Generating boundary tile");

            Tile toAppend = GridMgr.GetRandomBoundaryTile();
            if (toAppend == null) {
                Debug.Log("[TileGenerator] Generation failed: GridMgr yielded a null tile");
                return;
            }
            GameObject newPrefab = SelectNextPrefab();
            Tile newTile = Instantiate(newPrefab).GetComponent<Tile>();
            GridMgr.AppendToTile(toAppend, newTile);
            */
        }

        private GameObject SelectNextPrefab() {
            GameObject nextPrefab = m_generateTilePrefabs[m_prefabIndex];
            m_prefabIndex++;
            if (m_prefabIndex >= m_generateTilePrefabs.Length) {
                m_prefabIndex = 0;
            }

            return nextPrefab;
        }

        #endregion // Helpers

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs args) {
            Debug.Log("[TileGenerator] Cycle completed.");

            if (m_generateMode == GenerateMode.Replace) {
                GenerateReplaceTile();
            }
            else if (m_generateMode == GenerateMode.Boundary) {
                GenerateBoundaryTile();
            }

            Debug.Log("[TileGenerator] Cycle Handling completed from sender: " + sender);
        }

        #endregion // Handlers

        #region AllVars

        public void SetRelevantVars(ref AllVars defaultAllVars) {
            defaultAllVars.TileGeneratorCycleTime = this.GetComponent<Cycles>().CycleTime;
            defaultAllVars.TileGeneratorMode = m_generateMode;
        }

        public void HandleAllVarsUpdated(object sender, AllVarsEventArgs args) {
            this.GetComponent<Cycles>().CycleTime = args.UpdatedVars.TileGeneratorCycleTime;
            Debug.Log("[TileGenerator] new cycle time: " + args.UpdatedVars.TileGeneratorCycleTime);

            m_generateMode = args.UpdatedVars.TileGeneratorMode;
            Debug.Log("[TileGenerator] Setting new mode: " + m_generateMode);
        }


        #endregion // AllVars
    }

}