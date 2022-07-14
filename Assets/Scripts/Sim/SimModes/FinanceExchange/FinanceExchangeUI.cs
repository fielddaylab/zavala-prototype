using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Sim;

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

        private static float EXCHANGE_BASIC_COST = 20;
        private static float EXCHANGE_DIGEST_COST = 30;
        private static float EXCHANGE_BASIC_JOBS = 10;
        private static float EXCHANGE_DIGEST_JOBS = 12;

        private int m_numBasicExchanges, m_numDigesterExchanges = 0;

        private void Awake() {
            base.Awake();

            EventMgr.ExchangeBuilt?.AddListener(OnExchangeBuilt);
            EventMgr.ExchangeRemoved?.AddListener(OnExchangeRemoved);

            m_numBasicExchanges = m_numDigesterExchanges = 0;
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

            for (int i = 0; i < m_numBasicExchanges; i++) {
                EventMgr.RegisterAction.Invoke(SimAction.BuildBasicExchange);
            }
            for (int i = 0; i < m_numDigesterExchanges; i++) {
                EventMgr.RegisterAction.Invoke(SimAction.BuildDigesterExchange);
            }
        }

        private void InitIndicatorVals() {
            IndicatorMgr.Instance.SetIndicatorValue(PRIVATE_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(GOVT_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(OUTBREAK_INDEX, 0.9f);
            IndicatorMgr.Instance.SetIndicatorValue(JOBS_INDEX, 0);

            ExchangeDetails defaultBasicDetails = new ExchangeDetails(EXCHANGE_BASIC_COST, ExchangeType.Basic, EXCHANGE_BASIC_JOBS);
            ExchangeDetails defaultDigesterDetails = new ExchangeDetails(EXCHANGE_DIGEST_COST, ExchangeType.Digester, EXCHANGE_DIGEST_JOBS);

            for (int i = 0; i < m_numBasicExchanges; i++) {
                OnExchangeBuilt(defaultBasicDetails);
                m_numBasicExchanges--;
            }
            for (int i = 0; i < m_numDigesterExchanges; i++) {
                OnExchangeBuilt(defaultDigesterDetails);
                m_numDigesterExchanges--;
            }
        }

        private void PayForExchange(ExchangeType type, float indicatorExpense) {
            switch (type) {
                default:
                    break;
                case ExchangeType.Basic:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    m_numBasicExchanges++;
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    m_numDigesterExchanges++;
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
                    m_numBasicExchanges--;
                    break;
                case ExchangeType.Digester:
                    IndicatorMgr.Instance.AdjustIndicatorValue(PRIVATE_SPENDING_INDEX, indicatorExpense);
                    IndicatorMgr.Instance.AdjustIndicatorValue(GOVT_SPENDING_INDEX, indicatorExpense);
                    m_numDigesterExchanges--;
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
            Close();

            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }
}