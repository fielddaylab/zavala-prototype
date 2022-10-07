using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala.Functionalities
{
    public class GeneratesPhosphorus : MonoBehaviour
    {
        [Serializable]
        private struct ResourceAmtPair
        {
            public Resources.Type Type;
            public int Amt;

            public ResourceAmtPair(Resources.Type type, int amt) {
                Type = type;
                Amt = amt;
            }
        }

        [SerializeField] private List<ResourceAmtPair> m_resourceAmtMap;

        public void GeneratePipBatch(Tile destTile, Resources.Type resourceType) {
            for (int p = 0; p < m_resourceAmtMap.Count; p++) {
                if (m_resourceAmtMap[p].Type == resourceType) {
                    Debug.Log("[GeneratesPhosph] Generating " + m_resourceAmtMap[p].Amt + " units");
                    for (int i = 0; i < m_resourceAmtMap[p].Amt; i++) {
                        PhosphPip newPip = Instantiate(GameDB.Instance.PipPrefab).GetComponent<PhosphPip>();
                        newPip.Init(destTile);
                    }
                }
            }
        }
    }
}