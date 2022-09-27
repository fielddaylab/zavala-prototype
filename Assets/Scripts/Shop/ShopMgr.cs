using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala
{
    public class ShopMgr : MonoBehaviour
    {
        public static ShopMgr Instance;

        [SerializeField] private Text m_moneyText;

        public void Init() {
            Instance = this;

            EventMgr.Instance.PlayerReceivedMoney += HandlePlayerReceivedMoney;
        }

        public bool TryPurchase(ShopItem item) {
            // reduce m_moneyUnits

            UpdateText();

            return true;
        }

        
        private void UpdateText() {
            m_moneyText.text = "" + PlayerMgr.Instance.GetMoney();
        }


        #region Handlers

        private void HandlePlayerReceivedMoney(object sender, EventArgs args) {
            UpdateText();
        }

        #endregion // Handlers
    }
}
