using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
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

            public void SetAmt(int newAmt) {
                Amt = newAmt;
            }
        }

        [SerializeField] private List<ResourceAmtPair> m_resourceAmtMap;

        public void GeneratePipBatch(Tile destTile, Resources.Type resourceType) {
            for (int p = 0; p < m_resourceAmtMap.Count; p++) {
                if (m_resourceAmtMap[p].Type == resourceType) {
                    Debug.Log("[GeneratesPhosph] Generating " + m_resourceAmtMap[p].Amt + " units");
                    for (int i = 0; i < m_resourceAmtMap[p].Amt; i++) {
                        Debug.Log("[Instantiate] Instantiating pip prefab");
                        PhosphPip newPip = Instantiate(GameDB.Instance.PipPrefab).GetComponent<PhosphPip>();
                        newPip.Init(destTile);

                    }

                    EventMgr.Instance.TriggerEvent(ID.PipsGenerated, new PipsGeneratedEventArgs(
                        RegionMgr.Instance.GetRegionByPos(destTile.transform.position),
                        m_resourceAmtMap[p].Amt)
                        );
                }
            }
        }

        #region AllVars Gets & Sets

        public int GetAmtForResource(Resources.Type resourceType) {
            for (int i = 0; i < m_resourceAmtMap.Count; i++) {
                if (m_resourceAmtMap[i].Type == resourceType) {
                    return m_resourceAmtMap[i].Amt;
                }
            }

            return -1;
        }

        public void SetAmtForResource(Resources.Type resourceType, int newAmt) {
            for (int i = 0; i < m_resourceAmtMap.Count; i++) {
                if (m_resourceAmtMap[i].Type == resourceType) {
                    ResourceAmtPair newPair = new ResourceAmtPair(resourceType, newAmt);
                    m_resourceAmtMap.RemoveAt(i);
                    m_resourceAmtMap.Add(newPair);
                    break;
                }
            }
        }

        #endregion // AllVars Gets & Sets
    }
}