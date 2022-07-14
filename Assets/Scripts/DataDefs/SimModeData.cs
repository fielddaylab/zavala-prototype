using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    [CreateAssetMenu(fileName = "NewSimModeData", menuName = "Data/SimModeData")]
    public class SimModeData : ScriptableObject
    {
        [SerializeField] string m_modeID;
        [SerializeField] IndicatorData[] m_indicatorData;
        [SerializeField] private string m_feedbackText;

        [SerializeField] private string[] m_unlocksModes;
        [SerializeField] private string m_unlockModeText;
        [SerializeField] private Color m_hatColor;

        public string ID {
            get { return m_modeID; }
        }
        public IndicatorData[] IndicatorData {
            get { return m_indicatorData; }
        }
        public string FeedbackText {
            get { return m_feedbackText; }
        }
        public string[] UnlocksModes {
            get { return m_unlocksModes; }
        }
        public string UnlockModeText {
            get { return m_unlockModeText; }
        }
        public Color HatColor {
            get { return m_hatColor; }
        }
    }
}