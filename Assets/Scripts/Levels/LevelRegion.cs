using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;

namespace Zavala
{
    public class LevelRegion : MonoBehaviour
    {
        [Serializable] // TODO: could change this to a polygon collider if we need more granularity
        private struct RegionBounds {
            public float Left;
            public float Top;
            public float Right;
            public float Bottom;

            public RegionBounds(float left, float top, float right, float bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        [SerializeField] private GameObject m_regionContainer;
        [SerializeField] private int m_regionNum;
        [SerializeField] private bool m_startsActive;
        [SerializeField] private RegionBounds m_bounds;
        public GridMgr GridMgr;

        private void Awake() {
            EventMgr.Instance.RegionToggled += HandleRegionToggled;
            GridMgr.Init();

            RegionMgr.Instance.TrackRegion(this);
            if (!m_startsActive) {
                DeactivateRegion();
            }
        }

        public bool WithinBounds(Vector3 pos) {
            // z is left, -z is right, x is up, -x is down (I know, it's bad) 
            if (pos.z < m_bounds.Left && pos.z > m_bounds.Right && pos.x < m_bounds.Top && pos.x > m_bounds.Bottom) {
                return true;
            }
            /*
            if (!(pos.z < m_bounds.Left)) {
                Debug.Log("[LevelRegion] " + pos + " failed Left");
            }
            if (!(pos.z > m_bounds.Right)) {
                Debug.Log("[LevelRegion] " + pos + " failed Right");
            }
            if (!(pos.x < m_bounds.Top)) {
                Debug.Log("[LevelRegion] " + pos + " failed Top");
            }
            if (!(pos.x > m_bounds.Bottom)) {
                Debug.Log("[LevelRegion] " + pos + " failed Bottom: pos x " + pos.x + " is less than or equal to " + m_bounds.Bottom);
            }
            */
            return false;
        }

        #region Helpers

        private void ActivateRegion() {
            m_regionContainer.SetActive(true);
            RegionMgr.Instance.TrackRegion(this);
        }

        private void DeactivateRegion() {
            m_regionContainer.SetActive(false);
            RegionMgr.Instance.UntrackRegion(this);
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