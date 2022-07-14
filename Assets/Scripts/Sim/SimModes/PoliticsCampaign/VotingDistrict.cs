using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala {
    public class VotingDistrict : MonoBehaviour
    {
        [SerializeField] private float m_supportYield;

        private int m_numSupports;

        public float SupportYield {
            get { return m_supportYield; }
        }

        public bool IsSupporting() {
            return m_numSupports > 0;
        }
        public void AddSupport() {
            m_numSupports++;
        }
        public void RemoveSupport() {
            m_numSupports--;
        }


        private void Awake() {
            m_numSupports = 0;
        }
    }
}