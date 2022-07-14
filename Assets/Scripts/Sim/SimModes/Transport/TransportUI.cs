using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Transport;

namespace Zavala
{
    public class TransportUI : SimModeUI
    {

        private static int PRIVATE_SPENDING_INDEX = 0;
        private static int GOVT_SPENDING_INDEX = 1;
        private static int OUTBREAK_INDEX = 2;

        private static float EXPENSE_MOD = 0.01f;
        private static float OUTBREAK_MOD = 0.005f;

        private void Awake() {
            base.Awake();

            EventMgr.StructureBuilt?.AddListener(OnStructureBuilt);
            EventMgr.StructureRemoved?.AddListener(OnStructureRemoved);
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
            IndicatorMgr.Instance.SetIndicatorValue(PRIVATE_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(GOVT_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(OUTBREAK_INDEX, 0.9f);
        }

        private void PayForStructure(BuildType type, float indicatorExpense) {
            switch (type) {
                default:
                    break;
                case BuildType.Rail:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Highway:
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Road:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Bridge:
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
            }
        }

        private void ReimburseForStructure(BuildType type, float indicatorExpense) {
            switch (type) {
                default:
                    break;
                case BuildType.Rail:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Highway:
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Road:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    break;
                case BuildType.Bridge:
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
            }
        }

        private void ModifyOutbreak(BuildType type, float indicatorAmt) {
            switch (type) {
                default:
                    break;
                case BuildType.Rail:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
                case BuildType.Highway:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
                case BuildType.Road:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
                case BuildType.Bridge:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
            }
        }

        #region Handlers

        private void OnStructureBuilt(BuildDetails details) {
            float indicatorExpense = details.Cost * EXPENSE_MOD;

            PayForStructure(details.Type, indicatorExpense);

            float outbreakAmt = details.Cost * OUTBREAK_MOD;

            if (details.Reduces) {
                ModifyOutbreak(details.Type, -outbreakAmt);
            }
        }

        private void OnStructureRemoved(BuildDetails details) {
            float indicatorExpense = -details.Cost * EXPENSE_MOD;

            ReimburseForStructure(details.Type, indicatorExpense);

            float outbreakAmt = details.Cost * OUTBREAK_MOD;

            if (details.Reduces) {
                ModifyOutbreak(details.Type, outbreakAmt);
            }
        }

        protected override void OnSimCanvasSubmitted() {
            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}