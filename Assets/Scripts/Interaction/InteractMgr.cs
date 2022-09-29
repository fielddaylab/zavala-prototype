
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Functionalities;
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
        [SerializeField] private GameObject m_roadPrefab;

        private bool m_startedRoad;
        private List<Tile> m_tracedTiles;
        private Tile m_lastKnownTile;
        private Road m_roadInProgress;


        #region Callbacks

        public void Init() {
            Instance = this;

            EventMgr.Instance.InteractModeUpdated += HandleInteractModeUpdated;

            m_startedRoad = false;
            m_tracedTiles = new List<Tile>();
            m_roadInProgress = null;
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

            HighlightRoadProgress(); // <- temp
        }

        // TODO: remove this temp debug
        private void HighlightRoadProgress() {
            for(int i = 0; i < m_tracedTiles.Count; i++) {
                m_tracedTiles[i].DebugHighlight();
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
                    // check if a tile blocks building
                    if (GridMgr.OverTile(Input.mousePosition) == null || GridMgr.OverTile(Input.mousePosition).GetComponent<BlocksBuild>() != null) {
                        return;
                    }

                    // check if next to at least one connection node
                    List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdj(Input.mousePosition);
                    
                    if (adjNodes.Count > 0) {
                        // if so, save tile as starting node and start tracking
                        m_startedRoad = true;
                        m_roadInProgress = Instantiate(m_roadPrefab).GetComponent<Road>();
                        m_roadInProgress.SetStartConnectionNodes(adjNodes);
                        Tile currTile = GridMgr.OverTile(Input.mousePosition);
                        m_tracedTiles.Add(currTile);
                        m_lastKnownTile = currTile;
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
                    Tile currTile = GridMgr.OverTile(Input.mousePosition);
                    if (currTile != m_lastKnownTile) {
                        // changed tiles
                        if (m_tracedTiles.Contains(currTile)) {
                            // seen before -- don't add and rewind back to that tile

                            int rewindIndex = m_tracedTiles.IndexOf(currTile);
                            for (int i = m_tracedTiles.Count - 1; i > rewindIndex; i--) {
                                m_tracedTiles[i].UndoDebugHighlight();
                                m_tracedTiles.RemoveAt(i);
                            }
                            Debug.Log("[InteractMgr] rewound to index " + rewindIndex);
                        }
                        else {
                            // not seen before -- add if able

                            // cannot add road to blocking tile -- cancel road build
                            if (currTile == null || currTile.GetComponent<BlocksBuild>() != null) {
                                Debug.Log("Cannot build road on blocking or empty tile");
                                CancelRoad();
                                return;
                            }

                            m_tracedTiles.Add(currTile);
                            Debug.Log("[InteractMgr] added new tile to road path");
                        }
                        m_lastKnownTile = currTile;
                    }
                }
                else {
                    // button was released
                    if (TryCompleteRoad()) {
                        RoadCleanUp();
                    }
                    else {
                        CancelRoad();
                    }
                }
            }
        }

        private bool TryCompleteRoad() {
            // check if next to at least one connection node
            List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdj(Input.mousePosition);

            if (adjNodes.Count > 0) {
                // if so, save nodes as ending nodes
                m_roadInProgress.SetEndConnectionNodes(adjNodes);

                // save road segments
                m_roadInProgress.SetSegments(m_tracedTiles);

                // try to purchase road
                if (ShopMgr.Instance.TryPurchaseRoad(m_tracedTiles.Count)) {
                    // save road
                    for(int i = 0; i < m_tracedTiles.Count; i++) {
                        m_tracedTiles[i].ConstructRoad(ShopMgr.Instance.GetPurchasePrefab());
                    }

                    Debug.Log("[InteractMgr] Road saved!");
                    return true;
                }
                else {
                    // clear road
                    Debug.Log("[InteractMgr] shop failure");
                    return false;
                }
            }

            return false;
        }

        private void CancelRoad() {
            RoadCleanUp();
        }

        private void RoadCleanUp() {
            for (int i = m_tracedTiles.Count - 1; i > -1; i--) {
                m_tracedTiles[i].UndoDebugHighlight();
            }

            Debug.Log("Ended building road");
            m_startedRoad = false;
            m_tracedTiles.Clear();
            Destroy(m_roadInProgress.gameObject);
            m_roadInProgress = null;
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