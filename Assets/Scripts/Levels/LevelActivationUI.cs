using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Functionalities;

namespace Zavala
{
    public class LevelActivationUI : MonoBehaviour
    {
        [SerializeField] private Button[] m_levelButtons;

        public void Init() {
            for (int b = 0; b < m_levelButtons.Length; b++) {
                int levelIndex = b;
                m_levelButtons[b].onClick.AddListener(delegate {
                    EventMgr.Instance.TriggerEvent(ID.RegionToggled, new RegionToggleEventArgs(levelIndex));
                    m_levelButtons[levelIndex].interactable = false;

                    if (levelIndex == 2) {
                        // unlock import taxes
                        if (!TriggerTracker.Instance.IsTriggerExpended(SimEventType.ImportTax)) {
                            List<string> unlockList = new List<string>();

                            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ImportTax);

                            foreach (CardData data in unlockCards) {
                                unlockList.Add(data.CardID);
                            }

                            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("You can buy digesters now. Let's use import taxes to keep them competitive.", Advisors.AdvisorID.Economic, unlockList));
                            TriggerTracker.Instance.SetTriggerExpended(SimEventType.ImportTax);
                        }
                    }
                    if (levelIndex == 4) {
                        if (!TriggerTracker.Instance.IsTriggerExpended(SimEventType.ExportTax)) {
                            List<string> unlockList = new List<string>();

                            List<CardData> unlockCards = CardMgr.Instance.GetAllOptions(Sim.SimLeverID.ExportTax);

                            foreach (CardData data in unlockCards) {
                                unlockList.Add(data.CardID);
                            }

                            EventMgr.Instance.TriggerEvent(Events.ID.ChoiceUnlock, new ChoiceUnlockEventArgs("You now have access to export depots. We'll need to manage them using export taxes.", Advisors.AdvisorID.Economic, unlockList));
                            TriggerTracker.Instance.SetTriggerExpended(SimEventType.ExportTax);
                        }
                    }
                });
            }

            // deactivate region 0 button
            m_levelButtons[0].interactable = false;
        }
    }
}
