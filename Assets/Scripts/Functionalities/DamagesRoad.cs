using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Functionalities
{
    public class DamagesRoad : MonoBehaviour
    {
        #region Inspector

        [Serializable]
        private struct DamageResourcePair {
            public Resources.Type ResourceType;
            public float Damage;

            public DamageResourcePair(Resources.Type resourceType, float damage) {
                ResourceType = resourceType;
                Damage = damage;
            }
        }

        [SerializeField] private List<DamageResourcePair> m_damageMap;

        #endregion // Inspector


        public void DamageRoad(Road toDamage, Resources.Type carriedResourceType) {
            Debug.Log("[DamagesRoad] Damaging road...");

            float dmgAmt = 0;
            bool foundAny = false;
            for (int p = 0; p < m_damageMap.Count; p++) { 
                if (m_damageMap[p].ResourceType == carriedResourceType) {
                    dmgAmt = m_damageMap[p].Damage;
                    foundAny = true;
                    break;
                }
            }

            Debug.Log("[DamagesRoad] Damage amount: " + dmgAmt);

            toDamage.ApplyDamage(dmgAmt);

            if (!foundAny) {
                Debug.Log("[DamagesRoad] No damage specified for carried resource of type " + carriedResourceType);
            }
        }

        #region AllVars Gets & Sets

        public float GetAmtForResource(Resources.Type resourceType) {
            for (int i = 0; i < m_damageMap.Count; i++) {
                if (m_damageMap[i].ResourceType == resourceType) {
                    return m_damageMap[i].Damage;
                }
            }

            return -1;
        }

        public void SetAmtForResource(Resources.Type resourceType, float newAmt) {
            for (int i = 0; i < m_damageMap.Count; i++) {
                if (m_damageMap[i].ResourceType == resourceType) {
                    DamageResourcePair newPair = new DamageResourcePair(resourceType, newAmt);
                    m_damageMap.RemoveAt(i);
                    m_damageMap.Add(newPair);
                    break;
                }
            }
        }

        #endregion // AllVars Gets & Sets
    }
}
