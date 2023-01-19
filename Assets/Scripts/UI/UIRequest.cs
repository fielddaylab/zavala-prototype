using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;

namespace Zavala
{
    public class UIRequest : MonoBehaviour
    {
        [SerializeField] private Image m_bg;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private TMP_Text m_unitsText;

        private Resources.Type m_resourceType;

        private int m_initialUnits; // how many units in total request
        private int m_units;// how many units left to complete request

        //private UITimer m_uiTimer;

        private int m_remainingCycles;

        private bool m_enRoute; // whether a truck has been dispatched to serve this request

        public event EventHandler TimerExpired; // when the timer completes

        private void InitBasics(Resources.Type resourceType, int units) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_resourceType = resourceType;
            m_units = m_initialUnits = units;
            m_unitsText.text = "" + units;

            m_remainingCycles = -1;

            m_enRoute = false;
        }

        // no timeout
        public void Init(Resources.Type resourceType, int units) {
            InitBasics(resourceType, units);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int requestTimeout, Cycles cycleSync, int units) {
            InitBasics(resourceType, units);

            cycleSync.PreCycleCompleted += HandlePreCycleCompleted;
            m_remainingCycles = requestTimeout;

            m_bg.color = GameDB.Instance.UIRequestDefaultColor;

            //m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            //m_uiTimer.Init(requestTimeout, false);
            //m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        public int GetUnits() {
            return m_units;
        }

        public int GetInitialUnits() {
            return m_initialUnits;
        }

        public void SetEnRoute() {
            m_enRoute = true;

            // TODO: nuance here. If a request is partially fulfilled, remaining products must continue to be tracked as not en-route
            //m_bg.color = GameDB.Instance.UIRequestEnRouteColor;
        }

        public bool IsEnRoute() {
            return m_enRoute;
        }

        public void ModifyUnits(int amt) {
            m_units += amt;

            UpdateUnitsText();
        }

        public void UpdateUnitsText() {
            m_unitsText.text = "" + m_units;
        }

        #region Handlers

        /*
        private void HandleTimerCompleted(object sender, EventArgs e) {
            Debug.Log("[UIRequest] Timer completed");

            RequestExpired?.Invoke(this, EventArgs.Empty);
        }
        */

        private void HandlePreCycleCompleted(object sender, EventArgs e) {
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
                else if (m_remainingCycles == 1) {
                    m_bg.color = GameDB.Instance.UIRequestExpiringColor;
                }
            }
        }

        #endregion // Handlers
    }
}