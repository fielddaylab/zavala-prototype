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

        public string ID {
            get { return m_modeID; }
        }
        public IndicatorData[] IndicatorData {
            get { return m_indicatorData; }
        }
    }
}