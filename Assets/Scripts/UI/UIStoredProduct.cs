using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;

namespace Zavala
{
    public class UIStoredProduct : MonoBehaviour
    {
        [SerializeField] private Image m_resourceIcon;
        private Resources.Type m_resourceType;

        private int m_remainingCycles;

        public event EventHandler TimerExpired; // when the timer completes

        private void InitBasics(Resources.Type resourceType) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();
            m_resourceType = resourceType;
        }

        // no timeout
        public void Init(Resources.Type resourceType) {
            InitBasics(resourceType);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int storageTimeout, Cycles cycleSync) {
            InitBasics(resourceType);

            cycleSync.CycleCompleted += HandleCycleCompleted;
            m_remainingCycles = storageTimeout;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            if (m_remainingCycles == -1) {
                // request has no expiry
                return;
            }
            else {
                // tick cycles
                m_remainingCycles--;

                if (m_remainingCycles == 0) {
                    TimerExpired?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion // Handlers
    }
}
