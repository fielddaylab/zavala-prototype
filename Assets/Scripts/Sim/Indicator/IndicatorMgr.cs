using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Indicators;

namespace Zavala
{
    public class IndicatorMgr : MonoBehaviour
    {
        public static IndicatorMgr Instance;

        #region Inspector 

        [SerializeField] private Indicator[] m_indicators;
        [SerializeField] private Button m_submitButton;

        [SerializeField] private float indicatorDefaultPos;
        [SerializeField] private float indicatorOffset;

        #endregion // Inspector

        #region Unity Callbacks

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else if (this != Instance) {
                Debug.Log("Warning! Multiple instances of IndicatorMgr detected. Removing duplicate.");
                Destroy(gameObject);
                return;
            }

            EventMgr.SetNewIndicators?.AddListener(HandleNewIndicators);
            EventMgr.IndicatorUpdated?.AddListener(HandleIndicatorUpdated);
            m_submitButton.interactable = false;

            foreach (Indicator indicator in m_indicators) {
                indicator.gameObject.SetActive(false);
            }
        }

        #endregion // Unity Callbacks

        #region Event Handlers

        private void HandleIndicatorUpdated() {
            //Debug.Log("[IndicatorMgr] Handling indicator updated");
            EvaluateCutoffs();
        }

        private void HandleNewIndicators(IndicatorData[] newIndicatorData) {
            //Debug.Log("[IndicatorMgr] New indicators");
            int indicatorIndex = 0;

            foreach(IndicatorData data in newIndicatorData) {
                m_indicators[indicatorIndex].InitIndicator(data);
                m_indicators[indicatorIndex].gameObject.SetActive(true);

                Vector3 currPos = m_indicators[indicatorIndex].GetComponent<RectTransform>().localPosition;
                m_indicators[indicatorIndex].GetComponent<RectTransform>().localPosition = new Vector3(indicatorDefaultPos, currPos.y, currPos.z);

                indicatorIndex++;
            }

            // WARNING: VERY MUCH THROWAWAY CODE BELOW
            for (int i = 0; i < indicatorIndex - 1; i++) {
                // for each indicator
                for (int j = 0; j <= i; j++) {
                    // shift it and preceeding indicators indicators over
                    Vector3 currPos = m_indicators[j].GetComponent<RectTransform>().localPosition;
                    float shiftedPos = currPos.x + indicatorOffset;
                    m_indicators[j].GetComponent<RectTransform>().localPosition = new Vector3(shiftedPos, currPos.y, currPos.z);
                }
            }
            // END WARNING

            while (indicatorIndex < m_indicators.Length) {
                m_indicators[indicatorIndex].gameObject.SetActive(false);
                indicatorIndex++;
            }

            EvaluateCutoffs();
        }

        #endregion // Event Handlers

        private void EvaluateCutoffs() {
            foreach (Indicator indicator in m_indicators) {
                if (indicator.gameObject.activeSelf && !indicator.MeetsCutoff) {
                    m_submitButton.interactable = false;

                    //Debug.Log("[IndicatorMgr] An indicator did not meet its cutoff");

                    return;
                }
            }

            //Debug.Log("[IndicatorMgr] All indicators meet their cutoffs");
            m_submitButton.interactable = true;
        }

        public void SetIndicatorValue(int indicatorIndex, float sliderVal) {
            m_indicators[indicatorIndex].Slider.value = sliderVal;
        }
    }
}
