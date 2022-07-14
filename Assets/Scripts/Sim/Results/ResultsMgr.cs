﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
            m_actionsDisplay.LoadSimActions(prevSimActions);
        }

        public void LoadSimData(SimModeData data) {
            m_currSimModeData = data;
            m_feedbackText.text = data.FeedbackText;
            m_newTechDisplay.NewTechText.text = data.UnlockModeText;
            m_hat.color = data.HatColor;
        }
        
        private void ApplyUnlocks() {
            foreach(string id in m_currSimModeData.UnlocksModes) {
                UnlockMgr.Instance.UnlockSim(id);
            }
        }
    }
}