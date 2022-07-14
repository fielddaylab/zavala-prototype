using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Sim;
using Zavala.Strategy;

namespace Zavala
{
    public class PolicyCampaignUI : SimModeUI
    {
        private static float SUPPORT_MOD = 0.15f;

        private int m_numStops, m_numVideos;

        private void Awake() {
            base.Awake();

            EventMgr.StratDeployed?.AddListener(OnStratDeployed);
            EventMgr.StratRemoved?.AddListener(OnStratRemoved);

            m_numStops = m_numVideos = 0;
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

            for (int i = 0; i < m_numStops; i++) {
                EventMgr.RegisterAction.Invoke(SimAction.DistrictStop);
            }
            for (int i = 0; i < m_numVideos; i++) {
                EventMgr.RegisterAction.Invoke(SimAction.DistrictAd);
            }
        }

        private void InitIndicatorVals() {
            IndicatorMgr.Instance.SetIndicatorValue(0, 0.2f + (m_numStops + m_numVideos) * 0.5f * SUPPORT_MOD);
        }

        #region Handlers 

        private void OnStratDeployed(StratDetails stratDetails) {
            if (stratDetails.District == null) { return; }

            float indicatorAdjustment;

            switch (stratDetails.Type) {
                default:
                    break;
                case StratType.Stop:
                    //if (stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    stratDetails.District.AddSupport();
                    m_numStops++;
                    break;
                case StratType.Video:
                    //if (stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    stratDetails.District.AddSupport();
                    m_numVideos++;
                    break;
            }
        }

        private void OnStratRemoved(StratDetails stratDetails) {
            if (stratDetails.District == null) { return; }

            float indicatorAdjustment;

            switch (stratDetails.Type) {
                default:
                    break;
                case StratType.Stop:
                    stratDetails.District.RemoveSupport();
                    //if (!stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = -stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    m_numStops--;
                    break;
                case StratType.Video:
                    stratDetails.District.RemoveSupport();
                    //if (!stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = -stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    m_numVideos--;
                    break;
            }
        }

        protected override void OnSimCanvasSubmitted() {
            Close();

            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }

}