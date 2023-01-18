using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Zavala.Resources;

namespace Zavala.Functionalities
{
    public class StoresProduct : MonoBehaviour
    {
        public struct StoredProduct {
            public Resources.Type Type;
            public UIStoredProduct UI;
            public int Units;

            public StoredProduct(Resources.Type type, UIStoredProduct ui, int units) {
                Type = type;
                UI = ui;
                Units = units;
            }
        }

        public int MaxProducts; // the max number of products this can store

        public event EventHandler StorageExceeded; // when a product would be added but there is no room
        public event EventHandler<ResourceEventArgs> RemovedStorage;
        public event EventHandler<ResourceEventArgs> StorageExpired;

        [SerializeField] private bool m_hasTimeout;
        [SerializeField] private int m_storageTimeout; // num Cycles
        [SerializeField] private float m_iconOffsetZ = 0.25f;

        private List<StoredProduct> m_storageList; // the products in storage

        private Vector3 m_initialQueuePos;

        private void OnEnable() {
            m_storageList = new List<StoredProduct>();
        }

        private void Start() {
            m_initialQueuePos = GameDB.Instance.UIStoredProductPrefab.transform.localPosition;

            RegionMgr.Instance.GetRegionByPos(this.transform.position).RegisterWithClearingHouse(this);
        }

        public bool StorageContains(Resources.Type resourceType, int desiredUnits, out Resources.Type foundResourceType, out int foundUnits) {
            Debug.Log("[StoresProduct] Checking if storage contains " + resourceType);
            for (int i = 0; i < m_storageList.Count; i++) {
                Debug.Log("[StoresProduct] store component type: " + m_storageList[i].Type);
                if (m_storageList[i].Type == resourceType) {
                    if (m_storageList[i].Units >= desiredUnits) {
                        foundResourceType = resourceType;
                        foundUnits = m_storageList[i].Units;
                        Debug.Log("[StoresProduct] " + foundUnits + " units found");
                        return true;
                    }
                    else {
                        Debug.Log("[StoresProduct] resource found, but not enough units (" + m_storageList[i].Units + " units found vs. " + desiredUnits + " requested");
                    }
                }
                // handle SoilEnricher case (Manure OR Fertilizer)
                else if (resourceType == Resources.Type.SoilEnricher) {
                    if (m_storageList[i].Type == Resources.Type.Manure || m_storageList[i].Type == Resources.Type.Fertilizer) {
                        foundResourceType = m_storageList[i].Type;
                        foundUnits = 0;
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
            if (m_storageList.Count >= MaxProducts) {
                Debug.Log("[StoresProduct] Storage is full! Not adding to list.");
                StorageExceeded?.Invoke(this, EventArgs.Empty);
                return false;
            }
            else {
                Debug.Log("[StoresProduct] Added to storage list");
                Debug.Log("[Instantiate] Instantiating UIStoredProduct prefab");
                UIStoredProduct newProductUI = Instantiate(GameDB.Instance.UIStoredProductPrefab, this.transform).GetComponent<UIStoredProduct>();
                if (m_hasTimeout) {
                    newProductUI.Init(productType, m_storageTimeout, this.GetComponent<Cycles>());
                }
                else {
                    newProductUI.Init(productType);
                }
                StoredProduct newProduct = new StoredProduct(productType, newProductUI, units);
                m_storageList.Add(newProduct);
                newProductUI.TimerExpired += HandleTimerExpired;

                RedistributeQueue();
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
                RemoveFromStorageList(productType);

                RedistributeQueue();
                RemovedStorage?.Invoke(this, new ResourceEventArgs(productType, units));
                return true;
            }
        }

        private void RemoveFromStorageList(Resources.Type productType) {
            for (int i = 0; i < m_storageList.Count; i++) {
                if (m_storageList[i].Type == productType) {
                    Debug.Log("[StoresProduct] Destroying UI: " + m_storageList[i].UI.gameObject.name);

                    Destroy(m_storageList[i].UI.gameObject);
                    m_storageList.RemoveAt(i);
                    Debug.Log("[StoresProduct] Removed from list");

                    return;
                }
                // handle soilEnricher case (Manure OR Fertilizer)
                else if (productType == Resources.Type.SoilEnricher) {
                    if (m_storageList[i].Type == Resources.Type.Manure || m_storageList[i].Type == Resources.Type.Fertilizer) {
                        Debug.Log("[StoresProduct] Destroying UI: " + m_storageList[i].UI.gameObject.name);

                        Destroy(m_storageList[i].UI.gameObject);
                        m_storageList.RemoveAt(i);
                        Debug.Log("[StoresProduct] Removed from list");

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


        #endregion // Handlers
    }
}
