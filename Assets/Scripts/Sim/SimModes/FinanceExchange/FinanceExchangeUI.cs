using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;

namespace Zavala.Exchange
{
    public class FinanceExchangeUI : SimModeUI
    {
        private static int PRIVATE_SPENDING_INDEX = 0;
        private static int GOVT_SPENDING_INDEX = 1;
        private static int OUTBREAK_INDEX = 2;
        private static int JOBS_INDEX = 3;

        private static float EXPENSE_MOD = 0.0025f;
        private static float OUTBREAK_MOD = 0.0075f;
        private static float JOBS_MOD = 0.01f;

        private void Awake() {
            base.Awake();

            EventMgr.ExchangeBuilt?.AddListener(OnExchangeBuilt);
            EventMgr.ExchangeRemoved?.AddListener(OnExchangeRemoved);
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
            IndicatorMgr.Instance.SetIndicatorValue(JOBS_INDEX, 0);
        }

        private void PayForExchange(ExchangeType type, float indicatorExpense) {
            switch (type) {
                default:
                    break;
                case ExchangeType.Basic:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
            }
        }

        private void ReimburseForExchange(ExchangeType type, float indicatorExpense) {
            switch (type) {
                default:
                    break;
                case ExchangeType.Basic:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    break;
            }
        }

        private void ModifyOutbreak(ExchangeType type, float indicatorAmt) {
            switch (type) {
                default:
                    break;
                case ExchangeType.Basic:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(OUTBREAK_INDEX, indicatorAmt);
                    break;
            }
        }

        private void ModifyJobs(ExchangeType type, float indicatorAmt) {
            switch (type) {
                default:
                    break;
                case ExchangeType.Basic:
                    IndicatorMgr.Instance.AdjustIndicatorValue(JOBS_INDEX, indicatorAmt);
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(JOBS_INDEX, indicatorAmt);
                    break;
            }
        }

        #region Handlers

        private void OnExchangeBuilt(ExchangeDetails details) {
            float indicatorExpense = details.Cost * EXPENSE_MOD;

            PayForExchange(details.Type, indicatorExpense);

            float outbreakAmt = details.Cost * OUTBREAK_MOD;

            ModifyOutbreak(details.Type, -outbreakAmt);

            float jobsAmt = details.Jobs * JOBS_MOD;

            ModifyJobs(details.Type, jobsAmt);
        }

        private void OnExchangeRemoved(ExchangeDetails details) {
            float indicatorExpense = -details.Cost * EXPENSE_MOD;

            ReimburseForExchange(details.Type, indicatorExpense);

            float outbreakAmt = details.Cost * OUTBREAK_MOD;

            ModifyOutbreak(details.Type, outbreakAmt);

            float jobsAmt = details.Jobs * JOBS_MOD;

            ModifyJobs(details.Type, -jobsAmt);
        }

        protected override void OnSimCanvasSubmitted() {
            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}