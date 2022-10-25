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
        [SerializeField] private Sprite m_roadStraightSprite, m_roadEndSprite, m_roadBendSprite, m_roadTightBendSprite, m_roadRoundaboutSprite;
        //[SerializeField] private GameObject m_roadEndPrefab, m_roadStraightPrefab, m_roadBendPrefab, m_roadRoundaboutPrefab;

        private bool m_startedRoad;
        private List<Tile> m_tracedTiles;
        private Tile m_lastKnownTile;

        private List<RoadSegment> m_stagedSegments;
        private Road m_roadInProgress;

        public void Init() {
            Instance = this;

            m_startedRoad = false;
            m_tracedTiles = new List<Tile>();
            m_roadInProgress = null;
            m_stagedSegments = new List<RoadSegment>();
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
                // check if valid place for road (not empty, doesn't block build, and not road)
                Tile startTile = GridMgr.OverTile(Input.mousePosition);
                if (startTile == null || startTile.GetComponent<BlocksBuild>() != null || GridMgr.RoadAtPos(startTile.transform.position) != null) {
                    return;
                }

                // check if next to at least one connection node
                Tile centerTile = GridMgr.OverTile(Input.mousePosition);
                List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdjToTile(centerTile);

                if (adjNodes.Count > 0) {

                    // if so, save tile as starting node and start tracking
                    m_startedRoad = true;
                    Debug.Log("[Instantiate] Instantiating road prefab");
                    Tile currTile = GridMgr.OverTile(Input.mousePosition);
                    m_tracedTiles.Add(currTile);
                    m_lastKnownTile = currTile;

                    // place an initial road segment prefab
                    StageRoadSegment(currTile.gameObject);

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
                            UnstageSegment(i);
                        }

                        Debug.Log("[InteractMgr] rewound to index " + rewindIndex);
                    }
                    else {
                        // not seen before -- add if able

                        // cannot add road to blocking tile -- cancel road build
                        if (currTile == null || currTile.GetComponent<BlocksBuild>() != null || GridMgr.RoadAtPos(currTile.transform.position) != null) {
                            Debug.Log("Cannot build road on blocking, empty, or road tile");
                            CancelRoad();
                            return;
                        }

                        Tile prevTile = m_tracedTiles[m_tracedTiles.Count - 1];
                        m_tracedTiles.Add(currTile);
                        StageRoadSegment(currTile.gameObject);
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

            bool validEnd = false;

            // check if next to at least one connection node
            Tile centerTile = GridMgr.OverTile(Input.mousePosition);
            List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdjToTile(centerTile);
            if (adjNodes.Count > 0) {
                validEnd = true;
            }

            List<RoadSegment> adjSegments = GridMgr.AdjRoadSegments(m_lastKnownTile, m_stagedSegments);
            if (adjSegments.Count > 0) {
                validEnd = true;
            }

            if (validEnd) {
                // TODO: refresh connecting edges

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
                Destroy(m_stagedSegments[i].gameObject);
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
            m_stagedSegments.Clear();
        }

        private void FinalizeRoad() {
            for (int segIndex = 0; segIndex < m_stagedSegments.Count; segIndex++) {
                // check if near a road or connection node
                RoadSegment currSegment = m_stagedSegments[segIndex];
                Tile centerTile = GridMgr.TileAtPos(currSegment.transform.position);
                List<RoadSegment> adjSegments = GridMgr.AdjRoadSegments(centerTile);

                // if road
                if (adjSegments.Count > 0) {
                    // generate an edge and scale
                    for (int adjIndex = 0; adjIndex < adjSegments.Count; adjIndex++) {
                        RoadSegment adjSegment = adjSegments[adjIndex];
                        RoadBuildDir edgeDir = CalcBuildDirByPos(currSegment.transform.position, adjSegment.transform.position);
                        float elevationDelta = CalcElevationDeltaByPos(currSegment.transform.position, adjSegment.transform.position);
                        currSegment.ActivateEdge(edgeDir, elevationDelta);

                        // add an edge to the other road
                        RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);
                        adjSegment.ActivateEdge(reverseDir, -elevationDelta);
                    }
                }

                // if connection node
                List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdjToTile(centerTile);
                if (adjNodes.Count > 0) {
                    // generate an edge and scale
                    for (int adjIndex = 0; adjIndex < adjNodes.Count; adjIndex++) {
                        RoadBuildDir edgeDir = CalcBuildDirByPos(currSegment.transform.position, adjNodes[adjIndex].transform.position);
                        float elevationDelta = CalcElevationDeltaByPos(currSegment.transform.position, adjNodes[adjIndex].transform.position);
                        currSegment.ActivateEdge(edgeDir, elevationDelta);
                    }
                    // add this road to the connection node's list of road outlets
                }
            }

            Debug.Log("[InteractMgr] Road saved!");
        }

        private void StageRoadSegment(GameObject tileUnderRoadObj) {
            GameObject toStage = m_roadSegmentPrefab;

            Debug.Log("[Instantiate] Instantiating road segment prefab");
            RoadSegment roadSegmentInstance = Instantiate(toStage, tileUnderRoadObj.transform).GetComponent<RoadSegment>();

            // TODO: reveal connections according to neighboring roads / connection nodes

            m_stagedSegments.Add(roadSegmentInstance);
        }

        private void UnstageSegment(int i) {
            RoadSegment toUnstage = m_stagedSegments[i];
            Destroy(toUnstage.gameObject);
            m_stagedSegments.RemoveAt(i);
        }

        private RoadBuildDir CalcBuildDirByPos(Vector3 prevSegmentPos, Vector3 currSegmentPos) {
            Vector3 dirVector = (currSegmentPos - prevSegmentPos).normalized;

            return CalcBuildDirFromVector(dirVector);
        }

        private RoadBuildDir CalcBuildDirFromVector(Vector3 dirVector) {
            if (dirVector.x > 0) {
                // up, up-left, or up-right
                if (dirVector.z < 0) {
                    // up-right
                    return RoadBuildDir.Up_Right;
                }
                else if (dirVector.z > 0) {
                    // up-left
                    return RoadBuildDir.Up_Left;
                }
                else {
                    // up
                    return RoadBuildDir.Up;
                }
            }
            else {
                // down, down-left, or down-right
                if (dirVector.z < 0) {
                    // down-right
                    return RoadBuildDir.Down_Right;
                }
                else if (dirVector.z > 0) {
                    // down-left
                    return RoadBuildDir.Down_Left;
                }
                else {
                    // down
                    return RoadBuildDir.Down;
                }
            }
        }

        private float CalcElevationDeltaByPos(Vector3 prevSegmentPos, Vector3 currSegmentPos) {
            return currSegmentPos.y - prevSegmentPos.y;
        }

        #endregion // Helpers

        #region External

        public GameObject GetRoadSegmentPrefab() {
            return m_roadSegmentPrefab;
        }

        #endregion // External
    }
}
