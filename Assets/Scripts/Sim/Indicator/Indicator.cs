using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala.Indicators
{
    public enum CutoffType
    {
        Above,
        Below
    }

    public class Indicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private Slider m_slider;
        [SerializeField] private Image m_cutoff;

        private CutoffType m_cutoffType;
        private float m_cutoffValue;
        private bool m_meetsCutoff;

        public Slider Slider {
            get { return m_slider; }
        }
        public Image Cutoff {
            get { return m_cutoff; }
        }
        public TMP_Text Title {
            get { return m_title; }
        }
        public bool MeetsCutoff {
            get { return m_meetsCutoff; }
        }


        public void InitIndicator(IndicatorData newData) {
            m_cutoffType = newData.CutoffType;
            m_cutoffValue = newData.CutoffValue;

            m_title.color = newData.Color;
            m_slider.fillRect.GetComponent<Image>().color = newData.Color;

            // Set cutoff image
            Vector3 currScale = m_cutoff.rectTransform.localScale;
            if (m_cutoffType == CutoffType.Above) {
                m_cutoff.rectTransform.localScale = new Vector3(currScale.x, Mathf.Abs(currScale.y), currScale.z);
            }
            else if (m_cutoffType == CutoffType.Below){
                m_cutoff.rectTransform.localScale = new Vector3(currScale.x, -Mathf.Abs(currScale.y), currScale.z);
            }

            float barHeight = this.GetComponent<RectTransform>().rect.height;
            float relativeY = m_cutoffValue * barHeight - (barHeight / 2);
            Vector3 currPos = m_cutoff.rectTransform.localPosition;
            m_cutoff.rectTransform.localPosition = new Vector3(currPos.x, relativeY, currPos.z);

            HandleSliderValChanged();
        }

        #region Unity Callbacks

        private void Awake() {
            m_slider.onValueChanged.AddListener(delegate { HandleSliderValChanged(); });
        }

        #endregion // Unity Callbacks

        public void HandleSliderValChanged() {
            m_meetsCutoff = EvaluateCutoff();
            EventMgr.IndicatorUpdated?.Invoke();
        }

        private bool EvaluateCutoff() {
            switch (m_cutoffType) {
                default:
                    break;
                case CutoffType.Above:
                    return m_slider.value >= m_cutoffValue;
                case CutoffType.Below:
                    return m_slider.value <= m_cutoffValue;
            }
            return false;
        }
    }
}
