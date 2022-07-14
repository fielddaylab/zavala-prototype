using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Sim;

namespace Zavala.Factions
{
    public enum FactionType
    {
        Agriculture,
        Finance, 
        Transport,
        Government
    }

    public class FactionsDisplay : MonoBehaviour {
        [SerializeField] private FactionSlider[] m_factionSliders;

        private Dictionary<SimAction, List<FactionType>> m_factionLikeDict = null;
        private Dictionary<SimAction, List<FactionType>> m_factionDislikeDict = null;

        private static float SLIDER_DEFAULT = 0.5f;

        private static float FACTION_SWAY = 0.1f;


        private void Start() {
            InitDicts();
        }

        private void InitDicts() {
            m_factionLikeDict = new Dictionary<SimAction, List<FactionType>>();
            m_factionDislikeDict = new Dictionary<SimAction, List<FactionType>>();

            // Dislikes
            m_factionDislikeDict.Add(SimAction.MoveFarm, new List<FactionType> { FactionType.Agriculture, FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.RaisePhosphorousOutput, new List<FactionType> { FactionType.Agriculture });
            m_factionDislikeDict.Add(SimAction.IncreaseUptake, new List<FactionType> { FactionType.Transport });
            m_factionDislikeDict.Add(SimAction.BuildStorage, new List<FactionType> { FactionType.Transport });
            m_factionDislikeDict.Add(SimAction.BuildRail, new List<FactionType> { FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.BuildHighway, new List<FactionType> { FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.BuildRoad, new List<FactionType> { FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.BuildBridge, new List<FactionType> { FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.IncreaseGovernmentPolicy, new List<FactionType> { FactionType.Agriculture, FactionType.Transport });
            m_factionDislikeDict.Add(SimAction.IncreasePrivatePolicy, new List<FactionType> { FactionType.Government, FactionType.Finance });
            m_factionDislikeDict.Add(SimAction.DecreaseGovernmentPolicy, new List<FactionType> { FactionType.Finance, FactionType.Government });
            m_factionDislikeDict.Add(SimAction.DecreasePrivatePolicy, new List<FactionType> { FactionType.Agriculture });
            m_factionDislikeDict.Add(SimAction.BuildBasicExchange, new List<FactionType> { FactionType.Government });
            m_factionDislikeDict.Add(SimAction.BuildDigesterExchange, new List<FactionType> { FactionType.Government });
            //m_factionDislikeDict.Add(SimAction.DistrictStop, new List<FactionType> { FactionType.Finance });
            //m_factionDislikeDict.Add(SimAction.DistrictAd, new List<FactionType> { FactionType.Finance });

            // Likes
            m_factionLikeDict.Add(SimAction.LowerPhosphorousOutput, new List<FactionType> { FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.IncreaseUptake, new List<FactionType> { FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.BuildStorage, new List<FactionType> { FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.BuildRail, new List<FactionType> { FactionType.Transport });
            m_factionLikeDict.Add(SimAction.BuildHighway, new List<FactionType> { FactionType.Transport });
            m_factionLikeDict.Add(SimAction.BuildRoad, new List<FactionType> { FactionType.Transport });
            m_factionLikeDict.Add(SimAction.BuildBridge, new List<FactionType> { FactionType.Transport });
            m_factionLikeDict.Add(SimAction.IncreaseGovernmentPolicy, new List<FactionType> { FactionType.Finance, FactionType.Government });
            m_factionLikeDict.Add(SimAction.IncreasePrivatePolicy, new List<FactionType> { FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.DecreaseGovernmentPolicy, new List<FactionType> { FactionType.Agriculture, FactionType.Transport });
            m_factionLikeDict.Add(SimAction.DecreasePrivatePolicy, new List<FactionType> { FactionType.Government, FactionType.Finance });
            m_factionLikeDict.Add(SimAction.BuildBasicExchange, new List<FactionType> { FactionType.Finance, FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.BuildDigesterExchange, new List<FactionType> { FactionType.Finance, FactionType.Agriculture });
            m_factionLikeDict.Add(SimAction.DistrictStop, new List<FactionType> { FactionType.Government });
            m_factionLikeDict.Add(SimAction.DistrictAd, new List<FactionType> { FactionType.Government });
        }

        public void ResetSliders() {
            foreach (var slider in m_factionSliders) {
                slider.Slider.value = SLIDER_DEFAULT;
            }
        }

        public void InterpretAction(SimAction action) {
            if (m_factionDislikeDict == null) { InitDicts(); }

            if (m_factionDislikeDict.ContainsKey(action)) {
                // lower faction scores
                foreach (FactionType faction in m_factionDislikeDict[action]) {
                    foreach (FactionSlider slider in m_factionSliders) {
                        if (slider.Type == faction) {
                            slider.Slider.value -= FACTION_SWAY;
                        }
                    }
                }
            }

            if (m_factionLikeDict.ContainsKey(action)) {
                // raise faction scores
                foreach (FactionType faction in m_factionLikeDict[action]) {
                    foreach (FactionSlider slider in m_factionSliders) {
                        if (slider.Type == faction) {
                            slider.Slider.value += FACTION_SWAY;
                        }
                    }
                }
            }
        }

        public List<FactionType> FactionsDislikingAction(SimAction action) {
            if (m_factionDislikeDict.ContainsKey(action)) {
                return m_factionDislikeDict[action];
            }
            else {
                return new List<FactionType>();
            }
        }

        public List<FactionType> FactionsLikingAction(SimAction action) {
            if (m_factionLikeDict.ContainsKey(action)) {
                return m_factionLikeDict[action];
            }
            else {
                return new List<FactionType>();
            }
        }
    }
}