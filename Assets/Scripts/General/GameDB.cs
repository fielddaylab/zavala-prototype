using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class GameDB : MonoBehaviour
    {
        public static GameDB Instance;

        #region Inspector

        [Header("UI Timer")]
        public GameObject UITimerPrefabDefault;

        [Space(5)]

        [Header("UI Request")]
        public GameObject UIRequestPrefab;

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
                    break;
                case Resources.Type.Grain:
                    return GrainIcon;
                    break;
                case Resources.Type.Milk:
                    return MilkIcon;
                    break;
                case Resources.Type.Manure:
                    return ManureIcon;
                    break;
                default:
                    return NoneIcon;
                    break;
            }
        }
    }
}
