using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala.Roads
{
    public enum RoadSegmentType
    {
        End,
        Straight,
        Bend,
        TightBend,
        Roundabout
    }

    public enum RoadBuildDir {
        Up, // 0
        Up_Right, // 1
        Down_Right, // 2
        Down, // 3
        Down_Left, // 4
        Up_Left, // 5
    }

    public class RoadMgr : MonoBehaviour
    {
        public static RoadMgr Instance;

        // Road creation
        [SerializeField] private GameObject m_roadPrefab;

        [SerializeField] private GameObject m_roadSegmentPrefab;
        [SerializeField] private Sprite m_roadStraightSprite, m_roadEndSprite, m_roadBendSprite, m_roadTightBendSprite;
        //[SerializeField] private GameObject m_roadEndPrefab, m_roadStraightPrefab, m_roadBendPrefab, m_roadRoundaboutPrefab;

        private bool m_startedRoad;
        private List<Tile> m_tracedTiles;
        private Tile m_lastKnownTile;
        private Road m_roadInProgress;

        public void Init() {
            Instance = this;

            m_startedRoad = false;
            m_tracedTiles = new List<Tile>();
            m_roadInProgress = null;
        }

        // TODO: remove this temp debug
        public void HighlightRoadProgress() {
            for (int i = 0; i < m_tracedTiles.Count; i++) {
                m_tracedTiles[i].DebugHighlight();
            }
        }

        public bool StartedRoad {
            get { return m_startedRoad; }
        }

        public void StartDrawingRoad() {
            if (!RoadMgr.Instance.StartedRoad) {
                // check if a tile blocks building
                if (GridMgr.OverTile(Input.mousePosition) == null || GridMgr.OverTile(Input.mousePosition).GetComponent<BlocksBuild>() != null) {
                    return;
                }

                // check if next to at least one connection node
                List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdj(Input.mousePosition);

                if (adjNodes.Count > 0) {
                    // if so, save tile as starting node and start tracking
                    m_startedRoad = true;
                    Debug.Log("[Instantiate] Instantiating road prefab");
                    m_roadInProgress = Instantiate(m_roadPrefab).GetComponent<Road>();
                    m_roadInProgress.SetStartConnectionNodes(adjNodes);
                    Tile currTile = GridMgr.OverTile(Input.mousePosition);
                    m_tracedTiles.Add(currTile);
                    m_lastKnownTile = currTile;

                    // place an initial road segment prefab
                    m_roadInProgress.ConstructRoadSegmentInstance(currTile.gameObject, true);

                    Debug.Log("Started building road");
                }
                else {
                    Debug.Log("Invalid start: not next to connection node");
                }
            }
        }

        public void ContinueDrawingRoad() {
            if (RoadMgr.Instance.StartedRoad) {
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
                            m_roadInProgress.RemoveRoadSegmentInstance(i);
                        }

                        // reset latest tile to straight type (unless end)
                        if (m_roadInProgress.GetSegmentType(m_tracedTiles.Count - 1) == RoadSegmentType.End) {
                            m_roadInProgress.ConvertRoadSegment(RoadSegmentType.End, m_tracedTiles.Count - 1);
                        }
                        else {
                            m_roadInProgress.ConvertRoadSegment(RoadSegmentType.Straight, m_tracedTiles.Count - 1);
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

                        Tile prevTile = m_tracedTiles[m_tracedTiles.Count - 1];
                        m_tracedTiles.Add(currTile);
                        m_roadInProgress.ConstructRoadSegmentInstance(currTile.gameObject, false);
                        Debug.Log("[InteractMgr] added new tile to road path");
                    }
                    m_lastKnownTile = currTile;
                }
            }
        }

        public void EndDrawingRoad() {
            if (m_startedRoad) {
                Debug.Log("[RoadMgr] end drawing road start");
                if (RoadMgr.Instance.StartedRoad) {
                    Debug.Log("[RoadMgr] end drawing road 1");

                    if (TryCompleteRoad()) {
                        Debug.Log("[RoadMgr] end drawing road 2");

                        RoadCleanUp();
                    }
                    else {
                        Debug.Log("[RoadMgr] end drawing road 3");

                        CancelRoad();
                    }
                }
            }
        }

        #region Helpers

        private bool TryCompleteRoad() {
            Debug.Log("[RoadMgr] try complete road");

            // check if next to at least one connection node
            List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdj(Input.mousePosition);

            if (adjNodes.Count > 0) {
                // if so, save nodes as ending nodes
                m_roadInProgress.SetEndConnectionNodes(adjNodes);

                // convert last road segment to an end (or roundabout if 1)
                RoadSegmentType endType = m_tracedTiles.Count == 1 ? RoadSegmentType.Roundabout : RoadSegmentType.End;
                m_roadInProgress.ConvertRoadSegment(endType, m_tracedTiles.Count - 1);

                // save road segments
                m_roadInProgress.SetSegments(m_tracedTiles);

                Debug.Log("[RoadMgr] try purchase road");

                // try to purchase road
                if (ShopMgr.Instance.TryPurchaseRoad(m_tracedTiles.Count)) {
                    Debug.Log("[RoadMgr] Finalizing road");
                    // save road in mgr and connected nodes
                    FinalizeRoad();

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
            for (int i = m_tracedTiles.Count - 1; i > -1; i--) {
                m_roadInProgress.RemoveRoadSegmentInstance(i);
            }

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

        private void FinalizeRoad() {
            // on tiles
            // m_tracedTiles[i].ConstructRoad(ShopMgr.Instance.GetPurchasePrefab());
            //m_roadInProgress.ConstructRoad(ShopMgr.Instance.GetPurchasePrefab());

            // in connection nodes
            m_roadInProgress.FinalizeConnections();

            //m_roadInProgress.NormalizeSegmentHeights();

            Debug.Log("[InteractMgr] Road saved!");
        }

        #endregion // Helpers

        #region External

        public Sprite GetRoadSprite(RoadSegmentType segmentType) {
            switch(segmentType) {
                case RoadSegmentType.End:
                    return m_roadEndSprite;
                case RoadSegmentType.Straight:
                    return m_roadStraightSprite;
                case RoadSegmentType.Bend:
                    return m_roadBendSprite;
                case RoadSegmentType.TightBend:
                    return m_roadTightBendSprite;
                default:
                    return null;
            }
        }

        public GameObject GetRoadSegmentPrefab() {
            return m_roadSegmentPrefab;
        }

        #endregion // External
    }
}
