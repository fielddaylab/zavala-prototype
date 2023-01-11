using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala
{
    public class RegionMgr : MonoBehaviour
    {
        public static RegionMgr Instance;
        [SerializeField] private List<LevelRegion> m_levelRegions;
        [HideInInspector] public LevelRegion CurrRegion;

        public void Init() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
                return;
            }
        }

        public void TrackRegion(LevelRegion region) {
            m_levelRegions.Add(region);

            if (m_levelRegions.Count == 1) {
                CurrRegion = region;
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

            Debug.Log("[RegionMgr] position " + pos + " does not map to a region!");
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

            // look on tiles
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                if (hit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return hit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            return null;
        }
    }
}
