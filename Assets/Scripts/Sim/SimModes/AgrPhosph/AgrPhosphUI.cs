using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Sim;

namespace Zavala
{
    public class AgrPhosphUI : SimModeUI
    {
        [Header("AgrPhosphUI")]

        [SerializeField] private AgrPhosphInteractable[] m_farms;

        private void Awake() {
            base.Awake();

            EventMgr.AgrPhosphFarmExcessAdjusted.AddListener(OnExcessAdjusted);
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

        protected override void OnSimCanvasSubmitted() {
            Close();

            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}