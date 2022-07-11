using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class AgrPhosphUI : SimModeUI
    {
        //[Header("AgrPhosphUI")]

        [SerializeField] private AgrPhosphInteractable[] m_farms;
        //[SerializeField] private Lake[] m_lakes;

        private void Awake() {
            EventMgr.AgrPhosphFarmExcessAdjusted.AddListener(OnExcessAdjusted);
        }

        public override void Open() {
            InitIndicatorVals();
        }

        private void InitIndicatorVals() {
            float maxContribution = 1f / m_farms.Length;
            float totalOutbreaks = 0;

            foreach (AgrPhosphInteractable farm in m_farms) {
                float farmContribution = farm.UptakeSlider.value * maxContribution;

                totalOutbreaks += farmContribution;
            }

            IndicatorMgr.Instance.SetIndicatorValue(0, totalOutbreaks);
        }

        #region Handlers 

        private void OnExcessAdjusted(float delta) {
            float maxContribution = 1f / m_farms.Length;
            float totalDelta = delta * maxContribution;

            IndicatorMgr.Instance.AdjustIndicatorValue(0, totalDelta);
        }

        #endregion // Handlers
    }
}