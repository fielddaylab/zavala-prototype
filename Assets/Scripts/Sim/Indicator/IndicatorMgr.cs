using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Indicators;

namespace Zavala
{
    public class IndicatorMgr : MonoBehaviour
    {
        #region Inspector 

        [SerializeField] Indicator[] m_Indicators;
        [SerializeField] Button m_submitButton;

        #endregion // Inspector

        #region Unity Callbacks

        private void Awake() {
            EventMgr.SetNewIndicators?.AddListener(HandleNewIndicators);
            EventMgr.IndicatorUpdated?.AddListener(HandleIndicatorUpdated);
            m_submitButton.interactable = false;
        }

        #endregion // Unity Callbacks

        #region Event Handlers

        private void HandleIndicatorUpdated() {
            Debug.Log("[IndicatorMgr] Handling indicator updated");

            foreach (Indicator indicator in m_Indicators) {
                if (!indicator.MeetsCutoff) {
                    m_submitButton.interactable = false;

                    Debug.Log("[IndicatorMgr] An indicator did not meet its cutoff");

                    return;
                }
            }

            Debug.Log("[IndicatorMgr] All indicators meet their cutoffs");
            m_submitButton.interactable = true;
        }

        private void HandleNewIndicators(IndicatorData[] newIndicatorData) {
            Debug.Log("[IndicatorMgr] New indicators");
            int indicatorIndex = 0;

            foreach(IndicatorData data in newIndicatorData) {
                m_Indicators[indicatorIndex].InitIndicator(data);
                indicatorIndex++;
            }
        }

        #endregion // Event Handlers
    }
}
