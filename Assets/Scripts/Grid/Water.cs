using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Tile))]
    public class Water : MonoBehaviour
    {
        [SerializeField] private int m_greenifyRate;
        [SerializeField] private GameObject m_bloomObj;
        [SerializeField] private int m_bloomThreshold;
        private Color m_baseColor;

        private MeshRenderer m_rendererComponent;
        private Tile m_tileComponent;

        private void Awake() {
            m_rendererComponent = GetComponent<MeshRenderer>();

            m_tileComponent = GetComponent<Tile>();
            m_tileComponent.OnPhosphRefresh += HandlePhosphRefresh;

            m_baseColor = m_rendererComponent.material.color;

            m_bloomObj.SetActive(false);
        }

        public bool TrySkim(int skimAmt) {
            bool skimmedAny = false;
            for (int p = 0; p < skimAmt; p++) {
                if (m_tileComponent.TrySkimPip()) {
                    skimmedAny = true;
                }
            }
            return skimmedAny;
        }

        #region Handlers

        private void HandlePhosphRefresh(object sender, EventArgs args) {
            float greenDelta = m_greenifyRate / 255f * m_tileComponent.GetPipCount();
            Color newColor = new Color(m_baseColor.r, Mathf.Min((m_baseColor.g + greenDelta), 1), Mathf.Max(m_baseColor.b - greenDelta, 0.5f), m_baseColor.a);
            m_rendererComponent.material.color = newColor;

            if (m_tileComponent.GetPipCount() >= m_bloomThreshold) {
                m_bloomObj.SetActive(true);
            }
            else {
                m_bloomObj.SetActive(false);
            }
        }

        #endregion // Handlers
    }
}
