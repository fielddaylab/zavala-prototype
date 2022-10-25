using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;
using Zavala.Roads;

namespace Zavala
{
    [RequireComponent(typeof(Inspectable))]
    public class RoadSegment : MonoBehaviour
    {
        private Inspectable m_inspectComponent;

        [SerializeField] private GameObject m_rampPrefab, m_flatPrefab;

        private EdgeSegment[] m_edges;

        private static int NUM_HEX_EDGES = 6;

        private void Awake() {
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_edges = new EdgeSegment[NUM_HEX_EDGES];
        }

        private void Start() {
            m_inspectComponent.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="elevationDelta">how much higher the connected tile is than this center segments</param>
        public void ActivateEdge(RoadBuildDir dir, float elevationDelta) {
            int sideIndex = 0;
            switch (dir) {
                case RoadBuildDir.Up:
                    break;
                case RoadBuildDir.Up_Right:
                    sideIndex = 1;
                    break;
                case RoadBuildDir.Down_Right:
                    sideIndex = 2;
                    break;
                case RoadBuildDir.Down:
                    sideIndex = 3;
                    break;
                case RoadBuildDir.Down_Left:
                    sideIndex = 4;
                    break;
                case RoadBuildDir.Up_Left:
                    sideIndex = 5;
                    break;
                default:
                    break;
            }

            GameObject newEdgeObj = elevationDelta > 0 ? m_rampPrefab : m_flatPrefab;
            if (m_edges[sideIndex] != null) {
                // remove old prefab
                Destroy(m_edges[sideIndex].gameObject);
            }
            else {

            }
            // Instantiate new prefab
            EdgeSegment newEdge = Instantiate(newEdgeObj, this.transform).GetComponent<EdgeSegment>();
            m_edges[sideIndex] = newEdge;
            newEdge.RotateEdge(dir);
            if (elevationDelta > 0) {
                newEdge.ScaleEdge(elevationDelta);
            }
        }

        public void DeactivateEdge(RoadBuildDir dir) {
            int sideIndex = 0;
            switch (dir) {
                case RoadBuildDir.Up:
                    break;
                case RoadBuildDir.Up_Right:
                    sideIndex = 1;
                    break;
                case RoadBuildDir.Down_Right:
                    sideIndex = 2;
                    break;
                case RoadBuildDir.Down:
                    sideIndex = 3;
                    break;
                case RoadBuildDir.Down_Left:
                    sideIndex = 4;
                    break;
                case RoadBuildDir.Up_Left:
                    sideIndex = 5;
                    break;
                default:
                    break;
            }

            if (m_edges[sideIndex] != null) {
                // remove old prefab
                Destroy(m_edges[sideIndex].gameObject);
            }
            else {
                // No action necessary -- nothing at that edge
            }
        }
    }
}