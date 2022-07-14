using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Sim;

namespace Zavala
{
    public class AgrUptakeUI : SimModeUI
    {
        [Header("AgrUptakeUI")]

        [SerializeField] private AgrUptakeFarm[] m_farms;
        [SerializeField] private AgrUptakeSink[] m_sinks;

        private int m_numStorage;

        private void Awake() {
            base.Awake();

            EventMgr.AgrUptakeFarmExcessAdjusted.AddListener(OnExcessAdjusted);
            EventMgr.AgrUptakeSinkAmtAdjusted.AddListener(OnSinkAmtAdjusted);

            EventMgr.AgrUptakeStorageAdded.AddListener(OnStorageAdded);
            EventMgr.AgrUptakeStorageRemoved.AddListener(OnStorageRemoved);

            m_numStorage = 0;
        }

        private void OnEnable() {
            base.OnEnable();
        }

        private void OnDisable() {
            base.OnDisable();
        }

        public override void Open() {
            InitIndicatorVals();

            EventMgr.InteractModeUpdated?.Invoke(InteractMode.Default);
        }

        public override void Close() {
            if (m_completedActions == null) { return; }
            
            m_completedActions.Clear();

            for (int i = 0; i < m_numStorage; i++) {
                EventMgr.RegisterAction.Invoke(SimAction.BuildStorage);
            }

            foreach (var farm in m_farms) {
                switch (farm.GetActionState()) {
                    default:
                        break;
                    case -1:
                        EventMgr.RegisterAction.Invoke(SimAction.LowerPhosphorousOutput);
                        break;
                    case 0:
                        break;
                    case 1:
                        EventMgr.RegisterAction.Invoke(SimAction.RaisePhosphorousOutput);
                        break;
                }
            }

            foreach (var sink in m_sinks) {
                switch (sink.GetActionState()) {
                    default:
                        break;
                    case -1:
                        // all start at 0, so can only increase
                        //EventMgr.RegisterAction.Invoke(SimAction.IncreaseUptake);
                        break;
                    case 0:
                        break;
                    case 1:
                        EventMgr.RegisterAction.Invoke(SimAction.IncreaseUptake);
                        break;
                }
            }
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

            m_numStorage++;
        }

        private void OnStorageRemoved() {
            float maxContribution = 1f / m_farms.Length / 3f;
            float totalDelta = maxContribution;

            // adjust excess phosphorous inversely
            IndicatorMgr.Instance.AdjustIndicatorValue(0, totalDelta);

            m_numStorage--;
        }

        protected override void OnSimCanvasSubmitted() {
            Close();

            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}