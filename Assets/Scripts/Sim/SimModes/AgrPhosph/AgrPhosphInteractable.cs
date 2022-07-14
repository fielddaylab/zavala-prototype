using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class AgrPhosphInteractable : MonoBehaviour
    {
        [SerializeField] private Slider m_uptakeSlider;

        private float m_prevVal;
        private float m_startVal;

        public Slider UptakeSlider {
            get { return m_uptakeSlider; }
        }

        public int GetActionState() {
            if (m_uptakeSlider.value == m_startVal) {
                return 0; // even (no action)
            }
            else if (m_uptakeSlider.value < m_startVal) {
                return -1; // output was lowered
            }
            else {
                return 1; // output was raised
            }
        }

        void Start() {
            m_uptakeSlider.onValueChanged.AddListener(OnSliderChanged);

            m_prevVal = m_startVal = m_uptakeSlider.value;
        }

        #region Handlers

        private void OnSliderChanged(float newVal) {
            float delta = newVal - m_prevVal;

            EventMgr.AgrPhosphFarmExcessAdjusted?.Invoke(delta);

            m_prevVal = newVal;
        }

        #endregion // Handlers

    }
}