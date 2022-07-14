using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Fiscal;
using Zavala.Interact;

namespace Zavala
{
    public class FinanceFiscalUI : SimModeUI
    {
        //[Header("FinanceFiscalUI")]
        
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

        private void InitIndicatorVals() {
            IndicatorMgr.Instance.SetIndicatorValue(PRIVATE_SPENDING_INDEX, 0.5f);
            IndicatorMgr.Instance.SetIndicatorValue(GOVT_SPENDING_INDEX, 0.5f);
            IndicatorMgr.Instance.SetIndicatorValue(OUTBREAK_INDEX, 0.6f);
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
            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}
