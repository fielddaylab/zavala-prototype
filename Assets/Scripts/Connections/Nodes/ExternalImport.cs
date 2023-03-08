using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Roads;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Tile))]
    [RequireComponent(typeof(StoresProduct))]
    public class ExternalImport : MonoBehaviour
    {
        [SerializeField] private  List<Tile> m_prebuildRoad;

        [HideInInspector] public List<RoadSegment> Path;
        [HideInInspector] public StoresProduct StoresComponent;

        private void Awake() {
            StoresComponent = this.GetComponent<StoresProduct>();
        }

        private void Start() {
            // build road
            for (int i = 1; i < m_prebuildRoad.Count - 1; i++) {
                Path.Add(RoadMgr.Instance.CreateRoadSegment(m_prebuildRoad[i].gameObject));
            }

            RoadMgr.Instance.FinalizeRoad(m_prebuildRoad, Path);
        }
    }
}
