using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Zavala.Events;
using Zavala.Resources;
using Zavala.Roads;

namespace Zavala.Functionalities
{
    public class StoresProduct : MonoBehaviour
    {
        public enum SupplierType
        {
            Unknown,
            GrainFarm,
            DairyFarm,
            Digester
            // Storage
            // Skimmer
        }

        public struct StoredProduct
        {
            public Resources.Type Type;
            public UIStoredProduct UI;
            public int Units;

            public StoredProduct(Resources.Type type, UIStoredProduct ui, int units) {
                Type = type;
                UI = ui;
                Units = units;

                UI.UpdateUnitsText(Units);
            }
        }

        public int MaxProducts; // the max number of products this can store

        public event EventHandler StorageExceeded; // when a product would be added but there is no room
        public event EventHandler<ResourceEventArgs> RemovedStorage;
        public event EventHandler<ResourceEventArgs> StorageExpired;

        [SerializeField] private bool m_hasTimeout;
        [SerializeField] private int m_storageTimeout; // num Cycles
        [SerializeField] private float m_iconOffsetZ = 0.25f;
        [SerializeField] private bool m_excludeFromClearingHouse;

        public LetItSit SitOption = null; // only for nodes that can let something sit (i.e. dairy farm)

        private List<StoredProduct> m_storageList; // the products in storage

        private Vector3 m_initialQueuePos;

        private SupplierType m_supplierType = SupplierType.Unknown;


        private void OnEnable() {
            m_storageList = new List<StoredProduct>();

            EventMgr.Instance.EconomyUpdated += HandleEconomyUpdated;

            if (SitOption != null) {
                SitOption.Init();
            }
        }

        private void OnDisable() {
            EventMgr.Instance.EconomyUpdated -= HandleEconomyUpdated;
        }

        private void Start() {
            m_initialQueuePos = GameDB.Instance.UIStoredProductPrefab.transform.localPosition;

            if (!m_excludeFromClearingHouse) {
                RegionMgr.Instance.GetRegionByPos(this.transform.position).RegisterWithClearingHouse(this);
            }
        }

        public bool StorageContains(Resources.Type resourceType, int desiredUnits, out Resources.Type foundResourceType, out int foundUnits) {
            Debug.Log("[StoresProduct] Checking if storage contains " + resourceType);
            for (int i = 0; i < m_storageList.Count; i++) {
                Debug.Log("[StoresProduct] store component type: " + m_storageList[i].Type);
                if (m_storageList[i].Type == resourceType) {
                    foundResourceType = resourceType;
                    foundUnits = m_storageList[i].Units > desiredUnits ? desiredUnits : m_storageList[i].Units;
                    if (m_storageList[i].Units >= desiredUnits) {
                        Debug.Log("[StoresProduct] " + foundUnits + " units found");
                    }
                    else if (m_storageList[i].Units <= 0) {
                        // 0 units found
                        Debug.Log("[StoresProduct] found a supplier, but " + m_storageList[i].Units + " units in stock.");
                        return false;
                    }
                    else {
                        Debug.Log("[StoresProduct] resource found, but not enough units to fully complete request (" + m_storageList[i].Units + " units found vs. " + desiredUnits + " requested");
                    }

                    return true;
                }
                // handle SoilEnricher case (Manure OR Fertilizer)
                else if (resourceType == Resources.Type.SoilEnricher) {
                    if (m_storageList[i].Type == Resources.Type.Manure || m_storageList[i].Type == Resources.Type.Fertilizer) {
                        foundResourceType = m_storageList[i].Type;
                        foundUnits = m_storageList[i].Units > desiredUnits ? desiredUnits : m_storageList[i].Units;
                        if (m_storageList[i].Units >= desiredUnits) {
                            Debug.Log("[StoresProduct] " + foundUnits + " units found among a total of " + m_storageList[i].Units);
                        }
                        else if (m_storageList[i].Units <= 0) {
                            // 0 units found
                            Debug.Log("[StoresProduct] found a supplier, but only " + m_storageList[i].Units + " units in stock.");
                            return false;
                        }
                        else {
                            Debug.Log("[StoresProduct] resource found, but not enough units to fully complete request (" + m_storageList[i].Units + " units found vs. " + desiredUnits + " requested");
                        }

                        return true;
                    }
                }
            }
            foundResourceType = Resources.Type.None;
            foundUnits = 0;
            return false;
        }

        public List<Resources.Type> GetStoredResourceTypes() {
            List<Resources.Type> types = new List<Resources.Type>();

            for (int i = 0; i < m_storageList.Count; i++) {
                if (!types.Contains(m_storageList[i].Type)) {
                    types.Add(m_storageList[i].Type);
                }
            }

            return types;
        }

        public int GetIndexOfResource(Resources.Type resourceType) {
            for (int i = 0; i < m_storageList.Count; i++) {
                if (m_storageList[i].Type == resourceType) {
                    return i;
                }
            }

            return -1;
        }

        public bool IsStorageFull() {
            return m_storageList.Count == MaxProducts;
        }

        public int StorageCount() {
            return m_storageList.Count;
        }

        public int StorageCount(Resources.Type queryType) {
            int count = 0;

            for (int i = 0; i < m_storageList.Count; i++) {
                if (m_storageList[i].Type == queryType) {
                    count += m_storageList[i].Units;
                }
            }

            return count;
        }

        public bool TryAddToStorage(Resources.Type productType, int units) {
            if (m_storageList.Count >= MaxProducts) { // <- types of requests, not units
                Debug.Log("[StoresProduct] Storage is full! Not adding to list.");
                StorageExceeded?.Invoke(this, EventArgs.Empty);
                return false;
            }
            else {
                Debug.Log("[StoresProduct] Added to storage list with " + units + " units");

                int resourceIndex = GetIndexOfResource(productType);
                if (resourceIndex == -1) {
                    // create new icon

                    UIStoredProduct newProductUI = Instantiate(GameDB.Instance.UIStoredProductPrefab, this.transform).GetComponent<UIStoredProduct>();
                    if (m_hasTimeout) {
                        newProductUI.Init(productType, m_storageTimeout, this.GetComponent<Cycles>(), units);
                    }
                    else {
                        newProductUI.Init(productType, units);
                    }
                    StoredProduct newProduct = new StoredProduct(productType, newProductUI, units);
                    m_storageList.Add(newProduct);
                    newProductUI.TimerExpired += HandleTimerExpired;

                    RedistributeQueue();
                }
                else {
                    Debug.Log("[StoresProduct] Modifying units of " + productType + " by " + units + " units");
                    // add to existing icon
                    StoredProduct oldItem = m_storageList[resourceIndex];
                    StoredProduct newItem = new StoredProduct(oldItem.Type, oldItem.UI, oldItem.Units + units);
                    m_storageList[resourceIndex] = newItem;
                }

                return true;
            }
        }

        public bool TryRemoveFromStorage(Resources.Type productType, int units) {
            if (m_storageList.Count <= 0) {
                Debug.Log("[StoresProduct] Storage is empty! Not removing from list.");
                // StorageEmpty.Invoke(this, EventArgs.Empty);
                return false;
            }
            else {
                RemoveFromStorageList(productType, units);

                RedistributeQueue();
                RemovedStorage?.Invoke(this, new ResourceEventArgs(productType, units));
                return true;
            }
        }

        private void RemoveFromStorageList(Resources.Type productType, int units) {
            for (int i = 0; i < m_storageList.Count; i++) {
                if (m_storageList[i].Type == productType) {
                    if (units >= m_storageList[i].Units) {
                        Debug.Log("[StoresProduct] Destroying UI: " + m_storageList[i].UI.gameObject.name);

                        Destroy(m_storageList[i].UI.gameObject);
                        m_storageList.RemoveAt(i);
                        Debug.Log("[StoresProduct] Removed from list");
                    }
                    else {
                        StoredProduct oldItem = m_storageList[i];
                        StoredProduct newItem = new StoredProduct(oldItem.Type, oldItem.UI, oldItem.Units - units);
                        m_storageList[i] = newItem;

                        Debug.Log("[StoresProduct] Removed " + units + " from " + productType + " in storage. Remaining: " + m_storageList[i].Units);
                    }

                    return;
                }
                // handle soilEnricher case (Manure OR Fertilizer)
                else if (productType == Resources.Type.SoilEnricher) {
                    if (m_storageList[i].Type == Resources.Type.Manure || m_storageList[i].Type == Resources.Type.Fertilizer) {
                        if (units >= m_storageList[i].Units) {
                            Debug.Log("[StoresProduct] Destroying UI: " + m_storageList[i].UI.gameObject.name);

                            Destroy(m_storageList[i].UI.gameObject);
                            m_storageList.RemoveAt(i);
                            Debug.Log("[StoresProduct] Removed from list");
                        }
                        else {
                            StoredProduct oldItem = m_storageList[i];
                            StoredProduct newItem = new StoredProduct(oldItem.Type, oldItem.UI, oldItem.Units - units);
                            m_storageList[i] = newItem;

                            Debug.Log("[StoresProduct] Removed " + units + " from " + productType + " in storage. Remaining: " + m_storageList[i].Units);
                        }

                        return;
                    }
                }
            }
        }

        private void RedistributeQueue() {
            for (int i = 0; i < m_storageList.Count; i++) {
                // order storedProducts with newer on the left and older on the right
                StoredProduct storedProduct = m_storageList[i];
                storedProduct.UI.transform.localPosition = new Vector3(
                    m_initialQueuePos.x,
                    m_initialQueuePos.y,
                    m_initialQueuePos.z - (m_storageList.Count - i) * m_iconOffsetZ
                    );
            }
        }

        #region Handlers

        private void HandleTimerExpired(object sender, EventArgs e) {
            UIStoredProduct expiredProduct = (UIStoredProduct)sender;
            Debug.Log("[StoresProduct] storage expired");
            StorageExpired?.Invoke(this, new ResourceEventArgs(expiredProduct.GetResourceType(), expiredProduct.GetUnits()));

            if (TryRemoveFromStorage(expiredProduct.GetResourceType(), expiredProduct.GetUnits())) {
                RedistributeQueue();
            }
            else {
                Debug.Log("[Skimmer] Unable to remove");
            }
        }

        private void HandleEconomyUpdated(object sender, EventArgs args) {
            // Query sell solution
            for (int i = 0; i < m_storageList.Count; i++) {
                Resources.Type resourceType = m_storageList[i].Type;

                // for each product, find a buyer
                int unitsSold;
                List<RoadSegment> path;
                Requests recipient = RegionMgr.Instance.GetRegionByPos(this.transform.position).QueryClearingHouseSolution(this, resourceType, out unitsSold, out path);

                Debug.Log("[StoresProduct] " + this.gameObject.name + " trying to sell " + resourceType + ". UnitsSold: " + unitsSold);

                if (recipient != null) {
                    // found resource -- try summon truck

                    Debug.Log("[StoresProduct] Query was successful. length of path: " + path.Count + ". Units sold: " + unitsSold);
                    if (RoadMgr.Instance.TrySummonTruck(resourceType, unitsSold, path, this, recipient)) {
                        Debug.Log("[Requests] Truck summoned successfully");

                        // set request to en-route
                        recipient.SetEnRoute(resourceType, unitsSold);
                    }
                    else {
                        Debug.Log("[Requests] Truck not summoned");
                    }
                }
            }
        }
        #endregion // Handlers

        #region Gets and Sets

        public void SetSupplierType(SupplierType type) {
            m_supplierType = type;
        }

        public SupplierType GetSupplierType() {
            return m_supplierType;
        }

        #endregion // Gets and Sets
    }
}
