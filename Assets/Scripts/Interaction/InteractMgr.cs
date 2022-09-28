
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Tiles;

// PLACEHOLDER from last iteration. Currently non-functional

namespace Zavala.Interact
{
    public enum Mode
    {
        DefaultSelect,
        PlaceItem, // shop item
        DrawRoad,
        PhosphorousSelect
    }

    public class InteractMgr : MonoBehaviour
    {
        public static InteractMgr Instance;

        private Interact.Mode m_interactMode;

        private Texture2D m_defaultCursor = null;
        [SerializeField] private Texture2D m_placeCursor;
        // [SerializeField] private Texture2D m_removeCursor;


        // Road creation
        private bool m_startedRoad;


        #region Callbacks

        public void Init() {
            Instance = this;

            EventMgr.Instance.InteractModeUpdated += HandleInteractModeUpdated;

            m_startedRoad = false;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(1)) {
                // cancel interact mode with right click
                EventMgr.Instance.TriggerEvent(Events.ID.InteractModeUpdated, new InteractModeEventArgs(Interact.Mode.DefaultSelect));
            }

            switch (m_interactMode) {
                case Interact.Mode.DefaultSelect:
                    CheckForClick();
                    break;
                case Interact.Mode.PlaceItem:
                    CheckForClick();
                    break;
                case Interact.Mode.DrawRoad:
                    CheckForDrawingRoad();
                    break;
                case Interact.Mode.PhosphorousSelect:
                    CheckForClick();
                    break;
                default:
                    break;
            }
        }


        private void CheckForClick() {
            // detect for clicks
            if (Input.GetMouseButtonDown(0)) {
                Tile hoverTile = GridMgr.OverTile(Input.mousePosition);

                if (hoverTile != null) {
                    hoverTile.ClickTile();
                }
            }
        }

        private void CheckForDrawingRoad() {
            // road has not been started
            if (!m_startedRoad) {
                if (Input.GetMouseButtonDown(0)) {
                    // check if next to at least one connection node
                    List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdj(Input.mousePosition);
                    
                    if (adjNodes.Count > 0) {
                        // if so, save tile as starting node and start tracking
                        m_startedRoad = true;
                        Debug.Log("Started building road");
                    }
                    else {
                        Debug.Log("Invalid start: not next to connection node");
                    }
                }
            }
            // road is being actively drawn
            else {
                if (Input.GetMouseButton(0)) {
                    // update tile mouse is over


                    // check if next to connection node
                    // if so, save tile as starting node and start tracking
                }
                else {
                    // button was released
                    m_startedRoad = false;
                    TryCompleteRoad();
                    Debug.Log("Ended building road");
                }
            }
        }

        private void TryCompleteRoad() {
            // complete road:
            // check if final connects to a connection point
            // check if player has enough money for the number of road tiles
        }

        #endregion // Unity Callbacks

        private void UpdateCursor(Interact.Mode inMode) {
            Texture2D newCursor = m_defaultCursor;
            Vector3 offset = Vector3.zero;

            switch (inMode) {
                default:
                    break;
                case Interact.Mode.DefaultSelect:
                    break;
                case Interact.Mode.PlaceItem:
                    newCursor = m_placeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                case Interact.Mode.DrawRoad:
                    newCursor = m_placeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                case Interact.Mode.PhosphorousSelect:
                    break;
            }

            Cursor.SetCursor(newCursor, offset, CursorMode.ForceSoftware);
        }

        public Interact.Mode GetCurrMode() {
            return m_interactMode;
        }

        #region Event Handlers

        private void HandleInteractModeUpdated(object sender, InteractModeEventArgs args) {
            m_interactMode = args.Mode;

            UpdateCursor(m_interactMode);
        }

        #endregion // Event Handlers
    }
}