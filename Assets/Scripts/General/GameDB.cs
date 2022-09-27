using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [Space(5)]

        [Header("Resource Icons")]
        public Sprite NoneIcon;
        public Sprite FertilizerIcon;
        public Sprite GrainIcon;
        public Sprite MilkIcon;
        public Sprite ManureIcon;


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
                default:
                    return NoneIcon;
            }
        }
    }
}
