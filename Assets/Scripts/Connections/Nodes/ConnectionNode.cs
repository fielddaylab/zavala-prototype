using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class ConnectionNode : MonoBehaviour
    {
        private List<Road> m_connectedRoads;

        public event EventHandler NodeEconomyUpdated;

        private void Awake() {
            m_connectedRoads = new List<Road>();
        }

        private void OnDisable() {
            foreach(Road road in m_connectedRoads) {
                if (road != null) {
                    road.EconomyUpdated -= HandleEconomyUpdated;
                }
            }
        }

        public void AddRoad(Road road) {
            m_connectedRoads.Add(road);
            road.EconomyUpdated += HandleEconomyUpdated;
        }

        public void RemoveRoad(Road road) {
            m_connectedRoads.Remove(road);
            road.EconomyUpdated -= HandleEconomyUpdated;
        }

        public List<Road> GetConnectedRoads() {
            return m_connectedRoads;
        }

        // From node to roads
        public void UpdateNodeEconomy() {
            for(int i = 0; i < m_connectedRoads.Count; i++) {
                m_connectedRoads[i].UpdateEconomy();
            }
        }

        public void RemoveFromRoad() {
            foreach(Road road in m_connectedRoads) {
                road.RemoveConnection(this);
            }
        }

        #region Handlers

        // from road to nodes
        private void HandleEconomyUpdated(object sender, EventArgs args) {
            NodeEconomyUpdated?.Invoke(sender, args);
        }

        #endregion //Handlers
    }
}
