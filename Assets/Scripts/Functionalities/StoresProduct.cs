using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Zavala.Functionalities
{
    public class StoresProduct : MonoBehaviour
    {
        public struct StoredProduct {
            public Resources.Type Type;
            public UIStoredProduct UI;

            public StoredProduct(Resources.Type type, UIStoredProduct ui) {
                Type = type;
                UI = ui;
            }
        }

        public int MaxProducts; // the max number of products this can store

        public event EventHandler StorageExceeded; // when a product would be added but there is no room

        [SerializeField] private float m_iconOffsetZ = 0.25f;

        private List<StoredProduct> m_storageList; // the products in storage

        private Vector3 m_initialQueuePos;

        private void Awake() {
            m_storageList = new List<StoredProduct>();
            m_initialQueuePos = GameDB.Instance.UIStoredProductPrefab.transform.localPosition;
        }

        public List<StoredProduct> GetStorageList() {
            return m_storageList;
        }

        public bool TryAddToStorage(Resources.Type productType) {
            if (m_storageList.Count >= MaxProducts) {
                Debug.Log("Storage is full! Not adding to list.");
                StorageExceeded?.Invoke(this, EventArgs.Empty);
                return false;
            }
            else {
                Debug.Log("Added to list");
                UIStoredProduct newProductUI = Instantiate(GameDB.Instance.UIStoredProductPrefab, this.transform).GetComponent<UIStoredProduct>();
                newProductUI.Init(productType);
                StoredProduct newProduct = new StoredProduct(productType, newProductUI);
                m_storageList.Add(newProduct);

                RedistributeQueue();
                return true;
            }
        }

        public bool TryRemoveFromStorage(Resources.Type productType) {
            if (m_storageList.Count <= 0) {
                Debug.Log("Storage is empty! Not removing from list.");
                // StorageEmpty.Invoke(this, EventArgs.Empty);
                return false;
            }
            else {
                Debug.Log("Removed from list");
                RemoveFromStorageList(productType);

                RedistributeQueue();
                return true;
            }
        }

        private void RemoveFromStorageList(Resources.Type productType) {
            for (int i = 0; i < m_storageList.Count; i++) {
                if (m_storageList[i].Type == productType) {
                    Destroy(m_storageList[i].UI.gameObject);
                    m_storageList.RemoveAt(i);
                    return;
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
    }
}
