using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Cards;
using Zavala.Sim;

namespace Zavala
{
    [RequireComponent(typeof(SlotClickable))]
    public class ChoiceSlot : MonoBehaviour
    {
        [SerializeField] private SimLeverID m_slotType;
        [SerializeField] private Button m_button;

        [SerializeField] private SlotClickable m_slotClickable;

        private bool m_isGlobal;

        private SlotCard m_selection;

        private void OnEnable() {
            ActivateButton();
        }

        private void OnDisable() {
            DeactivateButton();
        }

        public void ActivateButton() {
            m_button.onClick.AddListener(HandleClick);
            m_button.interactable = true;
        }

        public void DeactivateButton() {
            m_button.onClick.RemoveAllListeners();
            m_button.interactable = false;
        }

        private void HandleClick() {
            m_slotClickable.HandleClick(m_slotType, m_isGlobal);
        }

        public void SetGlobal(bool global) {
            m_isGlobal = global;
        }
    }
}