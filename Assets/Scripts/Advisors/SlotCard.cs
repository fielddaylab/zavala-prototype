using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Advisors;
using Zavala.Events;
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

    public class SlotCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_header;
        [SerializeField] private Image m_image;
        [SerializeField] private TMP_Text m_secondaryText;

        [SerializeField] private CardData m_data;
        [SerializeField] private Button m_button;
        [SerializeField] private Canvas m_canvas;

        [SerializeField] private Sprite m_severityNone, m_severityLow, m_severityMed, m_severityHigh;

        private bool m_activated;

        private AdvisorUI m_parentUI;

        private SlotClickable m_baseSlot;

        private bool m_isGlobal;

        public void Display(CardData data, AdvisorUI parentUI) {
            m_header.text = data.Header;
            m_header.color = parentUI.ColorTheme;

            switch (data.Severity) {
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
            m_image.color = parentUI.ColorTheme;
            m_secondaryText.color = parentUI.ColorTheme;

            m_data = data;

            m_canvas.sortingOrder += 2;

            Destroy(this.gameObject.GetComponent<Button>());
        }

        public void Init(CardData data, AdvisorUI parentUI, SlotClickable baseSlot, bool isGlobal) {
            m_header.text = data.Header;
            m_parentUI = parentUI;
            m_header.color = parentUI.ColorTheme;

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
            m_image.color = parentUI.ColorTheme;
            m_secondaryText.color = parentUI.ColorTheme;

            m_data = data;

            m_baseSlot = baseSlot;

            m_isGlobal = isGlobal;
        }

        public void SetInSlot() {
            m_activated = true;
            m_canvas.sortingOrder--;
        }

        public bool GetActivated() {
            return m_activated;
        }

        public CardData GetCardData() {
            return m_data;
        }

        public void RemoveFromSlot() {
            m_canvas.sortingOrder++;
        }

        private void OnEnable() {
            m_button.onClick.AddListener(HandleClick);
        }

        private void OnDisable() {
            m_button.onClick.RemoveAllListeners();
        }

        private void HandleClick() {
            if (!m_activated) {
                EventMgr.Instance.TriggerEvent(Events.ID.ChoiceSlotUpdated, new ChoiceSlotEventArgs(this, RegionMgr.Instance.CurrRegion.GetRegionNum(), m_isGlobal));
                SetInSlot();
            }
            else {
                m_baseSlot.HandleClick(m_data.SimID, m_isGlobal);
            }
        }
    }
}
