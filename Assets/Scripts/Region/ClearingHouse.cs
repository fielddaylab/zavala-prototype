using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Roads;
using static Zavala.Functionalities.StoresProduct;

namespace Zavala
{
    /// <summary>
    /// Producers and Requesters register with the clearing house.
    /// When new producers or requesters are added to the market, the clearing house solves the equation of optimal matches.
    /// When a product is added to the market, the choice has already been solved.
    /// The optimal match is looked up in the dictionary and routed accordingly for as many as that producer can sell to the requester.
    /// </summary>
    public class ClearingHouse : MonoBehaviour
    {
        private struct DistributionData
        {
            public List<CandidateData> RequestCandidates; // TODO: should be in optimal sorted order
            public int UnitsToDistribute;

            public DistributionData(List<CandidateData> requestCandidates, int unitsToDistribute) {
                RequestCandidates = requestCandidates;
                UnitsToDistribute = unitsToDistribute;
            }
        }

        private struct CandidateData
        {
            public Requests RequestCandidate;
            public int Distance;

            public CandidateData(Requests requestCandidate, int distance) {
                RequestCandidate = requestCandidate;
                Distance = distance;
            }
        }

        // dictionary of producer key with potential requester endpoints
        Dictionary<StoresProduct, DistributionData> RoutingDict;

        // stores product: city, dairy farm, grain farm, digester, storage, skimmer
        private List<StoresProduct> m_registeredStoresProduct;

        // requests product: city, dairy farm, grain farm, digester, storage, export depot
        private List<Requests> m_registeredRequests;

        private LevelRegion m_parentRegion;

        public void Init(LevelRegion parentRegion) {
            m_parentRegion = parentRegion;
            RoutingDict = new Dictionary<StoresProduct, DistributionData>();

            EventMgr.Instance.EconomyUpdated += HandleEconomyUpdated;
        }

        // Register a producer
        public void RegisterStoresProduct(StoresProduct storesProduct) {
            m_registeredStoresProduct.Add(storesProduct);

            RecompileRoutingDict();

            // re-run solution algorithm with new producer
            // Solve();
        }

        // Register a requester
        public void RegisterRequestsProduct(Requests requester) {
            m_registeredRequests.Add(requester);

            RecompileRoutingDict();

            // re-run solution algorithm with new requester
            // Solve();
        }

        // Perform algorithm
        private void Solve() {
            // take each request candidates list and sort according to optimality
            // m_parentRegion.SimKnobs
        }

        // Use results of algorithm to route products from producer to requester.
        // Return optimal buyer and how many units they should/can sell.
        public Requests QuerySolution(StoresProduct seller, out int unitsSold) {
            if (RoutingDict.ContainsKey(seller)) {
                if (RoutingDict[seller].RequestCandidates.Count > 0) {
                    unitsSold = 0; // TODO: store this in RequestCandidateData
                    return RoutingDict[seller].RequestCandidates[0].RequestCandidate;
                }
            }

            // no optimal buyer -- should not occur
            unitsSold = -1;
            return null;
        }

        #region Helpers

        public void RecompileRoutingDict() {
            RoutingDict.Clear();

            foreach (StoresProduct sp in m_registeredStoresProduct) {
                // compile a list of all requesters this StoresProduct can reach via road
                List<CandidateData> requestCandidatesData = new List<CandidateData>();

                // for each registered requester, add to list if can be reached by this StoresProduct
                foreach (Requests candidate in m_registeredRequests) {
                    int pathLength;
                    bool pathExists = QueryIfConnected(sp.GetComponent<ConnectionNode>(), candidate.GetComponent<ConnectionNode>(), out pathLength);
                    if (pathExists) {
                        Debug.Log("[ClearingHouse] Path exists between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);

                        // check if resource types are relevant to each other (supplier stores resource that requester might buy)
                        foreach (Resources.Type storedResource in sp.GetStoredResourceTypes()) {
                            if (candidate.RequestTypes.Contains(storedResource)) {
                                CandidateData candidateData = new CandidateData(candidate, pathLength);
                                requestCandidatesData.Add(candidateData);
                                break;
                            }
                        }
                    }
                    else {
                        Debug.Log("[ClearingHouse] No path exists between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);
                    }
                }

                // consolidate data
                DistributionData distributionData = new DistributionData(requestCandidatesData, sp.StorageCount());

                // TODO: add option to let it sit


                // add new entry
                RoutingDict.Add(sp, distributionData);
            }
        }

        private bool QueryIfConnected(ConnectionNode origin, ConnectionNode endpoint, out int outPathLength) {
            List<RoadSegment> roadsConnectedToOrigin = origin.GetComponent<ConnectionNode>().GetConnectedRoads();
            Debug.Log("[ClearingHouse] Querying road if connected... (" + roadsConnectedToOrigin.Count + " roads connected to origin)");

            bool anyPathFound = false;
            //List<RoadSegment> shortestPath; // uncomment if want to know exact steps along path
            int shortestPathLength = int.MaxValue;
            int currPathLength;

            for (int roadIndex = 0; roadIndex < roadsConnectedToOrigin.Count; roadIndex++) {
                // query the road
                List<RoadSegment> path;
                if (RoadMgr.Instance.QueryRoadForConnection(origin, endpoint, roadsConnectedToOrigin[roadIndex], out path)) {
                    // found endpoint
                    anyPathFound = true;
                    currPathLength = path.Count;

                    if (currPathLength < shortestPathLength) {
                        // shortestPath = path;
                        shortestPathLength = currPathLength;
                    }
                }
            }

            if (anyPathFound) {
                outPathLength = shortestPathLength;
                return true;
            }
            else {
                outPathLength = -1;
                return false;
            }
        }

        #endregion // Helpers

        #region Handlers

        /// <summary>
        /// Update which producers can supply which requesters based on new road conditions
        /// </summary>
        private void HandleRoadUpdated() {

        }

        /// <summary>
        /// Update when producers have new products to ship (refers to new products, NOT new producer/requester)
        /// </summary>
        private void HandleEconomyUpdated(object sender, EconomyUpdatedEventArgs args) {
            if (m_parentRegion != args.Region) { return; }


        }

        #endregion // Handlers
    }
}