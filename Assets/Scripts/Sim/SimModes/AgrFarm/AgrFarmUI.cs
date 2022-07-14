using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;

namespace Zavala
{
    public class AgrFarmUI : SimModeUI
    {
        [Header("AgrFarmUI")]

        [SerializeField] private AgrFarmInteractable[] m_farms;
        [SerializeField] private Lake[] m_lakes;

        [SerializeField] private float m_minDist, m_maxDist;
        private float m_distWindow;


        private void Awake() {
            base.Awake();

            m_distWindow = m_maxDist - m_minDist;

            EventMgr.FarmMoved.AddListener(OnFarmMoved);
        }

        private void OnEnable() {
            base.OnEnable();
        }

        private void OnDisable() {
            base.OnDisable();
        }

        public override void Open() {
            CalcOutbreaks();

            EventMgr.InteractModeUpdated?.Invoke(InteractMode.Default);
        }

        public float GetNearestLakeDist(Vector2 farmPos) {
            //Lake nearestLake;
            float nearestDist = Mathf.Infinity;

            foreach (Lake lake in m_lakes) {
                float dist = CalcDist(farmPos, lake.transform.position);
                if (dist <= nearestDist) {
                    //nearestLake = lake;
                    nearestDist = dist;
                }
            }

            return nearestDist;
        }

        private float CalcDist(Vector2 loc1, Vector2 loc2) {
            return Vector2.Distance(loc1, loc2);
        }

        private void CalcOutbreaks() {
            float maxContribution = 1f / m_farms.Length;
            float farmContribution;
            float totalContributions = 0;

            foreach (AgrFarmInteractable farm in m_farms) {
                float farmDist = Mathf.Clamp(GetNearestLakeDist(farm.transform.position), m_minDist, m_maxDist);
                float adjDist = farmDist - m_minDist;

                farmContribution = ((m_distWindow - adjDist) / m_distWindow) * maxContribution;
                totalContributions += farmContribution;
            }

            IndicatorMgr.Instance.SetIndicatorValue(0, totalContributions);
        }


        #region Event Handlers 

        private void OnFarmMoved() {
            CalcOutbreaks();
        }

        protected override void OnSimCanvasSubmitted() {
            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Event Handlers
    }
}