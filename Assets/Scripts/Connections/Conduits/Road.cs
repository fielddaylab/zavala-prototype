using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala
{
    public class Road : MonoBehaviour
    {
        private List<Tile> m_end1Nodes;
        private List<Tile> m_end2Nodes;

        private List<RoadSegment> m_segments;
    }
}
