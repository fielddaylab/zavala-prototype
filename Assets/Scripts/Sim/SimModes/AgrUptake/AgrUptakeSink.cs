using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class AgrUptakeSink : MonoBehaviour
    {
        [SerializeField] private Slider m_uptakeSlider;

        private float m_prevVal;

        public Slider UptakeSlider {
            get { return m_uptakeSlider; }
        }

        void Start() {
            m_uptakeSlider.onValueChanged.AddListener(OnSliderChanged);

            m_prevVal = m_uptakeSlider.value;
        }

        #region Handlers

        private void OnSliderChanged(float newVal) {
            // TODO: only allow slider to change if there is excess to be uptaken

            float delta = newVal - m_prevVal;

            EventMgr.AgrUptakeSinkAmtAdjusted?.Invoke(-delta);

            m_prevVal = newVal;
        }

        #endregion // Handlers

    }
}