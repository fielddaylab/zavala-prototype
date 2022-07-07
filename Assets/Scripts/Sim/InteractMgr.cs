using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Interact
{
    public enum InteractMode
    {
        Farm,
        Phosphorous,
        Transport
        // etc.
    }

    public class InteractMgr : MonoBehaviour
    {
        private InteractMode m_interactMode;

        #region Unity Callbacks

        private void Awake() {
            EventMgr.InteractModeUpdated.AddListener(HandleInteractModeUpdated);
        }

        private void OnDestroy() {
            EventMgr.InteractModeUpdated.RemoveListener(HandleInteractModeUpdated);
        }

        #endregion // Unity Callbacks


        #region Event Handlers

        private void HandleInteractModeUpdated(InteractMode newMode) {
            m_interactMode = newMode;
            Debug.Log("[InteractMgr] New interact mode received: " + newMode);
        }

        #endregion // Event Handlers
    }
}