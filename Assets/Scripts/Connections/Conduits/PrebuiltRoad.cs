using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zavala.Roads;
using Zavala.Tiles;

namespace Zavala {
    public class PrebuiltRoad : MonoBehaviour
    {
        [SerializeField] private List<Tile> m_prebuiltRoad;
        [HideInInspector] public List<RoadSegment> Path;

        private void Start() {
            Path = new List<RoadSegment>();

            // build road
            for (int i = 1; i < m_prebuiltRoad.Count - 1; i++) {
                Path.Add(RoadMgr.Instance.CreateRoadSegment(m_prebuiltRoad[i].gameObject));
            }

            RoadMgr.Instance.FinalizeRoad(m_prebuiltRoad, Path);
        }
    }
}