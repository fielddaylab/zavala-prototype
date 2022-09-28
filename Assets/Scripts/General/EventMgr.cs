using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zavala.Interact;

namespace Zavala.Events
{
    #region Enums

    public enum ID
    {
        // Money
        ProduceMoney,
        PlayerUpdatedMoney,
        AttemptPurchase,
        PurchaseSuccessful,
        PurchaseFailure,

        // Interact
        InteractModeUpdated
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

    public class PurchaseSuccessfulEventArgs : EventArgs
    {
        public int Amt { get; set; }

        public PurchaseSuccessfulEventArgs(int amt) {
            Amt = amt;
        }
    }

    public class InteractModeEventArgs : EventArgs
    {
        public Interact.Mode Mode { get; set; }

        public InteractModeEventArgs(Interact.Mode mode) {
            Mode = mode;
        }
    }

    #endregion // EventArgs

    public class EventMgr : MonoBehaviour
    {
        public static EventMgr Instance;

        #region Money

        public event EventHandler<ProduceMoneyEventArgs> ProduceMoney;
        public event EventHandler PlayerUpdatedMoney;
        public event EventHandler AttemptPurchase;
        public event EventHandler<PurchaseSuccessfulEventArgs> PurchaseSuccessful;
        public event EventHandler PurchaseFailure;

        #endregion // Money

        #region Interaction

        public event EventHandler<InteractModeEventArgs> InteractModeUpdated;

        #endregion // Interaction

        public void Init() {
            Instance = this;
        }

        public void TriggerEvent(Events.ID id, EventArgs args) {
            switch(id) {
                case Events.ID.ProduceMoney:
                    ProduceMoney?.Invoke(this, (ProduceMoneyEventArgs)args);
                    break;
                case Events.ID.PlayerUpdatedMoney:
                    PlayerUpdatedMoney?.Invoke(this, args);
                    break;
                case Events.ID.AttemptPurchase:
                    AttemptPurchase?.Invoke(this, args);
                    break;
                case Events.ID.PurchaseSuccessful:
                    PurchaseSuccessful?.Invoke(this, (PurchaseSuccessfulEventArgs)args);
                    break;
                case Events.ID.PurchaseFailure:
                    PurchaseFailure?.Invoke(this, args);
                    break;
                case Events.ID.InteractModeUpdated:
                    InteractModeUpdated?.Invoke(this, (InteractModeEventArgs)args);
                    break;
                default:
                    break;
            }
        }
    }
}
