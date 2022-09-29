using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class ConnectionNode : MonoBehaviour
    {
        private List<Road> m_connectedRoads;

        private void Awake() {
            m_connectedRoads = new List<Road>();
        }

        public void AddRoad(Road road) {
            m_connectedRoads.Add(road);
        }

        public void RemoveRoad(Road road) {
            m_connectedRoads.Remove(road);
        }

        public List<Road> GetConnectedRoads() {
            return m_connectedRoads;
        }
    }
}
