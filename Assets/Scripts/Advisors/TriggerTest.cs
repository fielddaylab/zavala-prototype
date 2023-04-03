using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Cards;
using Zavala.Events;

namespace Zavala {
    public class TriggerTest : MonoBehaviour
    {
        [SerializeField] private Button[] m_buttons;

        private void Start() {
            m_buttons[0].onClick.AddListener(HandleB0);
            m_buttons[1].onClick.AddListener(HandleB1);
            m_buttons[2].onClick.AddListener(HandleB2);
            m_buttons[3].onClick.AddListener(HandleB3);
            m_buttons[4].onClick.AddListener(HandleB4);
        }

        #region Handlers (Triggers)

        private void HandleB0() {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This is a blurb from the ecology advisor!", Advisors.AdvisorID.Ecology));
        }

        private void HandleB1() {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This is a silent update!", Advisors.AdvisorID.Ecology, true));
        }

        private void HandleB2() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.RunoffPenalty);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("The Lake has gotten bad. I recommend nipping the problem in the bud, at the source: CAFOS.", Advisors.AdvisorID.Ecology, unlockList));
        }

        private void HandleB3() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ExportTax);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("If we don’t make enough revenue, we won’t be able to afford roads, let alone fancy skimmers.", Advisors.AdvisorID.Economic, unlockList));
        }

        private void HandleB4() {
            List<string> unlockList = new List<string>();

            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.GooseRace);

            foreach (CardData data in unlockCards) {
                unlockList.Add(data.CardID);
            }

            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("The citizenry are clamoring for a holiday, and the geese need running.", Advisors.AdvisorID.Goose, unlockList));
        }

        #endregion // Handlers
    }
}