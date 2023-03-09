using System;
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
    [RequireComponent(typeof(PrebuiltRoad))]
    public class ExternalImport : MonoBehaviour
    {
        [SerializeField] private PrebuiltRoad m_prebuiltRoadComp;
        [HideInInspector] public StoresProduct StoresComponent;

        private void Awake() {
            StoresComponent = this.GetComponent<StoresProduct>();
        }

        public List<RoadSegment> GetPath() {
            return m_prebuiltRoadComp.Path;
        }
    }
}
