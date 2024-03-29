﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Roads;
using Zavala.Tiles;

namespace Zavala.Interact
{
    public enum Mode
    {
        Select,
        PlaceItem, // shop item
        DrawRoad,
    }

    public class InteractMgr : MonoBehaviour
    {
        public static InteractMgr Instance;

        private Interact.Mode m_interactMode;

        private Texture2D m_defaultCursor = null;
        [SerializeField] private Texture2D m_placeCursor;
        // [SerializeField] private Texture2D m_removeCursor;


        #region Callbacks

        public void Init() {
            Instance = this;

            EventMgr.Instance.InteractModeUpdated += HandleInteractModeUpdated;
        }

        private void Update() {
            if (Time.timeScale == 0) { return; }

            if (Input.GetMouseButtonDown(1)) {
                if (m_interactMode == Interact.Mode.DrawRoad) {
                    RoadMgr.Instance.EndDrawingRoad();
                }

                // cancel interact mode with right click
                EventMgr.Instance.TriggerEvent(Events.ID.InteractModeUpdated, new InteractModeEventArgs(Interact.Mode.Select));
            }

            switch (m_interactMode) {
                case Interact.Mode.Select:
                    CheckForInspectClick();
                    break;
                case Interact.Mode.PlaceItem:
                    CheckForPlacementClick();
                    break;
                case Interact.Mode.DrawRoad:
                    CheckForDrawingRoad();
                    break;
                default:
                    break;
            }

            RoadMgr.Instance.HighlightRoadProgress(); // <- temp
        }


        private void CheckForInspectClick() {
            // detect for clicks
            if (Input.GetMouseButtonUp(0)) {
                Inspectable hoverInspectable = RegionMgr.OverInspectable(Input.mousePosition);

                if (hoverInspectable != null) {
                    hoverInspectable.Inspect();
                }
            }
        }

        private void CheckForPlacementClick() {
            // detect for clicks
            if (Input.GetMouseButtonUp(0)) {
                Tile hoverTile = RegionMgr.Instance.CurrRegion.GridMgr.OverTile(Input.mousePosition);

                if (hoverTile != null) {
                    hoverTile.ClickTilePlace();
                }
            }
        }

        private void CheckForDrawingRoad() {
            if (Input.GetMouseButtonDown(0)) {
                // button was clicked
                RoadMgr.Instance.StartDrawingRoad();
            }

            if (Input.GetMouseButton(0)) {
                // button continues to be held
                RoadMgr.Instance.ContinueDrawingRoad();
            }
            else {
                // button was released
                RoadMgr.Instance.EndDrawingRoad();
            }
        }

        #endregion // Unity Callbacks

        private void UpdateCursor(Interact.Mode inMode) {
            Texture2D newCursor = m_defaultCursor;
            Vector3 offset = Vector3.zero;

            switch (inMode) {
                case Interact.Mode.Select:
                    break;
                case Interact.Mode.PlaceItem:
                    newCursor = m_placeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                case Interact.Mode.DrawRoad:
                    newCursor = m_placeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                default:
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

            Debug.Log("[InteractMgr] Interact mode updated to " + args.Mode);
        }

        #endregion // Event Handlers
    }
}