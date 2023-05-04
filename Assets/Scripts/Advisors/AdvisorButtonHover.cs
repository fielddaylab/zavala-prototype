using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zavala
{
    public class AdvisorButtonHover : MonoBehaviour
    {
        [SerializeField] private HoverCounter m_hoverCounter;

        public void RegisterHover() {
            m_hoverCounter.RegisterHover();
        }

        public void DeregisterHover() {
            m_hoverCounter.DeregisterHover();
        }
    }
}
