using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.DataDefs;

namespace Zavala
{
    public class GameDB : MonoBehaviour
    {
        public static GameDB Instance;

        #region Inspector

        [Header("UI")]
        public GameObject UITimerPrefabDefault;
        public GameObject UIRequestPrefab;
        public GameObject UIStoredProductPrefab;
        public GameObject PipPrefab;
        public GameObject UIBlurbIconPrefab;
        public GameObject UIInspectPrefab;

        [Space(5)]

        [Header("Resource Icons")]
        public Sprite NoneIcon;
        public Sprite FertilizerIcon;
        public Sprite GrainIcon;
        public Sprite MilkIcon;
        public Sprite ManureIcon;
        public Sprite SoilEnricherIcon;

        [Space(5)]

        [Header("Meshes")]
        public Mesh HoverMesh;

        [Space(5)]

        [Header("Transport")]
        public GameObject TruckPrefab;
        public Color RoadDamagedColor;

        [Header("Node Prefabs")]
        public GameObject CityBlockPrefab;

        #endregion // Inspector

        public void Init() {
            Instance = this;
        }

        public Sprite GetResourceIcon(Resources.Type type) {
            switch(type) {
                case Resources.Type.Fertilizer:
                    return FertilizerIcon;
                case Resources.Type.Grain:
                    return GrainIcon;
                case Resources.Type.Milk:
                    return MilkIcon;
                case Resources.Type.Manure:
                    return ManureIcon;
                case Resources.Type.SoilEnricher:
                    return SoilEnricherIcon;
                default:
                    return NoneIcon;
            }
        }
    }
}
