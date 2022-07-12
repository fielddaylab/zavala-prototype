using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Interact;


namespace Zavala
{
    public class TransportButton : Button
    {
        [SerializeField] private InteractMode m_interactMode;

        private void Awake() {
            onClick.AddListener(TransportClick);
        }

        #region Handlers

        private void TransportClick() {
            EventMgr.InteractModeUpdated?.Invoke(m_interactMode);
        }

        #endregion // Handlers
    }
}