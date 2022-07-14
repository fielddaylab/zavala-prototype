using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Factions;
using Zavala.Sim;

namespace Zavala
{
    public class ResultsMgr : MonoBehaviour
    {
        public static ResultsMgr Instance;

        [SerializeField] private Button m_okayButton;

        [SerializeField] private FactionsDisplay m_factionsDisplay;
        [SerializeField] private ActionsDisplay m_actionsDisplay;
        [SerializeField] private NewTechDisplay m_newTechDisplay;
        [SerializeField] private TMP_Text m_feedbackText;

        [SerializeField] private Image m_hat;

        private SimModeData m_currSimModeData;


        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
            }

            m_okayButton.onClick.AddListener(delegate { EventMgr.ResultsCanvasOkayed?.Invoke(); });

            EventMgr.SimPostActions.AddListener(OnSimPostActions);
        }

        private void OnDestroy() {
            m_okayButton.onClick.RemoveAllListeners();
        }

        private void OnEnable() {
            m_okayButton.onClick.AddListener(ApplyUnlocks);
        }

        private void OnDisable() {
            m_okayButton.onClick.RemoveListener(ApplyUnlocks);
        }

        public void OnSimPostActions(List<SimAction> prevSimActions) {
            m_factionsDisplay.ResetSliders();

            prevSimActions.Sort();

            SimAction prevAction = SimAction.Null;
            List<FactionType> dislikers = new List<FactionType>();
            List<FactionType> likers = new List<FactionType>();
            int actionCount = 0;

            SimAction lastAction = SimAction.Null;

            string actionStr = "";
            string interpretStr = "";
            foreach (SimAction action in prevSimActions) {
                if (action == prevAction) {
                    // another of same type
                    actionCount++;
                }
                else if (prevAction != SimAction.Null) {
                    // one of different type; count is over (reset to 1)
                    actionStr += prevAction.ToString() + " x" + actionCount;
                    actionCount = 1;
                    dislikers = m_factionsDisplay.FactionsDislikingAction(action);
                    likers = m_factionsDisplay.FactionsLikingAction(action);
                    foreach (var d in dislikers) {
                        actionStr += "\n";
                        interpretStr += "- " + d.ToString() + "\n";
                        Debug.Log("Disliker of " + action.ToString() + ": " + d.ToString());
                    }
                    foreach (var l in likers) {
                        actionStr += "\n";
                        interpretStr += "+ " + l.ToString() + "\n";
                        Debug.Log("Liker of " + action.ToString() + ": " + l.ToString());
                    }
                    actionStr += "\n";
                    interpretStr += "\n";
                    m_actionsDisplay.InterpretText.text = interpretStr;
                    prevAction = action;
                }
                else {
                    // first non null type
                    actionCount = 1;
                    prevAction = action;
                }

                m_factionsDisplay.InterpretAction(action);
                lastAction = action;
            }

            // get last action category
            actionStr += prevAction.ToString() + " x" + actionCount + "\n";
            dislikers = m_factionsDisplay.FactionsDislikingAction(lastAction);
            likers = m_factionsDisplay.FactionsLikingAction(lastAction);
            foreach (var d in dislikers) {
                actionStr += "\n";
                interpretStr += "- " + d.ToString() + "\n";
                Debug.Log("Disliker of " + lastAction.ToString() + ": " + d.ToString());
            }
            foreach (var l in likers) {
                actionStr += "\n";
                interpretStr += "+ " + l.ToString() + "\n";
                Debug.Log("Liker of " + lastAction.ToString() + ": " + l.ToString());
            }

            m_actionsDisplay.InterpretText.text = interpretStr;
            m_actionsDisplay.ActionText.text = actionStr;
        }

        public void LoadSimData(SimModeData data) {
            m_currSimModeData = data;
            m_feedbackText.text = data.FeedbackText;
            m_newTechDisplay.NewTechText.text = data.UnlockModeText;
            m_hat.color = data.HatColor;
        }

        private void ApplyUnlocks() {
            foreach (string id in m_currSimModeData.UnlocksModes) {
                UnlockMgr.Instance.UnlockSim(id);
            }
        }
    }
}