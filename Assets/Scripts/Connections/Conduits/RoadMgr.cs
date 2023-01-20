using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using Zavala.Events;
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
                Tile startTile = RegionMgr.Instance.CurrRegion.GridMgr.OverTile(Input.mousePosition);
                if (startTile == null ||
                    (startTile.GetComponent<ConnectionNode>() == null
                    && !startTile.ConnectionInAddOns()
                    && RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(startTile.transform.position) == null)) {
                    return;
                }

                bool validStart = true;

                if (validStart) {
                    // if so, save tile as starting node and start tracking
                    m_startedRoad = true;
                    Debug.Log("[Instantiate] Instantiating road prefab");
                    Tile currTile = RegionMgr.Instance.CurrRegion.GridMgr.OverTile(Input.mousePosition);
                    m_tracedTiles.Add(currTile);
                    m_lastKnownTile = currTile;

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
                Tile currTile = RegionMgr.Instance.CurrRegion.GridMgr.OverTile(Input.mousePosition);
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
                        if (currTile.GetComponent<ConnectionNode>() != null || currTile.ConnectionInAddOns() || RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(currTile.transform.position) != null) {
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
            if (m_tracedTiles.Count == 1) {
                // cannot connect a single tile
                return false;
            }

            Tile endTile = RegionMgr.Instance.CurrRegion.GridMgr.OverTile(Input.mousePosition);
            ConnectionNode endConnection = endTile.GetComponent<ConnectionNode>();
            RoadSegment endRoad = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(endTile.transform.position);
            if (endConnection == null
                && !endTile.ConnectionInAddOns()
                && (endRoad == null || m_stagedSegments.Contains(endRoad))) {
                // road must end on a connection or an existing road
                return false;
            }
            RoadSegment startRoad = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(m_tracedTiles[0].transform.position);
            if (startRoad == null && endRoad == null && m_stagedSegments.Count == 0) {
                // there must be a road segment in at least one of the start, middle, and end
                return false;
            }

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
            Debug.Log("[RoadMgr] Finalizing road");
            // check if start is a road
            Vector3 firstPos = m_tracedTiles[0].transform.position;
            RoadSegment firstRoad = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(firstPos);
            if (firstRoad != null) {
                Debug.Log("[RoadMgr] First is road");
                // road -- generate edge in direction of first segment
                GameObject secondObj = m_stagedSegments.Count > 0 ? m_stagedSegments[0].gameObject : m_tracedTiles[m_tracedTiles.Count - 1].gameObject;
                RoadBuildDir dir = CalcBuildDirByPos(secondObj.transform.position, firstPos);
                float elevationDelta = CalcElevationDeltaByPos(transform.position, firstPos);
                firstRoad.ActivateEdge(dir, elevationDelta, secondObj);
            }
            // check if start is connection node
            Tile firstTile = RegionMgr.Instance.CurrRegion.GridMgr.TileAtPos(firstPos);
            List<ConnectionNode> firstConnectionNodes = firstTile.GetAllConnectionNodes();
            if (firstConnectionNodes.Count > 0) {
                Debug.Log("[RoadMgr] First is connection node");
                RoadSegment secondSegment = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(m_tracedTiles[1].transform.position);
                if (secondSegment != null) {
                    // Have this connection node track the adj road outlet
                    for (int n = 0; n < firstConnectionNodes.Count; n++) {
                        Debug.Log("[RoadMgr] Adding road to node " + firstConnectionNodes[n]);
                        firstConnectionNodes[n].AddRoad(secondSegment);
                    }
                }
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

                GameObject prevObj;
                if (segIndex == 0) {
                    if (firstRoad != null) {
                        prevObj = firstRoad.gameObject;
                    }
                    else {
                        prevObj = m_tracedTiles[0].gameObject;
                    }
                }
                else {
                    prevObj = m_stagedSegments[segIndex - 1].gameObject;
                }
                currSegment.ActivateEdge(edgeDir, elevationDelta, prevObj);

                if (segIndex != 0) {
                    // add an edge from previous road forward to this road
                    RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                    RoadSegment prevSegment = m_stagedSegments[segIndex - 1];
                    prevSegment.ActivateEdge(reverseDir, -elevationDelta, currSegment.gameObject);
                }
            }

            // check end
            Tile endTile = m_tracedTiles[m_tracedTiles.Count - 1];
            Vector3 endPos = endTile.transform.position;
            RoadSegment endSegment = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(endPos);
            prevPos = currPos;
            currPos = endPos;
            // if road
            if (endSegment != null) {
                Debug.Log("[RoadMgr] Final is road");
                // add an edge from this road back to previous road
                RoadBuildDir edgeDir = CalcBuildDirByPos(prevPos, currPos);
                float elevationDelta = CalcElevationDeltaByPos(prevPos, currPos);
                GameObject prevObj = m_tracedTiles[m_tracedTiles.Count - 2].gameObject;
                if (RegionMgr.Instance.GetRegionByPos(prevObj.transform.position).GridMgr.RoadAtPos(prevObj.transform.position) != null) {
                    prevObj = RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(prevObj.transform.position).gameObject;
                }
                endSegment.ActivateEdge(edgeDir, elevationDelta, prevObj);

                if (m_stagedSegments.Count > 0) {
                    // add an edge from previous road forward to this road
                    RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                    RoadSegment prevSegment = m_stagedSegments[m_stagedSegments.Count - 1];
                    prevSegment.ActivateEdge(reverseDir, -elevationDelta, endSegment.gameObject);
                }
            }
            // if connection node
            List<ConnectionNode> finalConnectionNodes = m_tracedTiles[m_tracedTiles.Count - 1].GetAllConnectionNodes();
            if (finalConnectionNodes.Count > 0) {
                Debug.Log("[RoadMgr] Final is connection node");

                RoadBuildDir edgeDir = CalcBuildDirByPos(prevPos, currPos);
                float elevationDelta = CalcElevationDeltaByPos(prevPos, currPos);

                // add an edge from previous road forward to this road
                RoadBuildDir reverseDir = (RoadBuildDir)(((int)edgeDir + 3) % 6);

                RoadSegment prevSegment = m_stagedSegments.Count > 0 ? m_stagedSegments[m_stagedSegments.Count - 1] : RegionMgr.Instance.CurrRegion.GridMgr.RoadAtPos(m_tracedTiles[0].transform.position);
                prevSegment.ActivateEdge(reverseDir, -elevationDelta, endTile.gameObject);

                // Have connection node track this road outlet
                for (int n = 0; n < finalConnectionNodes.Count; n++) {
                    Debug.Log("[RoadMgr] Adding road to node " + finalConnectionNodes[n]);
                    finalConnectionNodes[n].AddRoad(prevSegment);
                }
            }

            Debug.Log("[InteractMgr] Road saved!");

            // Update economy
            EventMgr.Instance.TriggerEvent(ID.EconomyUpdated, new EconomyUpdatedEventArgs(RegionMgr.Instance.GetRegionByPos(endPos)));
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

        #region Pathfinding

        public bool QueryRoadForResource(GameObject requester, RoadSegment road, Resources.Type resourceType, int desiredUnits, out List<RoadSegment> path, out StoresProduct supplier, out Resources.Type foundResourceType, out int foundUnits) {
            // look for a node with the requested resource
            path = AStarPathToResource(requester, road, resourceType, desiredUnits, out supplier, out foundResourceType, out foundUnits);

            return path.Count != 0;
        }

        public bool QueryRoadForConnection(ConnectionNode origin, ConnectionNode endpoint, RoadSegment road, out List<RoadSegment> path) {
            // look for the endpoint
            path = AStarPathToConnection(origin, endpoint, road);

            return path.Count != 0;
        }

        private List<RoadSegment> ReconstructPath(Dictionary<RoadSegment, RoadSegment> cameFrom, RoadSegment current) {
            List<RoadSegment> totalPath = new List<RoadSegment>();
            totalPath.Add(current);
            Debug.Log("[A*] Path reconstruct start with " + current);

            while (cameFrom.Keys.Contains(current)) {
                current = cameFrom[current];
                totalPath.Add(current);
                Debug.Log("[A*] Adding " + current);
            }

            Debug.Log("[A*] Path reconstructing finished with length " + totalPath.Count);

            return totalPath;
        }

        private float Heuristic(Vector3 start, Vector3 curr) {
            // returns a lower value for closer to destination
            return Vector3.Distance(start, curr);
        }

        private List<RoadSegment> AStarPathToResource(GameObject requester, RoadSegment start, Resources.Type resourceType, int desiredUnits, out StoresProduct supplier, out Resources.Type foundResourceType, out int foundUnits) {

            List<RoadSegment> finalPath = new List<RoadSegment>();

            // The set of discovered nodes that may need to be (re-)expanded.
            // Initially, only the start node is known.
            // This is usually implemented as a min-heap or priority queue rather than a hash-set.
            PriorityQueue<RoadSegment, float> openSet = new PriorityQueue<RoadSegment, float>();
            List<RoadSegment> openSetKeys = new List<RoadSegment>();
            if (start.IsUsable()) {
                openSet.Enqueue(start, 0);
                openSetKeys.Add(start);
            }

            // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
            // to n currently known.
            Dictionary<RoadSegment, RoadSegment> cameFrom = new Dictionary<RoadSegment, RoadSegment>();

            // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
            Dictionary<RoadSegment, float> gScore = new Dictionary<RoadSegment, float>();
            gScore[start] = 0;

            // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
            // how cheap a path could be from start to finish if it goes through n.
            Dictionary<RoadSegment, float> fScore = new Dictionary<RoadSegment, float>();
            fScore[start] = Heuristic(start.transform.position, start.transform.position);

            while (openSet.Count > 0) {
                RoadSegment current = openSet.Dequeue();
                openSetKeys.Remove(current);
                if (current.ResourceInEdges(resourceType, requester, desiredUnits, out supplier, out foundResourceType, out foundUnits)) {
                    finalPath = ReconstructPath(cameFrom, current);
                    return finalPath;
                }

                List<RoadSegment> connectedRoads = current.GetConnectedRoads();
                for (int r = 0; r < connectedRoads.Count; r++) {
                    RoadSegment neighbor = connectedRoads[r];
                    if (!gScore.ContainsKey(neighbor)) {
                        gScore.Add(neighbor, int.MaxValue);
                    }
                    if (!fScore.ContainsKey(neighbor)) {
                        fScore.Add(neighbor, int.MaxValue);
                    }
                    // d(current,neighbor) is the weight of the edge from current to neighbor
                    // tentative_gScore is the distance from start to the neighbor through current
                    float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    if (tentativeGScore < gScore[neighbor]) {
                        // This path to neighbor is better than any previous one. Record it!
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        float hNeighbor = Heuristic(start.transform.position, neighbor.transform.position);
                        fScore[neighbor] = tentativeGScore + hNeighbor;
                        if (!openSetKeys.Contains(neighbor)) {
                            if (neighbor.IsUsable()) {
                                openSet.Enqueue(neighbor, hNeighbor);
                                openSetKeys.Add(neighbor);
                            }
                        }
                    }
                }
            }

            foundResourceType = Resources.Type.None;
            supplier = null;
            foundUnits = 0;
            return finalPath;
        }

        private List<RoadSegment> AStarPathToConnection(ConnectionNode origin, ConnectionNode endpoint, RoadSegment start) {

            List<RoadSegment> finalPath = new List<RoadSegment>();

            // The set of discovered nodes that may need to be (re-)expanded.
            // Initially, only the start node is known.
            // This is usually implemented as a min-heap or priority queue rather than a hash-set.
            PriorityQueue<RoadSegment, float> openSet = new PriorityQueue<RoadSegment, float>();
            List<RoadSegment> openSetKeys = new List<RoadSegment>();
            if (start.IsUsable()) {
                openSet.Enqueue(start, 0);
                openSetKeys.Add(start);
            }

            // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
            // to n currently known.
            Dictionary<RoadSegment, RoadSegment> cameFrom = new Dictionary<RoadSegment, RoadSegment>();

            // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
            Dictionary<RoadSegment, float> gScore = new Dictionary<RoadSegment, float>();
            gScore[start] = 0;

            // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
            // how cheap a path could be from start to finish if it goes through n.
            Dictionary<RoadSegment, float> fScore = new Dictionary<RoadSegment, float>();
            fScore[start] = Heuristic(start.transform.position, start.transform.position);

            while (openSet.Count > 0) {
                RoadSegment current = openSet.Dequeue();
                openSetKeys.Remove(current);
                if (current.ConnectionNodeInEdges(endpoint)) {
                    finalPath = ReconstructPath(cameFrom, current);
                    finalPath.Reverse();
                    return finalPath;
                }

                List<RoadSegment> connectedRoads = current.GetConnectedRoads();
                for (int r = 0; r < connectedRoads.Count; r++) {
                    RoadSegment neighbor = connectedRoads[r];
                    if (!gScore.ContainsKey(neighbor)) {
                        gScore.Add(neighbor, int.MaxValue);
                    }
                    if (!fScore.ContainsKey(neighbor)) {
                        fScore.Add(neighbor, int.MaxValue);
                    }
                    // d(current,neighbor) is the weight of the edge from current to neighbor
                    // tentative_gScore is the distance from start to the neighbor through current
                    float tentativeGScore = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                    if (tentativeGScore < gScore[neighbor]) {
                        // This path to neighbor is better than any previous one. Record it!
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        float hNeighbor = Heuristic(start.transform.position, neighbor.transform.position);
                        fScore[neighbor] = tentativeGScore + hNeighbor;
                        if (!openSetKeys.Contains(neighbor)) {
                            if (neighbor.IsUsable()) {
                                openSet.Enqueue(neighbor, hNeighbor);
                                openSetKeys.Add(neighbor);
                            }
                        }
                    }
                }
            }

            return finalPath;
        }

        #endregion // Pathfinding

        #region Truck Summons

        public bool TrySummonTruck(Resources.Type resourceType, int units, List<RoadSegment> path, StoresProduct supplier, Requests recipient) {
            if (supplier.TryRemoveFromStorage(resourceType, units)) {
                // send to recipient

                // find start tile
                Transform startTransform = path[0].gameObject.transform;
                Debug.Log("[Instantiate] Instantiating truck prefab");
                Truck newTruck = Instantiate(GameDB.Instance.TruckPrefab).GetComponent<Truck>();
                newTruck.Init(resourceType, units, path, supplier, recipient);
                return true;
            }
            else {
                return false;
            }
        }

        #endregion // Truck Summons
    }
}
