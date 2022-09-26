using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance;

        [SerializeField] private GameDB m_gameDB;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
                return;
            }

            m_gameDB.Init();
        }
    }
}