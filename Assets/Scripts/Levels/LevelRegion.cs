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
        private struct RegionBounds
        {
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

        [SerializeField] private int m_startingMoney = 1000;
        private int m_moneyUnits;

        private void Awake() {
            EventMgr.Instance.RegionToggled += HandleRegionToggled;
            GridMgr.Init();

            RegionMgr.Instance.TrackRegion(this);

            ResetMoney();

            EventMgr.Instance.ProduceMoney += HandleProduceMoney;
            EventMgr.Instance.PurchaseSuccessful += HandlePurchaseSuccessful;
            EventMgr.Instance.LevelRestarted += HandleLevelRestarted;

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

        private void ResetMoney() {
            m_moneyUnits = 0;
            AddMoney(m_startingMoney);
        }

        private void AddMoney(int units) {
            m_moneyUnits += units;
            Debug.Log("[LevelRegion] Added money!");

            EventMgr.Instance.TriggerEvent(Events.ID.RegionUpdatedMoney, new RegionUpdatedMoneyEventArgs(this));
        }

        private void SpendMoney(int units) {
            m_moneyUnits -= units;

            EventMgr.Instance.TriggerEvent(Events.ID.RegionUpdatedMoney, new RegionUpdatedMoneyEventArgs(this));
        }


        // i.e. return money
        public int GetMoney() {
            return m_moneyUnits;
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

        private void HandleProduceMoney(object sender, ProduceMoneyEventArgs args) {
            if (args.Region != this) { return; }
            AddMoney(args.Amt);
        }

        private void HandlePurchaseSuccessful(object sender, PurchaseSuccessfulEventArgs args) {
            if (args.Region != this) { return; }
            SpendMoney(args.Amt);
        }

        private void HandleLevelRestarted(object sender, EventArgs args) {
            ResetMoney();
        }

        #endregion // Handlers
    }
}