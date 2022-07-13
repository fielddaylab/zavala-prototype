using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Indicators;

namespace Zavala
{
    [CreateAssetMenu(fileName = "NewIndicatorData", menuName = "Data/IndicatorData")]
    public class IndicatorData : ScriptableObject
    {
        [SerializeField] private string m_title;
        [SerializeField] private float m_cutoffValue;
        [SerializeField] private CutoffType m_cutoffType;
        [SerializeField] private Color m_color;
        [SerializeField] private Color m_bgColor;

        public string Title {
            get { return m_title; }
        }
        public float CutoffValue {
            get { return m_cutoffValue; }
        }
        public CutoffType CutoffType {
            get { return m_cutoffType; }
        }
        public Color Color {
            get { return m_color; }
        }
        public Color BGColor {
            get { return m_bgColor; }
        }
    }
}
