using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Fiscal;
using Zavala.Interact;
using Zavala.Sim;

namespace Zavala
{
    public class FinanceFiscalUI : SimModeUI
    {
        [Header("FinanceFiscalUI")]

        [SerializeField] private FinanceFiscalSlider[] m_sliders;
        
        private static int PRIVATE_SPENDING_INDEX = 0;
        private static int GOVT_SPENDING_INDEX = 1;
        private static int OUTBREAK_INDEX = 2;

        private static float PRIVATE_MOD = 0.6f;
        private static float GOVT_MOD = 0.8f;
        private static float OUTBREAK_MOD = 0.8f;


        private void Awake() {
            base.Awake();

            EventMgr.FiscalSliderChanged?.AddListener(OnFiscalSliderChanged);
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

            foreach (var slider in m_sliders) {
                switch (slider.GetActionState()) {
                    default:
                        break;
                    case -1:
                        if (slider.Type == FiscalType.Private) {
                            EventMgr.RegisterAction.Invoke(SimAction.DecreasePrivatePolicy);
                        }
                        else {
                            EventMgr.RegisterAction.Invoke(SimAction.DecreaseGovernmentPolicy);
                        }
                        break;
                    case 0:
                        break;
                    case 1:
                        if (slider.Type == FiscalType.Private) {
                            EventMgr.RegisterAction.Invoke(SimAction.IncreasePrivatePolicy);
                        }
                        else {
                            EventMgr.RegisterAction.Invoke(SimAction.IncreaseGovernmentPolicy);
                        }
                        break;
                }
            }
        }

        private void InitIndicatorVals() {
            IndicatorMgr.Instance.SetIndicatorValue(PRIVATE_SPENDING_INDEX, 0.5f);
            IndicatorMgr.Instance.SetIndicatorValue(GOVT_SPENDING_INDEX, 0.5f);
            IndicatorMgr.Instance.SetIndicatorValue(OUTBREAK_INDEX, 0.6f);

            foreach (var slider in m_sliders) {
                if (slider.Type == FiscalType.Private) {
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, slider.AggregateDelta * PRIVATE_MOD);
                }
                else {
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, slider.AggregateDelta * GOVT_MOD);
                }
                IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, -slider.AggregateDelta * OUTBREAK_MOD);
            }
        }

        #region Handlers 

        private void OnFiscalSliderChanged(FiscalChange change) {

            switch (change.Type) {
                default:
                    break;
                case FiscalType.Private:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, change.Delta * PRIVATE_MOD);
                    break;
                case FiscalType.Govt:
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, change.Delta * GOVT_MOD);
                    break;
            }
            IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, -change.Delta * OUTBREAK_MOD);
        }

        protected override void OnSimCanvasSubmitted() {
            Close();

            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}
