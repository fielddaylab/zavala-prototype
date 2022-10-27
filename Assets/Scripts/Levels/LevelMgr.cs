using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala;

namespace Zavala {
    public class LevelMgr : MonoBehaviour
    {
        public static LevelMgr Instance;

        [SerializeField] private LevelActivationUI m_activationUI;

        public void Init() {
            Instance = this;

            m_activationUI.Init();
        }
    }
}