using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;

namespace Zavala
{
    public class ConnectionNode : MonoBehaviour
    {
        private List<RoadSegment> m_connectedRoads;

        private void Awake() {
            m_connectedRoads = new List<RoadSegment>();
        }

        public void AddRoad(RoadSegment road) {
            m_connectedRoads.Add(road);

            Debug.Log("[ConnectionNode] Road added! Num roads connected: " + m_connectedRoads.Count);
        }

        public void RemoveRoad(RoadSegment road) {
            m_connectedRoads.Remove(road);
         
            Debug.Log("[ConnectionNode] Road removed! Num roads connected: " + m_connectedRoads.Count);
        }

        public List<RoadSegment> GetConnectedRoads() {
            return m_connectedRoads;
        }

        // From node to roads
        public void UpdateNodeEconomy() {
            EventMgr.Instance.TriggerEvent(Events.ID.EconomyUpdated, new EconomyUpdatedEventArgs(RegionMgr.Instance.GetRegionByPos(this.transform.position)));
        }
    }
}
