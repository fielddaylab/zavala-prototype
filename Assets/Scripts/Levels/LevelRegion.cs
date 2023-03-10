using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;

namespace Zavala
{
    [RequireComponent(typeof(GridMgr))]
    // [RequireComponent(typeof(ClearingHouse))]
    public class LevelRegion : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_regionBounds;

        [Serializable]
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

        [Serializable]
        public struct SimulationKnobs
        {
            // Percentages
            public SaleTax SaleTaxes;
            public float SittingManureTax;

            // Flat Values
            public int CAFOMinSellThreshold;
            public ImportCosts ImportCosts;
            public int CityMaxPayForMilk;

            public BidBuyPrice BidBuyPrices;
            public BidSellPrice BidSellPrices;
        }

        [Serializable]
        public struct SaleTax
        {
            public float InternalMilk;
            public float InternalManure;
            public float InternalFertilizer;
            public float ExternalManure;
            public float ExternalFertilizer;
        }

        [Serializable]
        public struct ImportCosts
        {
            public float Grain;
            public float Manure;
        }

        [Serializable]
        public struct BidBuyPrice
        {
            public int GrainFarmManure;
            public int GrainFarmFertilizer;
            public int CAFOGrain;
            public int DigesterManure;
            public int StorageManure;
            // public int ExportDepotFertilizer; // <- handled by individual depots
        }

        [Serializable]
        public struct BidSellPrice
        {
            public int GrainFarmGrain;
            public int CAFOManure;
            public int CAFOMilk;
            public int DigesterFertilizer;
        }

        [SerializeField] private GameObject m_regionContainer;
        [SerializeField] private int m_regionNum;
        [SerializeField] private bool m_startsActive;
        [SerializeField] private RegionBounds m_bounds;
        public GridMgr GridMgr;
        [SerializeField] private ClearingHouse m_clearingHouse;

        [SerializeField] private int m_startingMoney = 1000;
        private int m_moneyUnits;

        private bool m_activeRegion;

        [Space(10)]

        [Header("Simulation Knobs")]
        public SimulationKnobs SimKnobs;

        private void Awake() {
            EventMgr.Instance.RegionToggled += HandleRegionToggled;
            GridMgr.Init();
            // m_clearingHouse.Init(this);

            RegionMgr.Instance.TrackRegion(this);

            ResetMoney();

            EventMgr.Instance.ProduceMoney += HandleProduceMoney;
            EventMgr.Instance.PurchaseSuccessful += HandlePurchaseSuccessful;
            EventMgr.Instance.LevelRestarted += HandleLevelRestarted;

            m_activeRegion = true;

            if (!m_startsActive) {
                DeactivateRegion();
            }
        }

        public bool WithinBounds(Vector3 pos) {
            if (m_regionBounds == null) { return false; }

            return m_regionBounds.bounds.Contains(pos);
        }

        public void RegisterWithClearingHouse(StoresProduct storesProduct) {
            m_clearingHouse.RegisterStoresProduct(storesProduct);
        }

        public void RegisterWithClearingHouse(Requests requests) {
            m_clearingHouse.RegisterRequestsProduct(requests);
        }

        public Requests QueryClearingHouseSolution(StoresProduct seller, Resources.Type resourceType, out int unitsSold, out List<RoadSegment> path) {
            return m_clearingHouse.QuerySolution(seller, resourceType, out unitsSold, out path);
        }

        #region Helpers

        private void ActivateRegion() {
            m_activeRegion = true;
            m_regionContainer.SetActive(true);
            RegionMgr.Instance.TrackRegion(this);

            EventMgr.Instance.TriggerEvent(ID.RegionActivationCompleted, EventArgs.Empty);
        }

        private void DeactivateRegion() {
            m_activeRegion = false;
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

        #region Gets & Sets

        public bool IsRegionActive() {
            return m_activeRegion;
        }

        #endregion // Gets & Sets
    }
}