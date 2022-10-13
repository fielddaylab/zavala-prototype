using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;

namespace Zavala
{
    public class PlayerMgr : MonoBehaviour
    {
        public static PlayerMgr Instance;

        [SerializeField] private int m_startingMoney;
        private int m_moneyUnits;

        public void Init() {
            Instance = this;

            ResetMoney();

            EventMgr.Instance.ProduceMoney += HandleProduceMoney;
            EventMgr.Instance.PurchaseSuccessful += HandlePurchaseSuccessful;
            EventMgr.Instance.LevelRestarted += HandleLevelRestarted;
        }

        private void ResetMoney() {
            m_moneyUnits = 0;
            AddMoney(m_startingMoney);
        }

        private void AddMoney(int units) {
            m_moneyUnits += units;
            Debug.Log("[PlayerMgr] Added money!");

            EventMgr.Instance.TriggerEvent(Events.ID.PlayerUpdatedMoney, EventArgs.Empty);
        }

        private void SpendMoney(int units) {
            m_moneyUnits -= units;

            EventMgr.Instance.TriggerEvent(Events.ID.PlayerUpdatedMoney, EventArgs.Empty);
        }


        // i.e. return money
        public int GetMoney() {
            return m_moneyUnits;
        }

        #region Handlers

        private void HandleProduceMoney(object sender, ProduceMoneyEventArgs args) {
            AddMoney(args.Amt);
        }

        private void HandlePurchaseSuccessful(object sender, PurchaseSuccessfulEventArgs args) {
            SpendMoney(args.Amt);
        }
        
        private void HandleLevelRestarted(object sender, EventArgs args) {
            ResetMoney();
        }

        #endregion // Handlers
    }
}