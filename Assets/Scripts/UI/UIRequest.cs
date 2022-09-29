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

        private UITimer m_uiTimer;

        public event EventHandler RequestExpired; // when the timer completes

        // no timeout
        public void Init(Resources.Type resourceType) {
            InitBasics(resourceType);
        }

        // with timeout
        public void Init(Resources.Type resourceType, float requestTimeout) {
            InitBasics(resourceType);

            m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            m_uiTimer.Init(requestTimeout, false);
            m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        private void InitBasics(Resources.Type resourceType) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_resourceType = resourceType;
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        #region Handlers

        private void HandleTimerCompleted(object sender, EventArgs e) {
            Debug.Log("[UIRequest] Timer completed");

            RequestExpired?.Invoke(this, EventArgs.Empty);
        }

        #endregion // Handlers
    }
}