using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Functionalities
{
    public class Cycles : MonoBehaviour {
        public float CycleTime; // how long a cycle lasts

        private UITimer m_uiTimer;

        public event EventHandler CycleCompleted; // when the cycle completes

        private void Start() {
            m_uiTimer = null;

            StartCycle(); // debug
        }

        public void StartCycle() {
            m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            m_uiTimer.Init(CycleTime, SettingsMgr.VisibleCycles);
            m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        public void CompleteCycle() {
            Destroy(m_uiTimer.gameObject);
            m_uiTimer = null;
            CycleCompleted?.Invoke(this, EventArgs.Empty);

            // if repeating
            StartCycle();
        }

        #region Handlers
        
        private void HandleTimerCompleted(object sender, EventArgs e) {
            Debug.Log("[Cycles] Timer completed");
            CompleteCycle();
        }

        #endregion // Handlers
    }
}
