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

        public Color UIRequestDefaultColor;
        public Color UIRequestEnRouteColor;
        public Color UIRequestExpiringColor;

        public Sprite UIRequestBGIcon;
        public Sprite UIRequestExpiringBGIcon;


        [Space(5)]

        [Header("Resource Icons")]
        public Sprite NoneIcon;
        public Sprite FertilizerIcon;
        public Sprite GrainIcon;
        public Sprite MilkIcon;
        public Sprite ManureIcon;
        public Sprite SoilEnricherIcon;

        public Sprite GrainStoredAltIcon;

        public Sprite StoredShadowIcon;

        public Color FertilizerStoredShadowColor;
        public Color GrainStoredShadowColor;
        public Color MilkStoredShadowColor;
        public Color ManureStoredShadowColor;

        [Space(5)]

        [Header("Meshes")]
        public Mesh HoverMesh;

        [Space(5)]

        [Header("Transport")]
        public GameObject TruckPrefab;
        public Material RoadDisrepairMaterial;

        [Header("Node Prefabs")]
        public GameObject CityBlockPrefab;

        #endregion // Inspector

        public void Init() {
            Instance = this;
        }

        public Sprite GetResourceRequestIcon(Resources.Type type) {
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

        public Sprite GetResourceStoredIcon(Resources.Type type) {
            switch (type) {
                case Resources.Type.Fertilizer:
                    return FertilizerIcon;
                case Resources.Type.Grain:
                    return GrainStoredAltIcon;
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
