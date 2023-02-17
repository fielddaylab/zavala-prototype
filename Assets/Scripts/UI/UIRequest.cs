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
        [SerializeField] private Image m_unitsBG;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private TMP_Text m_unitsText;

        private Resources.Type m_resourceType;

        private int m_initialUnits; // how many units in total request
        private int m_units;// how many units left to complete request
        private int m_enRouteUnits; // how many units are currently on their way
        private bool m_continuous;

        //private UITimer m_uiTimer;

        private int m_remainingCycles;

        public event EventHandler TimerExpired; // when the timer completes

        private Cycles m_cycleSync;

        private void InitBasics(Resources.Type resourceType, int units, bool visible, bool continuous) {
            if (visible) {
                m_resourceIcon.sprite = GameDB.Instance.GetResourceRequestIcon(resourceType);
                m_resourceIcon.SetNativeSize();
            }
            else {
                m_resourceIcon.gameObject.SetActive(false);
                m_unitsText.gameObject.SetActive(false);
                m_bg.gameObject.SetActive(false);
                m_unitsBG.gameObject.SetActive(false);
            }

            m_resourceType = resourceType;
            m_enRouteUnits = 0;
            m_units = m_initialUnits = units;
            m_continuous = continuous;

            UpdateUnitsText();

            m_remainingCycles = -1;
        }

        // no timeout
        public void Init(Resources.Type resourceType, int units, bool visible, bool continuous) {
            InitBasics(resourceType, units, visible, continuous);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int requestTimeout, Cycles cycleSync, int units, bool visible, bool continuous) {
            InitBasics(resourceType, units, visible, continuous);

            m_cycleSync = cycleSync;
            m_cycleSync.PreCycleCompleted += HandlePreCycleCompleted;
            m_remainingCycles = requestTimeout;

            m_bg.color = GameDB.Instance.UIRequestDefaultColor;

            //m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            //m_uiTimer.Init(requestTimeout, false);
            //m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        public int GetFulfillableUnits() {
            return m_units - m_enRouteUnits;
        }

        public int GetInitialUnits() {
            return m_initialUnits;
        }

        public bool IsContinuous() {
            return m_continuous;
        }

        public void SetEnRoute(int enRouteUnits) {
            m_enRouteUnits += enRouteUnits;

            // TODO: nuance here. If a request is partially fulfilled, remaining products must continue to be tracked as not en-route
            //m_bg.color = GameDB.Instance.UIRequestEnRouteColor;
        }

        public void ModifyUnits(int amt) {
            if (!m_continuous) {
                m_units += amt;

                if (amt < 0) {
                    // en route units have been delivered
                    m_enRouteUnits += amt;
                }
            }

            UpdateUnitsText();
        }

        public void UpdateUnitsText() {
            if (m_continuous) {
                m_unitsText.text = "Inf";
            }
            else {
                m_unitsText.text = "" + m_units;
            }
        }

        private void MarkUrgent() {
            m_bg.sprite = GameDB.Instance.UIRequestExpiringBGIcon;
            // m_bg.color = GameDB.Instance.UIRequestExpiringColor;
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
                    if (m_bg != null) {
                        MarkUrgent();
                    }
                }
            }
        }

        #endregion // Handlers
    }
}