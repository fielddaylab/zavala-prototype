﻿using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
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
                switch(dir) {
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

        public static List<ConnectionNode> ConnectingNodesAdj(Vector3 queryPos) {
            Tile currTile = GridMgr.OverTile(queryPos);
            List<Tile> adjTiles = GridMgr.GetAdjTiles(currTile);
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
    }
}