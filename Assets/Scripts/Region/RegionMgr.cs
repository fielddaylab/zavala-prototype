using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Tiles;
using static Zavala.LevelRegion;

namespace Zavala
{
    public class RegionMgr : MonoBehaviour
    {
        [Serializable]
        public struct GlobalSimulationKnobs
        {
            // Percentages

            // Flat Values
            public TransportCosts TransportCosts;
        }

        [Serializable]
        public struct TransportCosts
        {
            public float Manure;
            public float Fertilizer;
            public float Grain;
            public float Milk;
        }

        public static RegionMgr Instance;
        [SerializeField] private List<LevelRegion> m_levelRegions;
        [HideInInspector] public LevelRegion CurrRegion;

        [Header("Global Simulation Knobs")]
        public GlobalSimulationKnobs GlobalSimKnobs;

        public void Init() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
                return;
            }

            EventMgr.Instance.CameraMoved += HandleCameraMoved;
        }

        public void TrackRegion(LevelRegion region) {
            m_levelRegions.Add(region);

            if (m_levelRegions.Count == 1) {
                CurrRegion = region;

                Debug.Log("[RegionMgr] New current region: " + CurrRegion.gameObject.name);
                EventMgr.Instance.TriggerEvent(ID.RegionSwitched, new RegionSwitchedEventArgs(CurrRegion));
            }
        }

        public void UntrackRegion(LevelRegion region) {
            m_levelRegions.Remove(region);
        }

        private void UntrackAllRegions() {
            m_levelRegions.Clear();
        }

        public LevelRegion GetRegionByPos(Vector3 pos) {
            foreach(LevelRegion region in m_levelRegions) {
                if (region.WithinBounds(pos)) {
                    return region;
                }
            }

            Debug.Log("[BoundsCheck] position " + pos + " does not map to a region!");
            return null;
        }

        public List<Tile> GetAllRegionsAllTiles() {
            List<Tile> allTiles = new List<Tile>();

            for (int i = 0; i < m_levelRegions.Count; i++) {
                List<Tile> regionTiles = m_levelRegions[i].GridMgr.GetAllTiles();
                if (regionTiles == null) { continue; }
                for (int t = 0; t < regionTiles.Count; t++) {
                    allTiles.Add(regionTiles[t]);
                }
            }

            return allTiles;
        }


        public static Inspectable OverInspectable(Vector2 pos) {
            // Look for inspectable
            Ray ray;
            RaycastHit[] hits;

            ray = Camera.main.ScreenPointToRay(pos);
            hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Inspect"));
            foreach (RaycastHit inspectHit in hits) {
                if (inspectHit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return inspectHit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            RaycastHit hit;

            // look on tolls
            ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Toll"))) {
                if (hit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return hit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            // look on tiles
            ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                if (hit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return hit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            return null;
        }

        public void UpdateCurrRegion(Vector2 pos) {
            Ray ray;
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                LevelRegion overRegion = GetRegionByPos(hit.collider.gameObject.transform.position);
                if (overRegion != CurrRegion) {
                    CurrRegion = GetRegionByPos(hit.collider.gameObject.transform.position);
                    Debug.Log("[RegionMgr] New current region: " + CurrRegion.gameObject.name);

                    EventMgr.Instance.TriggerEvent(ID.RegionSwitched, new RegionSwitchedEventArgs(CurrRegion));
                }
            }
            else {
                Debug.Log("[RegionMgr] No current region! Keeping default.");
            }
        }


        #region Handlers

        private void HandleCameraMoved(object sender, EventArgs args) {
            UpdateCurrRegion(new Vector2(Screen.width / 2, Screen.height / 2));
        }

        #endregion // Handlers
    }
}
