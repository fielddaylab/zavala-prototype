using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Sim;

namespace Zavala
{
    public class ResultsMgr : MonoBehaviour
    {
        [SerializeField] private Button m_okayButton;

        [SerializeField] private FactionsDisplay m_factionsDisplay;
        [SerializeField] private ActionsDisplay m_actionsDisplay;
        

        private void Awake() {
            m_okayButton.onClick.AddListener(delegate { EventMgr.ResultsCanvasOkayed?.Invoke(); });

            EventMgr.SimPostActions.AddListener(OnSimPostActions);
        }

        private void OnDestroy() {
            m_okayButton.onClick.RemoveAllListeners();
        }

        public void OnSimPostActions(List<SimAction> prevSimActions) {
            m_actionsDisplay.LoadSimActions(prevSimActions);
        }
    }
}