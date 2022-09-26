using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Functionalities
{
    public class StoresProduct : MonoBehaviour
    {
        public int MaxProducts; // the max number of products this can store

        public event EventHandler StorageExceeded; // when a product would be added but there is no room

        private List<Resources.Type> m_storageList; // the products in storage

        public List<Resources.Type> GetStorageList() {
            return m_storageList;
        }

        public void AddToStorage(Resources.Type productType) {
            if (m_storageList.Count >= MaxProducts) {
                Debug.Log("Storage is full! Not adding to list.");
                StorageExceeded.Invoke(this, EventArgs.Empty);
            }
            else {
                Debug.Log("Added to list");
                m_storageList.Add(productType);
            }
        }
    }
}
