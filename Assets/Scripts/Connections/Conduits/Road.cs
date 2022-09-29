using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala
{
    public class Road : MonoBehaviour
    {
        private List<ConnectionNode> m_end1Nodes; // arbitrary start
        private List<ConnectionNode> m_end2Nodes; // arbitrary end

        //private List<RoadSegment> m_segments;
        private List<Tile> m_segments;

        public void SetStartConnectionNodes(List<ConnectionNode> nodes) {
            m_end1Nodes = nodes;
        }

        public void SetEndConnectionNodes(List<ConnectionNode> nodes) {
            m_end2Nodes = nodes;
        }

        public void SetSegments(List<Tile> segments) {
            m_segments = segments;
        }
    }
}
