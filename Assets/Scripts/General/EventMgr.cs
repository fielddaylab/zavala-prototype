using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Events
{
    #region Enums

    public enum ID
    {
        // Money
        ProduceMoney,
        PlayerReceivedMoney,
        AttemptPurchase,
        PurchaseSuccessful,
        PurchaseFailure

        // Other
    }

    #endregion // Enums

    #region EventArgs

    public class ProduceMoneyEventArgs : EventArgs
    {
        public int Amt { get; set; }

        public ProduceMoneyEventArgs(int amt) {
            Amt = amt;
        }
    }

    #endregion // EventArgs

    public class EventMgr : MonoBehaviour
    {
        public static EventMgr Instance;

        #region Money

        public event EventHandler<ProduceMoneyEventArgs> ProduceMoney;
        public event EventHandler PlayerReceivedMoney;
        public event EventHandler AttemptPurchase;
        public event EventHandler PurchaseSuccessful;
        public event EventHandler PurchaseFailure;

        #endregion // Money

        public void Init() {
            Instance = this;
        }

        public void TriggerEvent(Events.ID id, EventArgs args) {
            switch(id) {
                case Events.ID.ProduceMoney:
                    ProduceMoney?.Invoke(this, (ProduceMoneyEventArgs)args);
                    break;
                case Events.ID.PlayerReceivedMoney:
                    PlayerReceivedMoney?.Invoke(this, args);
                    break;
                case Events.ID.AttemptPurchase:
                    AttemptPurchase?.Invoke(this, args);
                    break;
                case Events.ID.PurchaseSuccessful:
                    PurchaseSuccessful?.Invoke(this, args);
                    break;
                case Events.ID.PurchaseFailure:
                    PurchaseFailure?.Invoke(this, args);
                    break;
                default:
                    break;
            }
        }
    }
}
