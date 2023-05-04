using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Lenses;

namespace Zavala
{
    public class UIEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform m_rootTransform;
        [SerializeField] private CanvasGroup m_group;

        [SerializeField] private Image m_bg;
        [SerializeField] private Button m_button;
        [SerializeField] private Image m_banner;
        [SerializeField] private TMP_Text m_bannerText;

        private SimEventType m_eventType;

        private TriggersEvents m_parentTriggers;

        public void Init(SimEventType type, TriggersEvents parentTriggers) {
            Sprite eventSprite = null;
            m_parentTriggers = parentTriggers;
            switch (type) {
                // ECOLOGICAL
                case SimEventType.ExcessRunoff:
                    eventSprite = GameDB.Instance.UIEventEcologyIcon;
                    m_banner.color = GameDB.Instance.UIEventEcologyColor;
                    m_bannerText.text = "Excessive\nRunoff";
                    if (LensMgr.Instance.GetLensMode() != Mode.Phosphorus) {
                        HideUI();
                    }
                    break;
                case SimEventType.Skimmers:
                    eventSprite = GameDB.Instance.UIEventEcologyIcon;
                    m_banner.color = GameDB.Instance.UIEventEcologyColor;
                    m_bannerText.text = "Gathering\nBloom";
                    if (LensMgr.Instance.GetLensMode() != Mode.Phosphorus) {
                        HideUI();
                    }
                    break;
                case SimEventType.HaltOperations:
                    eventSprite = GameDB.Instance.UIEventEcologyIcon;
                    m_banner.color = GameDB.Instance.UIEventEcologyColor;
                    m_bannerText.text = "Halt\nNeeded";
                    if (LensMgr.Instance.GetLensMode() != Mode.Phosphorus) {
                        HideUI();
                    }
                    break;

                // ECONOMIC
                case SimEventType.PopDecline:
                    eventSprite = GameDB.Instance.UIEventEconomicIcon;
                    m_banner.color = GameDB.Instance.UIEventEconomicColor;
                    m_bannerText.text = "Population\nDecline";
                    if (LensMgr.Instance.GetLensMode() != Mode.Economic) {
                        HideUI();
                    }
                    break;
                // Import Tax happens auto
                // Export Tax happens auto
                default:
                    break;
            }

            // open/close banner
            LevelRegion thisRegion = RegionMgr.Instance.GetRegionByPos(new Vector3(this.transform.position.x, 0, this.transform.position.z));

            m_banner.enabled = m_bannerText.enabled = (thisRegion == RegionMgr.Instance.CurrRegion);

            m_bg.sprite = eventSprite;

            m_eventType = type;

            m_button.onClick.AddListener(HandleClick);

            EventMgr.Instance.LensModeUpdated += HandleLensModeUpdated;
            EventMgr.Instance.RegionSwitched += HandleRegionSwitched;
        }

        private void OnDisable() {
            EventMgr.Instance.LensModeUpdated -= HandleLensModeUpdated;
            EventMgr.Instance.RegionSwitched -= HandleRegionSwitched;
        }

        private void ShowUI() {
            m_group.alpha = 1;
        }

        private void HideUI() {
            m_group.alpha = 0;
        }

        private void HandleLensModeUpdated(object sender, LensModeEventArgs args) {
            switch (args.Mode) {
                case Lenses.Mode.Default:
                    switch (m_eventType) {
                        default:
                            HideUI();
                            break;
                    }
                    break;
                case Lenses.Mode.Phosphorus:
                    switch (m_eventType) {
                        case SimEventType.ExcessRunoff:
                        case SimEventType.Skimmers:
                        case SimEventType.HaltOperations:
                            ShowUI();
                            break;
                        default:
                            HideUI();
                            break;
                    }
                    break;

                case Lenses.Mode.Economic:
                    switch (m_eventType) {
                        case SimEventType.PopDecline:
                            // import/export taxes happen auto
                            ShowUI();
                            break;
                        default:
                            HideUI();
                            break;
                    }
                    break;

                default:
                    break;
            }
        }

        #region Handlers

        private void HandleClick() {
            // navigate to center of region
            LevelRegion thisRegion = RegionMgr.Instance.GetRegionByPos(new Vector3(this.transform.position.x, 0, this.transform.position.z));
            if (thisRegion != RegionMgr.Instance.CurrRegion) {
                EventMgr.Instance.TriggerEvent(ID.PanToRegion, new RegionSwitchedEventArgs(thisRegion));
            }

            // open advisor for this event
            switch (m_eventType) {
                case SimEventType.ExcessRunoff:
                    RunoffEvent();
                    break;
                case SimEventType.PopDecline:
                    PopulationDeclineEvent();
                    break;
                case SimEventType.Skimmers:
                    SkimmersEvent();
                    break;
                case SimEventType.HaltOperations:
                    HaltOperationsEvent();
                    break;
                // import/export taxes happen auto
                default:
                    break;
            }

            TriggerTracker.Instance.SetTriggerExpended(m_eventType);
            m_parentTriggers.RemoveEvent(this);
        }

        private void HandleRegionSwitched(object sender, RegionSwitchedEventArgs args) {
            // open/close banner
            LevelRegion thisRegion = RegionMgr.Instance.GetRegionByPos(new Vector3(this.transform.position.x, 0, this.transform.position.z));

            m_banner.enabled = m_bannerText.enabled = (thisRegion == RegionMgr.Instance.CurrRegion);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            // TODO: show banner / embiggen

        }

        public void OnPointerExit(PointerEventData eventData) {
            // TODO: show banner / embiggen

        }

        #endregion // Handlers

        #region Events

        private void RunoffEvent() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.RunoffPenalty);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            if (TriggerTracker.Instance.IsTriggerExpended(SimEventType.ExcessRunoff)) {
                // subsequent times only blurbs
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("The Lake has gotten bad. I recommend nipping the problem in the bud, at the source: CAFOS.", Advisors.AdvisorID.Ecology));
            }
            else {
                // first time unlocks choices
                EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("The Lake has gotten bad. I recommend nipping the problem in the bud, at the source: CAFOS.", Advisors.AdvisorID.Ecology, unlockList));
            }
        }

        private void PopulationDeclineEvent() {
            if (TriggerTracker.Instance.IsTriggerExpended(SimEventType.PopDecline)) {
                // subsequent times only blurbs
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("People have begun to move away from this city due to the algae blooms! Better find a solution to that.", Advisors.AdvisorID.Economic));
            }
            else {
                // first time still only blurb
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("People have begun to move away from this city due to the algae blooms! Better find a solution to that.", Advisors.AdvisorID.Economic));
            }
        }

        private void HaltOperationsEvent() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.HaltOperations);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            if (TriggerTracker.Instance.IsTriggerExpended(SimEventType.HaltOperations)) {
                // subsequent times only blurbs
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This CAFO is out of control. DNR has authorized shutting it down until the lakes are cleaner.", Advisors.AdvisorID.Ecology));
            }
            else {
                // first time unlocks choices
                EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("This CAFO is out of control. DNR has authorized shutting it down until the lakes are cleaner.", Advisors.AdvisorID.Ecology, unlockList));
            }
        }

        private void ImportTaxEvent() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ImportTax);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("This commercial phosphorus is too cheap, it's keeping our digesters from becoming competitive. Let's tax 'em.", Advisors.AdvisorID.Economic, unlockList));
        }

        private void ExportTaxEvent() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ExportTax);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("If we don’t make enough revenue, we won’t be able to afford roads, let alone fancy skimmers.", Advisors.AdvisorID.Economic, unlockList));
        }

        private void SkimmersEvent() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.Skimmers);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("It may be a band-aid solution, but we need to start implementing skimmers.", Advisors.AdvisorID.Ecology, unlockList));
        }

        #endregion // Events
    }
}