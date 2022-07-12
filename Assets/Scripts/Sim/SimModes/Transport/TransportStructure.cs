using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class TransportStructure : MonoBehaviour
    {
        private float m_cost;

        public void Remove() {
            Destroy(this.gameObject);
        }
    }
}

