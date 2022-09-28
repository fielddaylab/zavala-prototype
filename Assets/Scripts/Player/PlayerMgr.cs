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
            m_moneyUnits = 0;
            AddMoney(m_startingMoney);

            EventMgr.Instance.ProduceMoney += HandleProduceMoney;
        }

        private void AddMoney(int units) {
            m_moneyUnits += units;
            Debug.Log("[PlayerMgr] Added money!");

            EventMgr.Instance.TriggerEvent(Events.ID.PlayerReceivedMoney, EventArgs.Empty);
        }

        public int GetMoney() {
            return m_moneyUnits;
        }

        #region Handlers

        private void HandleProduceMoney(object sender, ProduceMoneyEventArgs args) {
            AddMoney(args.Amt);
        }

        #endregion // Handlers
    }
}