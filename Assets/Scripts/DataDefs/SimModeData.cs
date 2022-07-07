using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    [CreateAssetMenu(fileName = "NewSimModeData", menuName = "Data/SimModeData")]
    public class SimModeData : ScriptableObject
    {
        [SerializeField] IndicatorData[] m_indicatorData;

        public IndicatorData[] IndicatorData {
            get { return m_indicatorData; }
        }
    }
}