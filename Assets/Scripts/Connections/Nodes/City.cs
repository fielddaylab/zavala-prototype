using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Settings;
using static Zavala.Functionalities.Produces;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    [RequireComponent(typeof(BloomAffectable))]
    [RequireComponent(typeof(Inspectable))]
    public class City : MonoBehaviour, IAllVars
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;
        private BloomAffectable m_bloomAffectableComponent;
        private Inspectable m_inspectComponent;

        private bool m_firstCycle; // whether this is first cycle. Produces product for free after first cycle

        [SerializeField] private int m_population;
        [SerializeField] private int m_minPopulation;
        [SerializeField] private int m_cyclesBetweenGrowth;
        private int m_cyclesBeforeGrowth;

        private List<GameObject> m_cityBlocks;

        private static float BLOCK_EXTENTS = 0.06f * 4;

        private void OnEnable() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();
            m_bloomAffectableComponent = this.GetComponent<BloomAffectable>();
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
            m_bloomAffectableComponent.BloomEffect += HandleBloomEffect;

            m_firstCycle = true;

            m_cityBlocks = new List<GameObject>();

            m_cyclesBeforeGrowth = m_cyclesBetweenGrowth;

            // create city blocks for initial population
            for (int p = 0; p < m_population; p++) {
                // SpawnNewCityBlock();
            }

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;
        }

        private void Start() {
            m_inspectComponent.Init();
            m_inspectComponent.SetAdditionalText("Population: " + m_population);
        }

        private void OnDisable() {
            if (m_requestsComponent != null) {
                m_requestsComponent.RequestFulfilled -= HandleRequestFulfilled;
                m_requestsComponent.RequestExpired -= HandleRequestExpired;
            }
            if (m_cyclesComponent != null) {
                m_cyclesComponent.CycleCompleted -= HandleCycleCompleted;
            }
            if (m_bloomAffectableComponent != null) {
                m_bloomAffectableComponent.BloomEffect -= HandleBloomEffect;
            }

            EventMgr.Instance.AllVarsUpdated -= HandleAllVarsUpdated;
        }

        private void StraightToStorage() {
            // produce money per population
            for (int p = 0; p < m_population; p++) {
                m_producesComponent.SetProduceAmt(RegionMgr.Instance.GetRegionByPos(this.transform.position).SimKnobs.CityMaxPayForMilk);
                List<ProductBundle> newProducts = m_producesComponent.Produce();
                if (newProducts == null) {
                    return;
                }

                for (int i = 0; i < newProducts.Count; i++) {
                    if (!m_storesComponent.TryAddToStorage(newProducts[i].Type, newProducts[i].Units)) {
                        Debug.Log("[City] Request fulfilled, but storage full!");
                    }
                    else {
                        m_connectionNodeComponent.UpdateNodeEconomy();
                    }
                }
            }
        }

        private void IncrementPopulation() {
            // TEMP HACK -- don't let population above 4 for UI reasons (TODO: consolidate requests)
            if (m_population == 4) {
                return;
            }

            Debug.Log("[City] Incrementing population...");
            m_population++;

            m_inspectComponent.SetAdditionalText("Population: " + m_population);
            SpawnNewCityBlock();
        }

        private void DecrementPopulation() {
            Debug.Log("[City] Decrementing population...");

            if (m_population == m_minPopulation) {
                Debug.Log("[City] City is already as low as it can be!");
            }
            else {
                m_population--;

                // remove most recent district
                /*
                GameObject toRemove = m_cityBlocks[m_cityBlocks.Count - 1];
                m_cityBlocks.RemoveAt(m_cityBlocks.Count - 1);
                Destroy(toRemove);
                */

                m_inspectComponent.SetAdditionalText("Population: " + m_population);

                m_requestsComponent.CancelLastRequest(m_requestsComponent.RequestBundles);
            }
        }

        private void SpawnNewCityBlock() {
            float randX = UnityEngine.Random.Range(-BLOCK_EXTENTS, BLOCK_EXTENTS);
            float randZ = UnityEngine.Random.Range(-BLOCK_EXTENTS, BLOCK_EXTENTS);

            Debug.Log("[Instantiate] Instantiating CityBlock prefab");
            GameObject newBlock = Instantiate(GameDB.Instance.CityBlockPrefab, this.transform);
            newBlock.transform.position = new Vector3(
                newBlock.transform.position.x + randX,
                newBlock.transform.position.y,
                newBlock.transform.position.z + randZ
                );
            m_cityBlocks.Add(newBlock);
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs args) {
            Debug.Log("[City] Cycle completed");

            m_cyclesBeforeGrowth--;

            // Cities grow by 1 when no requests remain at the end of x cycles
            if (m_requestsComponent.GetNumActiveRequests() == 0) {
                if (m_cyclesBeforeGrowth == 0) {
                    IncrementPopulation();
                    m_cyclesBeforeGrowth = m_cyclesBetweenGrowth;
                }
            }

            m_requestsComponent.QueueRequest(m_population);

            if (m_firstCycle) {
                StraightToStorage();
                m_firstCycle = false;
            }
        }

        private void HandleRequestFulfilled(object sender, EventArgs args) {
            Debug.Log("[City] Request fulfilled");

            StraightToStorage();
        }

        private void HandleRequestExpired(object sender, EventArgs args) {
            Debug.Log("[City] Request expired");

            // city shrinks by 1
            DecrementPopulation();
        }

        private void HandleBloomEffect(object sender, EventArgs args) {
            Debug.Log("[City] Bloom effect triggered");

            DecrementPopulation();
        }

        #endregion // Handlers

        #region All Vars Settings

        public void SetRelevantVars(ref AllVars defaultVars) {
            defaultVars.CityPopStart = m_population;
            defaultVars.CityCycleTime = this.GetComponent<Cycles>().CycleTime;

            defaultVars.CityRequestTimeout = this.GetComponent<Requests>().GetRequestTimeout();
            defaultVars.CityProduceMoneyAmt = this.GetComponent<Produces>().GetProduceAmt();
            defaultVars.CityBloomTolerance = this.GetComponent<BloomAffectable>().GetBloomTolerance();
        }

        public void HandleAllVarsUpdated(object sender, AllVarsEventArgs args) {
            m_population = args.UpdatedVars.CityPopStart;
            CleanUpPrevPop();
            this.GetComponent<Cycles>().CycleTime = args.UpdatedVars.CityCycleTime;

            this.GetComponent<Requests>().SetRequestTimeout(args.UpdatedVars.CityRequestTimeout);
            this.GetComponent<Produces>().SetProduceAmt(args.UpdatedVars.CityProduceMoneyAmt);
            this.GetComponent<BloomAffectable>().SetBloomTolerance(args.UpdatedVars.CityBloomTolerance);

            m_inspectComponent.SetAdditionalText("Population: " + m_population);
        }

        #endregion // All Vars Settings

        #region All Vars Helpers

        private void CleanUpPrevPop() {
            // remove old population blocks
            for (int p = 0; p < m_cityBlocks.Count; p++) {
                Destroy(m_cityBlocks[p]);
            }

            m_cityBlocks = new List<GameObject>();
            // create city blocks for initial population
            for (int p = 0; p < m_population; p++) {
                SpawnNewCityBlock();
            }
        }

        #endregion // All Vars Helpers

    }
}
