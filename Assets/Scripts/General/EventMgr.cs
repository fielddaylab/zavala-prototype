using System;
using UnityEngine;
using Zavala.Settings;

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
        InteractModeUpdated,
        InspectableOpened,

        // Lenses
        LensModeUpdated,

        // Narrative
        NarrativeBlurbTriggered,
        NarrativeBlurbClosed,

        // Settings
        AllVarsUpdated
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

    public class LensModeEventArgs : EventArgs
    {
        public Lenses.Mode Mode { get; set; }

        public LensModeEventArgs(Lenses.Mode mode) {
            Mode = mode;
        }
    }

    public class NarrativeBlurbEventArgs : EventArgs
    {
        public string BlurbText;

        public NarrativeBlurbEventArgs(string text) {
            BlurbText = text;
        }
    }

    public class AllVarsEventArgs : EventArgs
    {
        public AllVars UpdatedVars;

        public AllVarsEventArgs(AllVars updatedVars) {
            UpdatedVars = updatedVars;
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
        public event EventHandler InspectableOpened;

        #endregion // Interaction

        #region Narrative 

        public event EventHandler<NarrativeBlurbEventArgs> NarrativeBlurbTriggered;
        public event EventHandler NarrativeBlurbClosed;

        #endregion // Narrative

        #region Settings

        public event EventHandler<AllVarsEventArgs> AllVarsUpdated;

        #endregion // Settings

        #region Lenses

        public event EventHandler<LensModeEventArgs> LensModeUpdated;

        #endregion // Lenses

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
                case Events.ID.InspectableOpened:
                    InspectableOpened?.Invoke(this, args);
                    break;
                case Events.ID.LensModeUpdated:
                    LensModeUpdated?.Invoke(this, (LensModeEventArgs)args);
                    break;
                case Events.ID.NarrativeBlurbTriggered:
                    NarrativeBlurbTriggered?.Invoke(this, (NarrativeBlurbEventArgs)args);
                    break;
                case Events.ID.NarrativeBlurbClosed:
                    NarrativeBlurbClosed?.Invoke(this, args);
                    break;
                case Events.ID.AllVarsUpdated:
                    AllVarsUpdated?.Invoke(this, (AllVarsEventArgs)args);
                    break;
                default:
                    break;
            }
        }
    }
}
