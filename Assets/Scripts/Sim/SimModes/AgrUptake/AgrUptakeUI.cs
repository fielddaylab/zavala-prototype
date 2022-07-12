using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;

namespace Zavala
{
    public class AgrUptakeUI : SimModeUI
    {
        [Header("AgrUptakeUI")]

        [SerializeField] private AgrUptakeFarm[] m_farms;
        [SerializeField] private AgrUptakeSink[] m_sinks;

        private void Awake() {
            EventMgr.AgrUptakeFarmExcessAdjusted.AddListener(OnExcessAdjusted);
            EventMgr.AgrUptakeSinkAmtAdjusted.AddListener(OnSinkAmtAdjusted);

            EventMgr.AgrUptakeStorageAdded.AddListener(OnStorageAdded);
            EventMgr.AgrUptakeStorageRemoved.AddListener(OnStorageRemoved);
        }

        public override void Open() {
            InitIndicatorVals();

            EventMgr.InteractModeUpdated?.Invoke(InteractMode.Default);
        }

        private void InitIndicatorVals() {
            // Algal Outbreaks

            float maxContribution = 1f / m_farms.Length / 2f;
            float totalOutbreaks = 0;

            foreach (AgrUptakeFarm farm in m_farms) {
                float farmContribution = farm.UptakeSlider.value * maxContribution;

                totalOutbreaks += farmContribution;
            }

            IndicatorMgr.Instance.SetIndicatorValue(1, totalOutbreaks);

            // Excess Phosphorous

            // TODO: set this to a reasonable starting value
            IndicatorMgr.Instance.SetIndicatorValue(0, totalOutbreaks);
        }

        #region Handlers 

        private void OnExcessAdjusted(float delta) {
            float maxContribution = 1f / m_farms.Length / 2f;
            float totalDelta = delta * maxContribution;

            // adjust excess phosphorous inversely
            IndicatorMgr.Instance.AdjustIndicatorValue(0, -totalDelta);

            // decrease algal outbreaks near farm
            IndicatorMgr.Instance.AdjustIndicatorValue(1, totalDelta);
        }

        private void OnSinkAmtAdjusted(float delta) {
            float maxContribution = 1f / m_sinks.Length / 2f;
            float totalDelta = delta * maxContribution;

            // adjust excess phosphorous inversely
            IndicatorMgr.Instance.AdjustIndicatorValue(0, totalDelta);
        }

        private void OnStorageAdded() {
            float maxContribution = 1f / m_farms.Length / 3f;
            float totalDelta = maxContribution;

            // adjust excess phosphorous inversely
            IndicatorMgr.Instance.AdjustIndicatorValue(0, -totalDelta);
        }

        private void OnStorageRemoved() {
            float maxContribution = 1f / m_farms.Length / 3f;
            float totalDelta = maxContribution;

            // adjust excess phosphorous inversely
            IndicatorMgr.Instance.AdjustIndicatorValue(0, totalDelta);
        }

        #endregion // Handlers
    }
}