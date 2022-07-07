using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class CanvasMgr : MonoBehaviour
    {
        [SerializeField] private Canvas m_simCanvas;
        [SerializeField] private Canvas m_resultsCanvas;

        private void Awake() {
            // Activate sim screen
            m_simCanvas.gameObject.SetActive(true);
            m_resultsCanvas.gameObject.SetActive(false);

            EventMgr.SimCanvasSubmitted.AddListener(HandleSimCanvasSubmitted);
            EventMgr.ResultsCanvasOkayed.AddListener(HandleResultsCanvasOkayed);
        }

        private void HandleSimCanvasSubmitted() {
            // close sim canvas
            m_simCanvas.gameObject.SetActive(false);


            // open results canvas
            m_resultsCanvas.gameObject.SetActive(true);
        }

        private void HandleResultsCanvasOkayed() {
            // close results canvas
            m_resultsCanvas.gameObject.SetActive(false);

            // open open canvas
            m_simCanvas.gameObject.SetActive(true);
        }
    }
}
