using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala.Factions
{
    public class FactionSlider : MonoBehaviour
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private FactionType m_type;


        public Slider Slider {
            get { return m_slider; }
        }
        public FactionType Type {
            get { return m_type; }
        }
    }
}