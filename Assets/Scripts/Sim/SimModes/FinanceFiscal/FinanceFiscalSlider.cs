using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala.Fiscal
{

    public enum FiscalType
    {
        Private,
        Govt
    }

    public struct FiscalChange
    {
        public float Delta;
        public FiscalType Type;

        public FiscalChange(float inDelta, FiscalType inType) {
            Delta = inDelta;
            Type = inType;
        }
    }

    public class FinanceFiscalSlider : MonoBehaviour
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private TMP_Text m_title;

        [SerializeField] private FiscalType m_type;

        private float m_startVal;
        private float m_prevVal;

        private float m_aggregateDelta;

        public Slider Slider {
            get { return m_slider; }
        }
        public TMP_Text Title {
            get { return m_title; }
        }
        public FiscalType Type {
            get { return m_type; }
        }
        public float AggregateDelta {
            get { return m_aggregateDelta; }
        }

        private void Awake() {
            m_slider.onValueChanged.AddListener(HandleSliderValChanged);

            m_prevVal = m_startVal = m_slider.value;

            m_aggregateDelta = 0;
        }

        public int GetActionState() {
            if (m_slider.value == m_startVal) {
                return 0; // even (no action)
            }
            else if (m_slider.value < m_startVal) {
                return -1; // output was lowered
            }
            else {
                return 1; // output was raised
            }
        }

        #region Handlers 

        private void HandleSliderValChanged(float newVal) {
            float delta = newVal - m_prevVal;

            FiscalChange newChange = new FiscalChange(delta, m_type);
            EventMgr.FiscalSliderChanged?.Invoke(newChange);

            m_prevVal = newVal;

            m_aggregateDelta = newVal - m_startVal;
        }

        #endregion // Handlers
    }
}
