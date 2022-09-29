using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    public class Road : MonoBehaviour
    {
        private List<ConnectionNode> m_end1Nodes; // arbitrary start
        private List<ConnectionNode> m_end2Nodes; // arbitrary end

        //private List<RoadSegment> m_segments;
        private List<Tile> m_segments;

        public event EventHandler EconomyUpdated;

        #region Road Creation

        public void SetStartConnectionNodes(List<ConnectionNode> nodes) {
            m_end1Nodes = nodes;
        }

        public void SetEndConnectionNodes(List<ConnectionNode> nodes) {
            m_end2Nodes = nodes;
        }

        public void SetSegments(List<Tile> segments) {
            m_segments = segments;
        }

        public void FinalizeConnections() {
            FinalizeConnectionsInList(m_end1Nodes);
            FinalizeConnectionsInList(m_end2Nodes);

            UpdateEconomy();
        }

        private void FinalizeConnectionsInList(List<ConnectionNode> nodeList) {
            for (int i = 0; i < nodeList.Count; i++) {
                nodeList[i].AddRoad(this);
            }
        }

        #endregion // Road Creation

        #region Queries

        // Wether a connected node has the specified resource in storage
        public bool ResourceOnRoad(Resources.Type resourceType) {
            if (ResourceInList(m_end1Nodes, resourceType)) {
                return true;
            }
            if (ResourceInList(m_end2Nodes, resourceType)) {
                return true;
            }

            return false;
        }

        public StoresProduct GetSupplierOnRoad(Resources.Type resourceType) {
            StoresProduct supplier = SupplierInList(m_end1Nodes, resourceType);
            if (supplier != null) {
                return supplier;
            }
            supplier = SupplierInList(m_end2Nodes, resourceType);
            if (supplier != null) {
                return supplier;
            }

            return null;
        }

        private bool ResourceInList(List<ConnectionNode> nodeList, Resources.Type resourceType) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent != null && storeComponent.StorageContains(resourceType)) {
                    return true;
                }
            }
            return false;
        }

        private StoresProduct SupplierInList(List<ConnectionNode> nodeList, Resources.Type resourceType) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent != null && storeComponent.StorageContains(resourceType)) {
                    return storeComponent;
                }
            }
            return null;
        }

        #endregion //  Queries

        #region Triggers 

        public void UpdateEconomy() {
            EconomyUpdated.Invoke(this, EventArgs.Empty);
        }

        #endregion // Triggers
    }
}
