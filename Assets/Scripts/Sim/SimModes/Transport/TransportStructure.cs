using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Transport
{
    public enum BuildType
    {
        Rail,
        Highway,
        Road,
        Bridge
    }

    public struct BuildDetails
    {
        public float Cost;
        public BuildType Type;
        public bool Reduces;

        public BuildDetails(float inCost, BuildType inType, bool inReduces) {
            Cost = inCost;
            Type = inType;
            Reduces = inReduces;
        }
    }

    public class TransportStructure : MonoBehaviour
    {
        private BuildDetails m_buildDetails;

        public void SetDetails(float cost, BuildType type, bool reduces) {
            m_buildDetails = new BuildDetails(cost, type, reduces);
        }

        public BuildDetails GetDetails() {
            return m_buildDetails;
        }

        public void Build() {
            EventMgr.StructureBuilt?.Invoke(m_buildDetails);
        }

        public void Remove() {
            EventMgr.StructureRemoved?.Invoke(m_buildDetails);
            Destroy(this.gameObject);
        }
    }
}

