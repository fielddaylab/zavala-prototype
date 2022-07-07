using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class ResultsMgr : MonoBehaviour
    {
        [SerializeField] private Button m_okayButton;

        private void Awake() {
            m_okayButton.onClick.AddListener(delegate { EventMgr.ResultsCanvasOkayed?.Invoke(); });
        }

        private void OnDestroy() {
            m_okayButton.onClick.RemoveAllListeners();
        }
    }
}