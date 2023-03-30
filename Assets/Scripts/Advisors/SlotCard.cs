using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Sim;

namespace Zavala.Cards
{
    public enum Severity {
        None,
        Low,
        Medium,
        High
    }

    [Serializable]
    public struct CardData {
        public string CardID;
        public string Header;
        public Severity Severity;
        public SimLeverID SimID;

        public CardData(string cardID, string header, Severity severity, SimLeverID simId) {
            CardID = cardID;
            Header = header;
            Severity = severity;
            SimID = simId;
        }
    }

    [RequireComponent(typeof(SlotClickable))]
    public class SlotCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_header;
        [SerializeField] private Image m_image;
        [SerializeField] private TMP_Text m_secondaryText;

        [SerializeField] private CardData m_data;
        [SerializeField] private Button m_button;

        [SerializeField] private Sprite m_severityNone, m_severityLow, m_severityMed, m_severityHigh;

        [SerializeField] private SlotClickable m_slotClickable;

        public void Init(CardData data) {
            m_header.text = data.Header;

            switch(data.Severity) {
                case Severity.None:
                    m_image.sprite = m_severityNone;
                    m_secondaryText.text = "NONE";
                    break;
                case Severity.Low:
                    m_image.sprite = m_severityLow;
                    m_secondaryText.text = "LOW";
                    break;
                case Severity.Medium:
                    m_image.sprite = m_severityMed;
                    m_secondaryText.text = "MEDIUM";
                    break;
                case Severity.High:
                    m_image.sprite = m_severityHigh;
                    m_secondaryText.text = "HIGH";
                    break;
                default:
                    break;
            }

            m_data = data;
        }

        private void OnEnable() {
            m_button.onClick.AddListener(HandleClick);
        }

        private void OnDisable() {
            m_button.onClick.RemoveAllListeners();
        }

        private void HandleClick() {
            m_slotClickable.HandleClick(m_data.SimID);
        }
    }
}
