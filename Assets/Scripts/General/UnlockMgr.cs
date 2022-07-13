using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class UnlockMgr : MonoBehaviour
    {
        public static UnlockMgr Instance;

        [SerializeField] private string[] m_startUnlocked;
        [SerializeField] private bool m_unlockAll; // debug tool

        private List<string> m_unlockedSims;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (this != Instance) {
                Destroy(this.gameObject);
                return;
            }

            m_unlockedSims = new List<string>();

            foreach (string str in m_startUnlocked) {
                m_unlockedSims.Add(str);
            }
        }

        public void UnlockSim(string simID) {
            if (m_unlockedSims.Contains(simID)) {
                Debug.Log("[UnlockMgr] sim " + simID + " already unlocked");
            }
            else {
                m_unlockedSims.Add(simID);

                EventMgr.ModeUnlocked?.Invoke();
            }
        }

        public bool IsSimUnlocked(string simID) {
            if (m_unlockAll) { return true; }

            return m_unlockedSims.Contains(simID);
        }
    }
}
