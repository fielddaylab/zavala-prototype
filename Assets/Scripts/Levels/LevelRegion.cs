using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;

namespace Zavala
{
    public class LevelRegion : MonoBehaviour
    {
        [SerializeField] private GameObject m_regionContainer;
        [SerializeField] private int m_regionNum;
        [SerializeField] private bool m_startsActive;

        private void Awake() {
            EventMgr.Instance.RegionToggled += HandleRegionToggled;

            if (!m_startsActive) {
                DeactivateRegion();
            }
        }

        #region Helpers

        private void ActivateRegion() {
            m_regionContainer.SetActive(true);
        }

        private void DeactivateRegion() {
            m_regionContainer.SetActive(false);
        }

        #endregion // Helpers

        #region Handlers

        private void HandleRegionToggled(object sender, RegionToggleEventArgs args) {
            if (args.RegionNum == m_regionNum) {
                if (m_regionContainer.activeSelf) {
                    DeactivateRegion();
                }
                else {
                    ActivateRegion();
                }
            }
        }

        #endregion // Handlers
    }
}