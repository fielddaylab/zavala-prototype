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

    public enum RoadBuildDir
    {
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

        public void Init() {
            Instance = this;

            m_startedRoad = false;
            m_tracedTiles = new List<Tile>();
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
                // check if valid place for road start (not empty, and has either connection node or road)
                Tile startTile = GridMgr.OverTile(Input.mousePosition);
                if (startTile == null ||
                    (startTile.GetComponent<ConnectionNode>() == null
                    && !startTile.ConnectionInAddOns()
                    && GridMgr.RoadAtPos(startTile.transform.position) == null)) {
                    return;
                }

                bool validStart = true;

                // check if next to at least one connection node
                //Tile centerTile = GridMgr.OverTile(Input.mousePosition);
                //List<ConnectionNode> adjNodes = GridMgr.ConnectingNodesAdjToTile(centerTile);
                // validStart = adjNodes.Count > 0

                if (validStart) {
                    // if so, save tile as starting node and start tracking
                    m_startedRoad = true;
                    Debug.Log("[Instantiate] Instantiating road prefab");
                    Tile currTile = GridMgr.OverTile(Input.mousePosition);
                    m_tracedTiles.Add(currTile);
                    m_lastKnownTile = currTile;

                    // place an initial road segment prefab
                    // StageRoadSegment(currTile.gameObject);

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
                        int numToUnstage = m_tracedTiles.Count - 1 - rewindIndex;
                        Debug.Log("[RoadMgr] num to unstage: " + numToUnstage);
                        for (int i = m_tracedTiles.Count - 1; i > rewindIndex; i--) {
                            // first trace is not a new road tile
                            m_tracedTiles[i].UndoDebugHighlight();
                            m_tracedTiles.RemoveAt(i);
                        }

                        int lowerBound = m_stagedSegments.Count - 1 - numToUnstage;
                        for (int i = m_stagedSegments.Count - 1; i > lowerBound; i--) {
                            UnstageSegment(i);
                        }

                        Debug.Log("[InteractMgr] rewound to index " + rewindIndex);
                    }
                    else {
                        // not seen before -- add if able

                        // cannot add road to empty tile -- cancel road build
                        if (currTile == null) {
                            Debug.Log("Cannot build road on empty tile");
                            CancelRoad();
                            return;
                        }

                        // reach a potential termination point
                        if (currTile.GetComponent<ConnectionNode>() != null || currTile.ConnectionInAddOns() || GridMgr.RoadAtPos(currTile.transform.position) != null) {
                            m_tracedTiles.Add(currTile);
                            EndDrawingRoad();
                            return;
                        }

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

            Tile endTile = GridMgr.OverTile(Input.mousePosition);
            ConnectionNode endConnection = endTile.GetComponent<ConnectionNode>();
            RoadSegment endRoad = GridMgr.RoadAtPos(endTile.transform.position);
            if (endConnection == null
                && !endTile.ConnectionInAddOns()
                && (endRoad == null || m_stagedSegments.Contains(endRoad))) {
                // road must end on a connection or an existing road
                return false;
            }
            RoadSegment startRoad = GridMgr.RoadAtPos(m_tracedTiles[0].transform.position);
            if (startRoad == null && endRoad == null && m_stagedSegments.Count == 0) {
                // there must be a road segment in at least one of the start, middle, and end
                return false;
            }

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

        private void CancelRoad() {
            for (int i = m_stagedSegments.Count - 1; i > -1; i--) {
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
            // check if start is a road
            Vector3 firstPos = m_tracedTiles[0].transform.position;
            RoadSegment firstRoad = GridMgr.RoadAtPos(firstPos);
            if (firstRoad != null) {
                // road -- generate edge in direction of first segment
                Vector3 secondPos = m_stagedSegments.Count > 0 ? m_stagedSegments[0].transform.position : m_tracedTiles[m_tracedTiles.Count - 1].transform.position;
                RoadBuildDir dir = CalcBuildDirByPos(secondPos, firstPos);
                float elevationDelta = CalcElevationDeltaByPos(secondPos, firstPos);
                firstRoad.ActivateEdge(dir, elevationDelta);
            }

            // create edges for intermediate nodes
            Vector3 prevPos;
            Vector3 currPos = firstPos;
            for (int segIndex = 0; segIndex < m_stagedSegments.Count; segIndex++) {
                RoadSegment currSegment = m_stagedSegments[segIndex];
                prevPos = currPos;
                currPos = currSegment.transform.position;

                // add an edge from this road back to previous position
                RoadBuildDir edgeDir = CalcBuildDirByPos(prevPos, currPos);
                float elevationDelta = CalcElevationDeltaByPos(prevPos, currPos);
                currSegment.ActivateEdge(edgeDir, elevationDelta);

                if (segIndex != 0) {
                    // add an edge from previous road forward to this road
                    RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                    RoadSegment prevSegment = m_stagedSegments[segIndex - 1];
                    prevSegment.ActivateEdge(reverseDir, -elevationDelta);
                }
            }

            // check end
            Vector3 endPos = m_tracedTiles[m_tracedTiles.Count - 1].transform.position;
            RoadSegment endSegment = GridMgr.RoadAtPos(endPos);
            prevPos = currPos;
            currPos = endPos;
            // if road
            if (endSegment != null) {
                Debug.Log("[RoadMgr] Final is road");
                // add an edge from this road back to previous road
                RoadBuildDir edgeDir = CalcBuildDirByPos(prevPos, currPos);
                float elevationDelta = CalcElevationDeltaByPos(prevPos, currPos);
                endSegment.ActivateEdge(edgeDir, elevationDelta);

                if (m_stagedSegments.Count > 0) {
                    // add an edge from previous road forward to this road
                    RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                    RoadSegment prevSegment = m_stagedSegments[m_stagedSegments.Count - 1];
                    prevSegment.ActivateEdge(reverseDir, -elevationDelta);
                }
            }
            // if connection node
            if (m_tracedTiles[m_tracedTiles.Count - 1].GetComponent<ConnectionNode>() != null 
                || m_tracedTiles[m_tracedTiles.Count - 1].ConnectionInAddOns()) {
                Debug.Log("[RoadMgr] Final is connection node");

                RoadBuildDir edgeDir = CalcBuildDirByPos(prevPos, currPos);
                float elevationDelta = CalcElevationDeltaByPos(prevPos, currPos);

                // add an edge from previous road forward to this road
                RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                RoadSegment prevSegment = m_stagedSegments.Count > 0 ? m_stagedSegments[m_stagedSegments.Count - 1] : GridMgr.RoadAtPos(m_tracedTiles[0].transform.position);
                prevSegment.ActivateEdge(reverseDir, -elevationDelta);
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
            Debug.Log("[RoadMgr] Unstaging at index " + i);
            RoadSegment toUnstage = m_stagedSegments[i];
            Destroy(toUnstage.gameObject);
            m_stagedSegments.RemoveAt(i);
        }

        // Direction is from perspective of the currSegment
        private RoadBuildDir CalcBuildDirByPos(Vector3 prevSegmentPos, Vector3 currSegmentPos) {
            Vector3 dirVector = (prevSegmentPos - currSegmentPos).normalized;

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
            return prevSegmentPos.y - currSegmentPos.y;
        }

        #endregion // Helpers

        #region External

        public GameObject GetRoadSegmentPrefab() {
            return m_roadSegmentPrefab;
        }

        #endregion // External
    }
}
