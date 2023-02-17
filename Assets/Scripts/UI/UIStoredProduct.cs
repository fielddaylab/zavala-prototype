using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;

namespace Zavala
{
    public class UIStoredProduct : MonoBehaviour
    {
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private TMP_Text m_unitsText;

        private Resources.Type m_resourceType;
        private int m_units;

        private int m_remainingCycles;

        public event EventHandler TimerExpired; // when the timer completes

        private void InitBasics(Resources.Type resourceType, int units) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceStoredIcon(resourceType);
            m_resourceIcon.SetNativeSize();
            m_resourceType = resourceType;
            m_units = units;
            m_unitsText.text = "" + units;
        }

        // no timeout
        public void Init(Resources.Type resourceType, int units) {
            InitBasics(resourceType, units);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int storageTimeout, Cycles cycleSync, int units) {
            InitBasics(resourceType, units);

            cycleSync.CycleCompleted += HandleCycleCompleted;
            m_remainingCycles = storageTimeout;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        public int GetUnits() {
            return m_units;
        }

        public void UpdateUnitsText(int newUnits) {
            m_unitsText.text = "" + newUnits;
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
