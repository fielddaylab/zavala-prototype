using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zavala
{
    public class GameMgr : MonoBehaviour
    {
        public static GameMgr Instance;

        void Start() {
            Init();

            LoadSim();
        }

        private void Init() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
            }
        }

        private void LoadSim() {
            SceneManager.LoadScene("Sim");
        }
    }
}
