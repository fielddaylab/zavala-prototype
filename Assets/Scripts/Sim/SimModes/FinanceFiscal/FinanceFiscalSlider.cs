using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class FinanceFiscalSlider : MonoBehaviour
    {
        [SerializeField] private Slider m_slider;
        [SerializeField] private TMP_Text m_title;


        public Slider Slider {
            get { return m_slider; }
        }
        public TMP_Text Title {
            get { return m_title; }
        }
    }
}
