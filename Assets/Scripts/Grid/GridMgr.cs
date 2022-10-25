using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    public class GridMgr : MonoBehaviour
    {
        private static List<Tile> AllTiles;

        public static void Init() {
            AllTiles = new List<Tile>();
        }

        public static void TrackTile(Tile toTrack) {
            AllTiles.Add(toTrack);
        }

        public static void UntrackTile(Tile toUntrack) {
            AllTiles.Remove(toUntrack);
        }

        public static List<Tile> GetAllTiles() {
            return AllTiles;
        }

        public static Zavala.Tiles.Tile OverTile(Vector2 pos) {
            Ray ray;
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                return hit.collider.gameObject.GetComponent<Tile>();
            }
            else {
                return null;
            }
        }

        public static Inspectable OverInspectable(Vector2 pos) {
            // Look for inspectable
            Ray ray;
            RaycastHit[] hits;

            ray = Camera.main.ScreenPointToRay(pos);
            hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Inspect"));
            foreach (RaycastHit inspectHit in hits) {
                if (inspectHit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return inspectHit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            // look on tiles
            RaycastHit hit;

            ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                if (hit.collider.gameObject.GetComponent<Inspectable>() != null) {
                    return hit.collider.gameObject.GetComponent<Inspectable>();
                }
            }

            return null;
        }

        public static Zavala.Tiles.Tile TileAtPos(Vector3 pos) {
            // raise ray and point downward
            RaycastHit hit;
            Vector3 origin = pos + Vector3.up * 50; // arbitrary height above all tiles
            Vector3 rayDir = (pos - origin).normalized;
            if (Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                return hit.collider.gameObject.GetComponent<Tile>();
            }

            return null;
        }

        public static RoadSegment RoadAtPos(Vector3 pos) {
            // raise ray and point downward
            RaycastHit hit;
            Vector3 origin = pos + Vector3.up * 50; // arbitrary height above all tiles
            Vector3 rayDir = (pos - origin).normalized;
            if (Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Road"))) {
                return hit.collider.gameObject.GetComponent<RoadSegment>();
            }

            return null;
        }

        public static List<Tile> GetAdjTiles(Tile centerTile) {
            List<Tile> adjTiles = new List<Tile>();

            if (centerTile == null) {
                return adjTiles;
            }

            // raycast in 6 directions
            Vector3 startPos = centerTile.transform.position;
            Vector3 adjPos = startPos;
            RaycastHit hit;

            for (int dir = 0; dir < 6; dir++) {
                switch (dir) {
                    case 0:
                        // up
                        adjPos = startPos + new Vector3(1f, 0f, 0f);
                        break;
                    case 1:
                        // up-right
                        adjPos = startPos + new Vector3(0.5f, 0f, -0.875f);
                        break;
                    case 2:
                        // down-right
                        adjPos = startPos + new Vector3(-0.5f, 0f, -0.875f);
                        break;
                    case 3:
                        // down
                        adjPos = startPos + new Vector3(-1f, 0f, 0f);
                        break;
                    case 4:
                        // down-left
                        adjPos = startPos + new Vector3(-0.5f, 0, 0.875f);
                        break;
                    case 5:
                        // up-left
                        adjPos = startPos + new Vector3(0.5f, 0, 0.875f);
                        break;
                    default:
                        break;
                }
                // raise ray and point downward
                Vector3 origin = adjPos + Vector3.up * 50; // arbitrary height above all tiles
                Vector3 rayDir = (adjPos - origin).normalized;
                if (Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                    adjTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
                }
            }

            return adjTiles;
        }

        public static int GetEmptyDir(Tile centerTile) {
            List<int> emptyDirs = new List<int>();

            // raycast in 6 directions
            Vector3 startPos = centerTile.transform.position;
            Vector3 adjPos = startPos;
            RaycastHit hit;

            for (int dir = 0; dir < 6; dir++) {
                switch (dir) {
                    case 0:
                        // up
                        adjPos = startPos + new Vector3(1f, 0f, 0f);
                        break;
                    case 1:
                        // up-right
                        adjPos = startPos + new Vector3(0.5f, 0f, -0.875f);
                        break;
                    case 2:
                        // down-right
                        adjPos = startPos + new Vector3(-0.5f, 0f, -0.875f);
                        break;
                    case 3:
                        // down
                        adjPos = startPos + new Vector3(-1f, 0f, 0f);
                        break;
                    case 4:
                        // down-left
                        adjPos = startPos + new Vector3(-0.5f, 0, 0.875f);
                        break;
                    case 5:
                        // up-left
                        adjPos = startPos + new Vector3(0.5f, 0, 0.875f);
                        break;
                    default:
                        break;
                }
                // raise ray and point downward
                Vector3 origin = adjPos + Vector3.up * 50; // arbitrary height above all tiles
                Vector3 rayDir = (adjPos - origin).normalized;
                if (!Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Tile"))) {
                    emptyDirs.Add(dir);
                }
            }

            // return a random empty direction
            return emptyDirs[Random.Range(0, emptyDirs.Count)];
        }

        public static List<ConnectionNode> ConnectingNodesAdjToTile(Tile queryTile) {
            List<Tile> adjTiles = GridMgr.GetAdjTiles(queryTile);
            List<ConnectionNode> adjNodes = new List<ConnectionNode>();

            for (int i = 0; i < adjTiles.Count; i++) {
                if (adjTiles[i].gameObject.GetComponent<ConnectionNode>() != null) {
                    adjNodes.Add(adjTiles[i].GetComponent<ConnectionNode>());
                }
                List<AddOn> tileAddOns = adjTiles[i].GetAddOns();
                for (int a = 0; a < tileAddOns.Count; a++) {
                    if (tileAddOns[a].GetComponent<ConnectionNode>() != null) {
                        adjNodes.Add(tileAddOns[a].GetComponent<ConnectionNode>());
                    }
                }
            }

            return adjNodes;
        }

        public static List<ConnectionNode> ConnectingNodesAdjToPos(Tile queryTile) {
            List<Tile> adjTiles = GridMgr.GetAdjTiles(queryTile);
            List<ConnectionNode> adjNodes = new List<ConnectionNode>();

            for (int i = 0; i < adjTiles.Count; i++) {
                if (adjTiles[i].gameObject.GetComponent<ConnectionNode>() != null) {
                    adjNodes.Add(adjTiles[i].GetComponent<ConnectionNode>());
                }
                List<AddOn> tileAddOns = adjTiles[i].GetAddOns();
                for (int a = 0; a < tileAddOns.Count; a++) {
                    if (tileAddOns[a].GetComponent<ConnectionNode>() != null) {
                        adjNodes.Add(tileAddOns[a].GetComponent<ConnectionNode>());
                    }
                }
            }

            return adjNodes;
        }

        public static List<RoadSegment> AdjRoadSegments(Tile queryTile, List<RoadSegment> ignoreList = null) {
            List<RoadSegment> adjSegments = new List<RoadSegment>();

            // raycast in 6 directions
            Vector3 startPos = queryTile.transform.position;
            Vector3 adjPos = startPos;
            RaycastHit hit;

            for (int dir = 0; dir < 6; dir++) {
                switch (dir) {
                    case 0:
                        // up
                        adjPos = startPos + new Vector3(1f, 0f, 0f);
                        break;
                    case 1:
                        // up-right
                        adjPos = startPos + new Vector3(0.5f, 0f, -0.875f);
                        break;
                    case 2:
                        // down-right
                        adjPos = startPos + new Vector3(-0.5f, 0f, -0.875f);
                        break;
                    case 3:
                        // down
                        adjPos = startPos + new Vector3(-1f, 0f, 0f);
                        break;
                    case 4:
                        // down-left
                        adjPos = startPos + new Vector3(-0.5f, 0, 0.875f);
                        break;
                    case 5:
                        // up-left
                        adjPos = startPos + new Vector3(0.5f, 0, 0.875f);
                        break;
                    default:
                        break;
                }
                // raise ray and point downward
                Vector3 origin = adjPos + Vector3.up * 50; // arbitrary height above all tiles
                Vector3 rayDir = (adjPos - origin).normalized;
                if (Physics.Raycast(origin, rayDir, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Road"))) {
                    // ignore the road segments currently being staged
                    if (ignoreList == null || !ignoreList.Contains(hit.collider.GetComponent<RoadSegment>())) {
                        adjSegments.Add(hit.collider.GetComponent<RoadSegment>());
                    }
                }
            }

            return adjSegments;
        }

        public static Tile GetRandomTile(bool mustBeBuildable) {
            List<Tile> candidates = new List<Tile>();
            for (int i = 0; i < AllTiles.Count; i++) {
                if (mustBeBuildable) {
                    if ((AllTiles[i].GetAddOns().Count == 0)
                        && AllTiles[i].GetComponent<BlocksBuild>() == null
                        && AllTiles[i].GetComponent<Water>() == null
                        && RoadAtPos(AllTiles[i].transform.position) == null) {
                        candidates.Add(AllTiles[i]);
                    }
                }
                else {
                    candidates.Add(AllTiles[i]);
                }
            }

            if (candidates.Count == 0) {
                Debug.Log("[GridMgr] No buildable tiles. returning null.");
                return null;
            }

            int randIndex = Random.Range(0, candidates.Count);

            return candidates[randIndex];
        }

        public static Tile GetRandomBoundaryTile() {
            List<Tile> candidates = new List<Tile>();
            for (int i = 0; i < AllTiles.Count; i++) {
                if (GetAdjTiles(AllTiles[i]).Count < 6) {
                    candidates.Add(AllTiles[i]);
                }
            }

            if (candidates.Count == 0) {
                Debug.Log("[GridMgr] No appendable tiles. returning null.");
                return null;
            }

            int randIndex = Random.Range(0, candidates.Count);

            return candidates[randIndex];
        }

        public static void ReplaceTile(Tile toReplace, Tile newTile) {
            AllTiles.Remove(toReplace);
            newTile.transform.position = toReplace.transform.position;
            AllTiles.Add(newTile);
            Destroy(toReplace.gameObject);
        }

        public static void AppendToTile(Tile toAppend, Tile newTile) {
            // find adj position
            int emptyDir = GetEmptyDir(toAppend);
            Vector3 adjPos = Vector3.zero;
            Vector3 startPos = toAppend.transform.position;
            switch(emptyDir) {
                case 0:
                    // up
                    adjPos = startPos + new Vector3(1f, 0f, 0f);
                    break;
                case 1:
                    // up-right
                    adjPos = startPos + new Vector3(0.5f, 0f, -0.875f);
                    break;
                case 2:
                    // down-right
                    adjPos = startPos + new Vector3(-0.5f, 0f, -0.875f);
                    break;
                case 3:
                    // down
                    adjPos = startPos + new Vector3(-1f, 0f, 0f);
                    break;
                case 4:
                    // down-left
                    adjPos = startPos + new Vector3(-0.5f, 0, 0.875f);
                    break;
                case 5:
                    // up-left
                    adjPos = startPos + new Vector3(0.5f, 0, 0.875f);
                    break;
                default:
                    break;
            }

            if (adjPos == Vector3.zero) {
                Debug.Log("[GridMgr] Failed to find an empty adjacent dir, despite guarantee that empty adj dir exists. Canceling appending generation");
                Destroy(newTile.gameObject);
                return;
            }

            newTile.transform.position = adjPos;
            AllTiles.Add(newTile);
        }
    }
}