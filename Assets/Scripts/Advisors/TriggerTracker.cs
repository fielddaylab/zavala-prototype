using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;

namespace Zavala
{
    public class TriggerTracker : MonoBehaviour
    {
        public static TriggerTracker Instance;

        private Dictionary<SimEventType, bool> m_expendedMap;

        public void Init() {
            Instance = this;

            m_expendedMap = new Dictionary<SimEventType, bool>();
            foreach(SimEventType val in Enum.GetValues(typeof(SimEventType))) {
                m_expendedMap.Add(val, false);
            }
        }

        public void SetTriggerExpended(SimEventType triggerType) {
            m_expendedMap[triggerType] = true;
        }

        public bool IsTriggerExpended(SimEventType triggerType) {
            return m_expendedMap[triggerType];
        }
    }
}
