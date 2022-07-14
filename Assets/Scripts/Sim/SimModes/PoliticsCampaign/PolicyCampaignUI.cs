using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;
using Zavala.Strategy;

namespace Zavala
{
    public class PolicyCampaignUI : SimModeUI
    {
        private static float SUPPORT_MOD = 0.15f;

        private void Awake() {
            base.Awake();

            EventMgr.StratDeployed?.AddListener(OnStratDeployed);
            EventMgr.StratRemoved?.AddListener(OnStratRemoved);
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
            IndicatorMgr.Instance.SetIndicatorValue(0, 0.2f);
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
                    break;
                case StratType.Video:
                    //if (stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    stratDetails.District.AddSupport();
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
                    break;
                case StratType.Video:
                    stratDetails.District.RemoveSupport();
                    //if (!stratDetails.District.IsSupporting()) {
                    indicatorAdjustment = -stratDetails.District.SupportYield * SUPPORT_MOD;
                    IndicatorMgr.Instance.AdjustIndicatorValue(0, indicatorAdjustment);
                    break;
            }
        }

        protected override void OnSimCanvasSubmitted() {
            EventMgr.SimStageActions?.Invoke();
        }

        #endregion // Handlers
    }

}