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
            m_segments = new List<Tile>();

            for (int i = 0; i < segments.Count; i++) {
                m_segments.Add(segments[i]);
            }
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
        public bool ResourceOnRoad(Resources.Type resourceType, GameObject requester) {
            if (ResourceInList(m_end1Nodes, resourceType, requester)) {
                return true;
            }
            if (ResourceInList(m_end2Nodes, resourceType, requester)) {
                return true;
            }

            return false;
        }

        public StoresProduct GetSupplierOnRoad(Resources.Type resourceType) {
            StoresProduct supplier = GetSupplierInList(m_end1Nodes, resourceType);
            if (supplier != null) {
                return supplier;
            }
            supplier = GetSupplierInList(m_end2Nodes, resourceType);
            if (supplier != null) {
                return supplier;
            }

            return null;
        }

        public Tile GetTileAtIndex(int index) {
            return m_segments[index];
        }

        private bool ResourceInList(List<ConnectionNode> nodeList, Resources.Type resourceType, GameObject requester) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent != null && storeComponent.StorageContains(resourceType) && storeComponent.gameObject != requester) {
                    return true;
                }
            }
            return false;
        }

        private StoresProduct GetSupplierInList(List<ConnectionNode> nodeList, Resources.Type resourceType) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent != null && storeComponent.StorageContains(resourceType)) {
                    return storeComponent;
                }
            }
            return null;
        }

        private bool IsSupplierInList(List<ConnectionNode> nodeList, StoresProduct supplier) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent == supplier) {
                    return true;
                }
            }
            return false;
        }

        private bool IsRecipientInList(List<ConnectionNode> nodeList, Requests recipient) {
            for (int i = 0; i < nodeList.Count; i++) {
                Requests requestComponent = nodeList[i].gameObject.GetComponent<Requests>();
                if (requestComponent == recipient) {
                    return true;
                }
            }
            return false;
        }

        #endregion //  Queries

        #region Triggers 

        public void UpdateEconomy() {
            EconomyUpdated.Invoke(this, EventArgs.Empty);
        }

        public bool TrySummonTruck(Resources.Type resourceType, StoresProduct supplier, Requests recipient) {
            if (supplier.TryRemoveFromStorage(resourceType)) {
                // send to recipient

                // find start tile
                Transform startTransform = m_segments[GetStartIndex(supplier)].gameObject.transform;
                Truck newTruck = Instantiate(GameDB.Instance.TruckPrefab).GetComponent<Truck>();
                newTruck.Init(resourceType, supplier, recipient, this);
                return true;
            }
            else {
                return false;
            }
        }

        #endregion // Triggers

        public int GetStartIndex(StoresProduct supplier) {
            if (IsSupplierInList(m_end1Nodes, supplier)) {
                return 0;
            }
            else {
                return m_segments.Count - 1;
            }
        }

        public int GetEndIndex(Requests recipient) {
            if (IsRecipientInList(m_end1Nodes, recipient)) {
                return 0;
            }
            else {
                return m_segments.Count - 1;
            }
        }
    }
}
