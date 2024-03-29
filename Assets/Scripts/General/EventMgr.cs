using System;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Advisors;
using Zavala.Cards;
using Zavala.Settings;

namespace Zavala.Events
{
    #region Enums

    public enum ID
    {
        // Money
        ProduceMoney,
        RegionUpdatedMoney,
        AttemptPurchase,
        PurchaseSuccessful,
        PurchaseFailure,

        // Economy
        EconomyUpdated,

        // Interact
        InteractModeUpdated,
        InspectableOpened,

        // Lenses
        LensModeUpdated,

        // Narrative
        NarrativeBlurbTriggered,
        NarrativeBlurbClosed,

        // Settings
        AllVarsUpdated,

        // Restart
        LevelRestarted,

        // Regions
        RegionToggled,
        RegionSwitched,
        CameraMoved,
        RegionActivationCompleted,
        PanToRegion,

        // Phosph
        PipsGenerated,
        PipMovementCompleted,

        // Advisors
        ChoiceSlotUpdated,
        AdvisorBlurb,
        ChoiceUnlock,
        ViewNewPolicies,
        AdvisorHidden,
        AdvisorShown,
        AdvisorSelected,
        AdvisorSelectToggle,
        AdvisorButtonClicked,
        AdvisorNoReplacement
    }

    #endregion // Enums

    #region EventArgs

    public class ProduceMoneyEventArgs : EventArgs
    {
        public int Amt { get; set; }
        public LevelRegion Region;

        public ProduceMoneyEventArgs(int amt, LevelRegion region) {
            Amt = amt;
            Region = region;
        }
    }

    public class RegionUpdatedMoneyEventArgs : EventArgs
    {
        public LevelRegion Region;

        public RegionUpdatedMoneyEventArgs(LevelRegion region) {
            Region = region;
        }
    }

    public class PurchaseSuccessfulEventArgs : EventArgs
    {
        public int Amt { get; set; }
        public LevelRegion Region;

        public PurchaseSuccessfulEventArgs(int amt, LevelRegion region) {
            Amt = amt;
            Region = region;
        }
    }

    public class EconomyUpdatedEventArgs : EventArgs
    {
        public LevelRegion Region;

        public EconomyUpdatedEventArgs(LevelRegion region) {
            Region = region;
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

    public class RegionToggleEventArgs : EventArgs {
        public int RegionNum;

        public RegionToggleEventArgs(int regionNum) {
            RegionNum = regionNum;
        }
    }

    public class RegionSwitchedEventArgs : EventArgs
    {
        public LevelRegion NewRegion;

        public RegionSwitchedEventArgs(LevelRegion newRegion) {
            NewRegion = newRegion;
        }
    }

    public class PipsGeneratedEventArgs : EventArgs
    {
        public LevelRegion Region;
        public int Quantity;

        public PipsGeneratedEventArgs(LevelRegion region, int quantity) {
            Region = region;
            Quantity = quantity;
        }
    }

    public class ChoiceSlotEventArgs : EventArgs
    {
        public SlotCard Card;
        public int RegionNum;
        public bool IsGlobal;

        public ChoiceSlotEventArgs(SlotCard card, int regionNum, bool isGlobal) {
            Card = card;
            RegionNum = regionNum;
            IsGlobal = isGlobal;
        }
    }

    public class AdvisorBlurbEventArgs : EventArgs
    {
        public string Text;
        public AdvisorID AdvisorID;
        public bool IsSilent;

        public AdvisorBlurbEventArgs(string text, AdvisorID advisorID, bool isSilent = false) {
            Text = text;
            AdvisorID = advisorID;
            IsSilent = isSilent;
        }
    }

    public class ChoiceUnlockEventArgs : EventArgs
    {
        public string Text;
        public AdvisorID AdvisorID;
        public List<string> ToUnlock;

        public ChoiceUnlockEventArgs(string text, AdvisorID advisorID, List<string> toUnlock) {
            Text = text;
            AdvisorID = advisorID;
            ToUnlock = toUnlock;
        }
    }

    public class AdvisorEventArgs : EventArgs
    {
        public AdvisorID AdvisorID;

        public AdvisorEventArgs(AdvisorID advisorID) {
            AdvisorID = advisorID;
        }
    }


    #endregion // EventArgs

    public class EventMgr : MonoBehaviour
    {
        public static EventMgr Instance;

        #region Money

        public event EventHandler<ProduceMoneyEventArgs> ProduceMoney;
        public event EventHandler<RegionUpdatedMoneyEventArgs> RegionUpdatedMoney;
        public event EventHandler AttemptPurchase;
        public event EventHandler<PurchaseSuccessfulEventArgs> PurchaseSuccessful;
        public event EventHandler PurchaseFailure;

        #endregion // Money

        #region Economy

        public event EventHandler<EconomyUpdatedEventArgs> EconomyUpdated;

        #endregion // Economy

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

        #region Restart 

        public event EventHandler LevelRestarted;

        #endregion // Restart

        #region Region

        public event EventHandler<RegionToggleEventArgs> RegionToggled;
        public event EventHandler<RegionSwitchedEventArgs> RegionSwitched;
        public event EventHandler<EventArgs> RegionActivationCompleted;
        public event EventHandler<EventArgs> CameraMoved;
        public event EventHandler<RegionSwitchedEventArgs> PanToRegion;

        #endregion // Region

        #region Phosph 

        public event EventHandler<PipsGeneratedEventArgs> PipsGenerated;
        public event EventHandler PipMovementCompleted;

        #endregion // Phosph

        #region Lenses

        public event EventHandler<LensModeEventArgs> LensModeUpdated;

        #endregion // Lenses


        #region Advisor

        public event EventHandler<ChoiceSlotEventArgs> ChoiceSlotUpdated;
        public event EventHandler<AdvisorBlurbEventArgs> AdvisorBlurb;
        public event EventHandler<ChoiceUnlockEventArgs> ChoiceUnlock;
        public event EventHandler<ChoiceUnlockEventArgs> ViewNewPolicies;
        public event EventHandler<AdvisorEventArgs> AdvisorHidden;
        public event EventHandler<AdvisorEventArgs> AdvisorShown; // with policies
        public event EventHandler<AdvisorEventArgs> AdvisorSelected; // only lens change, forces selection
        public event EventHandler<AdvisorEventArgs> AdvisorSelectToggle; // only lens change, toggles selection
        public event EventHandler<AdvisorEventArgs> AdvisorButtonClicked;
        public event EventHandler AdvisorsNoReplacement;

        #endregion // Advisor


        public void Init() {
            Instance = this;
        }

        public void TriggerEvent(Events.ID id, EventArgs args) {
            switch(id) {
                case Events.ID.ProduceMoney:
                    ProduceMoney?.Invoke(this, (ProduceMoneyEventArgs)args);
                    break;
                case Events.ID.RegionUpdatedMoney:
                    RegionUpdatedMoney?.Invoke(this, (RegionUpdatedMoneyEventArgs)args);
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
                case Events.ID.EconomyUpdated:
                    EconomyUpdated?.Invoke(this, (EconomyUpdatedEventArgs)args);
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
                case Events.ID.LevelRestarted:
                    LevelRestarted?.Invoke(this, args);
                    break;
                case Events.ID.RegionToggled:
                    RegionToggled?.Invoke(this, (RegionToggleEventArgs)args);
                    break;
                case Events.ID.RegionSwitched:
                    RegionSwitched?.Invoke(this, (RegionSwitchedEventArgs)args);
                    break;
                case Events.ID.RegionActivationCompleted:
                    RegionActivationCompleted?.Invoke(this, args);
                    break;
                case Events.ID.CameraMoved:
                    CameraMoved?.Invoke(this, args);
                    break;
                case Events.ID.PanToRegion:
                    PanToRegion?.Invoke(this, (RegionSwitchedEventArgs)args);
                    break;
                case Events.ID.PipsGenerated:
                    PipsGenerated?.Invoke(this, (PipsGeneratedEventArgs)args);
                    break;
                case Events.ID.PipMovementCompleted:
                    PipMovementCompleted?.Invoke(this, args);
                    break;
                case Events.ID.ChoiceSlotUpdated:
                    ChoiceSlotUpdated?.Invoke(this, (ChoiceSlotEventArgs)args);
                    break;
                case Events.ID.AdvisorBlurb:
                    AdvisorBlurb?.Invoke(this, (AdvisorBlurbEventArgs)args);
                    break;
                case Events.ID.ChoiceUnlock:
                    ChoiceUnlock?.Invoke(this, (ChoiceUnlockEventArgs)args);
                    break;
                case Events.ID.ViewNewPolicies:
                    ViewNewPolicies?.Invoke(this, (ChoiceUnlockEventArgs)args);
                    break;
                case Events.ID.AdvisorHidden:
                    AdvisorHidden?.Invoke(this, (AdvisorEventArgs)args);
                    break;
                case Events.ID.AdvisorShown:
                    AdvisorShown?.Invoke(this, (AdvisorEventArgs)args);
                    break;
                case Events.ID.AdvisorSelected:
                    AdvisorSelected?.Invoke(this, (AdvisorEventArgs)args);
                    break;
                case Events.ID.AdvisorSelectToggle:
                    AdvisorSelectToggle?.Invoke(this, (AdvisorEventArgs)args);
                    break;
                case Events.ID.AdvisorButtonClicked:
                    AdvisorButtonClicked?.Invoke(this, (AdvisorEventArgs)args);
                    break;
                case Events.ID.AdvisorNoReplacement:
                    AdvisorsNoReplacement?.Invoke(this, args);
                    break;
                default:
                    break;
            }
        }
    }
}
