using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Interact
{
    public enum InteractMode
    {
        Default,
        //Phosphorous,
        Transport_Rail,
        Transport_Highway,
        Transport_Road,
        Transport_Bridge
        //Transport_Remove
        // etc.
    }

    public class InteractMgr : MonoBehaviour
    {
        private Texture2D m_defaultCursor = null;
        [SerializeField] private Texture2D m_drawCursor;
        [SerializeField] private Texture2D m_bridgeCursor;

        private InteractMode m_interactMode;

        #region Unity Callbacks

        private void Awake() {
            EventMgr.InteractModeUpdated.AddListener(HandleInteractModeUpdated);
        }

        private void OnDestroy() {
            EventMgr.InteractModeUpdated.RemoveListener(HandleInteractModeUpdated);
        }

        #endregion // Unity Callbacks

        private void UpdateCursor(InteractMode inMode) {
            Texture2D newCursor = m_defaultCursor;
            Vector3 offset = Vector3.zero;

            switch(inMode) {
                default:
                    break;
                case InteractMode.Default:
                    break;
                case InteractMode.Transport_Rail:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case InteractMode.Transport_Highway:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case InteractMode.Transport_Road:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case InteractMode.Transport_Bridge:
                    newCursor = m_bridgeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
            }

            Cursor.SetCursor(newCursor, offset, CursorMode.ForceSoftware);
        }

        #region Event Handlers

        private void HandleInteractModeUpdated(InteractMode newMode) {
            m_interactMode = newMode;
            Debug.Log("[InteractMgr] New interact mode received: " + newMode);

            UpdateCursor(newMode);
        }

        #endregion // Event Handlers
    }
}