using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zavala.Sim;

namespace Zavala
{
    public class ActionsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;

        private List<SimAction> m_actions;

        public void LoadSimActions(List<SimAction> actions) {
            m_actions = actions;

            actions.Sort();

            SimAction prevAction = SimAction.Null;
            int actionCount = 0;

            string actionStr = "\n";
            foreach (SimAction action in actions) {
                if (action == prevAction) {
                    // another of same type
                    actionCount++;
                }
                else if (prevAction != SimAction.Null){
                    // one of different type; count is over (reset to 1)
                    actionStr += prevAction.ToString() + " x" + actionCount + "\n";
                    actionCount = 1;
                    prevAction = action;
                }
                else {
                    // first non null type
                    actionCount = 1;
                    prevAction = action;
                }
            }

            // get last action category
            actionStr += prevAction.ToString() + " x" + actionCount + "\n";

            m_text.text = actionStr;
        }
    }
}