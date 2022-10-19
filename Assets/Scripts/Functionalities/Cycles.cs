using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Settings;

namespace Zavala.Functionalities
{
    public class Cycles : MonoBehaviour {
        public float CycleTime; // how long a cycle lasts

        [SerializeField] private bool m_isVisible = true;

        private UITimer m_uiTimer;

        public event EventHandler PreCycleCompleted; // just before cycle completes
        public event EventHandler CycleCompleted; // when the cycle completes

        private void Start() {
            m_uiTimer = null;

            StartCycle(); // debug
        }

        public void StartCycle() {
            Debug.Log("[Instantiate] Instantiating UITimer prefab");
            m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            bool visible = SettingsMgr.VisibleCycles && m_isVisible;
            m_uiTimer.Init(CycleTime, visible);
            m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        public void CompleteCycle() {
            Destroy(m_uiTimer.gameObject);
            m_uiTimer = null;
            PreCycleCompleted?.Invoke(this, EventArgs.Empty);
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
