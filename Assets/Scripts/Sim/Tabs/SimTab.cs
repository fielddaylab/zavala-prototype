using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    [RequireComponent(typeof(Button))]
    public class SimTab : MonoBehaviour
    {
        [SerializeField] private SimButtonData[] m_buttonData;

        public SimButtonData[] GetSimButtonData() {
            return m_buttonData;
        }
    }
}
