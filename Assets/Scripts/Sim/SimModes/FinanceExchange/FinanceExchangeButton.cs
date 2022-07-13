using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Interact;


namespace Zavala
{
    public class FinanceExchangeButton : Button
    {
        [SerializeField] private InteractMode m_interactMode;

        private void Awake() {
            onClick.AddListener(ButtonClick);
        }

        #region Handlers

        private void ButtonClick() {
            EventMgr.InteractModeUpdated?.Invoke(m_interactMode);
        }

        #endregion // Handlers
    }
}