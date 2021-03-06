using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zavala.Interact;
using Zavala.Transport;
using Zavala.Fiscal;
using Zavala.Exchange;
using Zavala.Strategy;
using Zavala.Sim;

namespace Zavala
{
    public class EventMgr : MonoBehaviour
    {
        #region Unlock Events

        public static UnityEvent ModeUnlocked = new UnityEvent();

        #endregion // Unlock Events

        #region Interact Events

        public class InteractEvent : UnityEvent<InteractMode> { }
        public static InteractEvent InteractModeUpdated = new InteractEvent();

        #endregion // Interact Events

        #region Sim Events

        public class SimEvent : UnityEvent<SimModeData> { }
        public static SimEvent SetNewMode = new SimEvent();

        #endregion // Sim Events

        #region SimAction Events

        public class SimActionEvent : UnityEvent<SimAction> { }
        public static SimActionEvent RegisterAction = new SimActionEvent();
        public static SimActionEvent RemoveAction = new SimActionEvent();

        public static UnityEvent SimStageActions = new UnityEvent();

        public class SimPostActionEvent : UnityEvent<List<SimAction>> { }
        public static SimPostActionEvent SimPostActions = new SimPostActionEvent();

        #endregion // Sim Action Events

        #region Indicator Events

        public static UnityEvent IndicatorUpdated = new UnityEvent();

        public class IndicatorEvent : UnityEvent<IndicatorData[]> { }
        public static IndicatorEvent SetNewIndicators = new IndicatorEvent();

        #endregion // Indicator Events

        #region Canvas Events

        public static UnityEvent SimCanvasSubmitted = new UnityEvent();
        public static UnityEvent ResultsCanvasOkayed = new UnityEvent();

        #endregion // Canvas Events

        #region AgrFarm Events

        public static UnityEvent FarmMoved = new UnityEvent();

        #endregion // AgrFarm Events

        #region AgrPhosph Events

        public class AgrPhosphEvent : UnityEvent<float> { }

        public static AgrPhosphEvent AgrPhosphFarmExcessAdjusted = new AgrPhosphEvent();

        #endregion // AgrPhosph Events

        #region AgrUptake Events

        public class AgrUptakeEvent : UnityEvent<float> { }

        public static AgrUptakeEvent AgrUptakeFarmExcessAdjusted = new AgrUptakeEvent();
        public static AgrUptakeEvent AgrUptakeSinkAmtAdjusted = new AgrUptakeEvent();

        public static UnityEvent AgrUptakeStorageAdded = new UnityEvent();
        public static UnityEvent AgrUptakeStorageRemoved = new UnityEvent();

        #endregion // AgrUptake Events

        #region TransportBuilding Events

        public class TransportBuildingEvent : UnityEvent<BuildDetails> { }

        public static TransportBuildingEvent StructureBuilt = new TransportBuildingEvent();
        public static TransportBuildingEvent StructureRemoved = new TransportBuildingEvent();

        #endregion // TransportBuilding Events

        #region FinanceFiscal Events

        public class FinanceFiscalEvent : UnityEvent<FiscalChange> { }

        public static FinanceFiscalEvent FiscalSliderChanged = new FinanceFiscalEvent();

        #endregion // FinanceFiscal Events

        #region FinanceExchange Events

        public class FinanceExchangeEvent : UnityEvent<ExchangeDetails> { }

        public static FinanceExchangeEvent ExchangeBuilt = new FinanceExchangeEvent();
        public static FinanceExchangeEvent ExchangeRemoved = new FinanceExchangeEvent();

        #endregion // FinanceExchange Events

        #region PolicyCampaign Events

        public class PolicyCampaignEvent : UnityEvent<StratDetails> { }

        public static PolicyCampaignEvent StratDeployed = new PolicyCampaignEvent();
        public static PolicyCampaignEvent StratRemoved = new PolicyCampaignEvent();

        #endregion // FinanceExchange Events

        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}