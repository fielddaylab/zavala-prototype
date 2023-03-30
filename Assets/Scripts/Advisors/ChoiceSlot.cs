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

        private void OnEnable() {
            m_button.onClick.AddListener(HandleClick);
        }

        private void OnDisable() {
            m_button.onClick.RemoveAllListeners();
        }

        private void HandleClick() {
            m_slotClickable.HandleClick(m_slotType);
        }
    }
}