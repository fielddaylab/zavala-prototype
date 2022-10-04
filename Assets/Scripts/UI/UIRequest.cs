using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;

namespace Zavala
{
    public class UIRequest : MonoBehaviour
    {
        [SerializeField] private Image m_resourceIcon;

        private Resources.Type m_resourceType;

        //private UITimer m_uiTimer;

        private int m_remainingCycles;

        private bool m_enRoute; // whether a truck has been dispatched to serve this request

        public event EventHandler TimerExpired; // when the timer completes

        private void InitBasics(Resources.Type resourceType) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_resourceType = resourceType;

            m_remainingCycles = -1;

            m_enRoute = false;
        }

        // no timeout
        public void Init(Resources.Type resourceType) {
            InitBasics(resourceType);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int requestTimeout, Cycles cycleSync) {
            InitBasics(resourceType);

            cycleSync.CycleCompleted += HandleCycleCompleted;
            m_remainingCycles = requestTimeout;

            //m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            //m_uiTimer.Init(requestTimeout, false);
            //m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        public void SetEnRoute() {
            m_enRoute = true;
        }

        public bool IsEnRoute() {
            return m_enRoute;
        }

        #region Handlers

        /*
        private void HandleTimerCompleted(object sender, EventArgs e) {
            Debug.Log("[UIRequest] Timer completed");

            RequestExpired?.Invoke(this, EventArgs.Empty);
        }
        */

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