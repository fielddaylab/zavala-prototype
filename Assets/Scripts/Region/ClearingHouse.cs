using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Roads;
using Zavala.Tiles;
using static Zavala.Functionalities.StoresProduct;

namespace Zavala.Sim
{
    public enum SimLeverID
    {
        None,
        RunoffPenalty,
        ExportTax,
        Skimmers,
        GooseRace,
        ImportTax,
        HaltOperations
    }

    /// <summary>
    /// Producers and Requesters register with the clearing house.
    /// When new producers or requesters are added to the market, the clearing house solves the equation of optimal matches.
    /// When a product is added to the market, the choice has already been solved.
    /// The optimal match is looked up in the dictionary and routed accordingly for as many as that producer can sell to the requester.
    /// </summary>
    [RequireComponent(typeof(Cycles))]
    public class ClearingHouse : MonoBehaviour
    {
        private Cycles m_cyclesComponent;

        private struct DistributionData
        {
            public List<CandidateData> RequestCandidates; // staging
            public PriorityQueue<CandidateData, float> OptimalRouting; // prioritizing
            public List<CDataPriorityPair> OptimalList; // enumerating
            public int UnitsToDistribute;

            public DistributionData(List<CandidateData> requestCandidates, PriorityQueue<CandidateData, float> optimalRouting, List<CDataPriorityPair> optimalList, int unitsToDistribute) {
                RequestCandidates = requestCandidates;
                OptimalRouting = optimalRouting;
                OptimalList = optimalList;
                UnitsToDistribute = unitsToDistribute;
            }
        }

        private struct CandidateData
        {
            public Requests RequestCandidate;
            public int Distance;
            public List<RoadSegment> Path;
            public LevelRegion ParentRegion;

            public CandidateData(Requests requestCandidate, int distance, List<RoadSegment> path, LevelRegion parentRegion) {
                RequestCandidate = requestCandidate;
                Distance = distance;
                Path = path;
                ParentRegion = parentRegion;
            }
        }

        private struct CDataPriorityPair {
            public CandidateData CData;
            public float Priority;

            public CDataPriorityPair(CandidateData cData, float priority) {
                CData = cData;
                Priority = priority;
            }
        }

        // dictionary of producer key with potential requester endpoints
        Dictionary<StoresProduct, DistributionData> RoutingDict;

        // stores product: city, dairy farm, grain farm, digester, storage, skimmer
        private List<StoresProduct> m_registeredStoresProduct;

        // requests product: city, dairy farm, grain farm, digester, storage, export depot
        private List<Requests> m_registeredRequests;

        // private LevelRegion m_parentRegion;

        private void Awake() {
            Init();
        }

        private void Start() {
            EventMgr.Instance.RegionActivationCompleted += HandleRegionActivationCompleted;
        }

        public void Init() {
            m_cyclesComponent = this.GetComponent<Cycles>();

            m_registeredRequests = new List<Requests>();
            m_registeredStoresProduct = new List<StoresProduct>();

            // m_parentRegion = parentRegion;
            RoutingDict = new Dictionary<StoresProduct, DistributionData>();

            EventMgr.Instance.EconomyUpdated += HandleEconomyUpdated;

            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;
        }

        // Register a producer
        public void RegisterStoresProduct(StoresProduct storesProduct) {
            Debug.Log("[ClearingHouse] Registering a StoresProduct.");

            m_registeredStoresProduct.Add(storesProduct);

            // re-run solution algorithm with new producer
            // Solve();
        }

        // Register a requester
        public void RegisterRequestsProduct(Requests requester) {
            Debug.Log("[ClearingHouse] Registering a Requests.");

            m_registeredRequests.Add(requester);

            // re-run solution algorithm with new requester
            // Solve();
        }

        private void RecompileRoutingDict() {
            RoutingDict.Clear();

            foreach (StoresProduct sp in m_registeredStoresProduct) {
                // compile a list of all requesters this StoresProduct can reach via road
                List<CandidateData> requestCandidatesData = new List<CandidateData>();

                // for each registered requester, add to list if can be reached by this StoresProduct
                foreach (Requests candidate in m_registeredRequests) {
                    if (candidate == null) {
                        m_registeredRequests.Remove(candidate);
                        continue;
                    }
                    Debug.Log("[ClearingHouse] [Compile] StoresProduct road count: " + sp.GetComponent<ConnectionNode>().GetConnectedRoads().Count);
                    Debug.Log("[ClearingHouse] [Compile] Requests road count: " + candidate.GetComponent<ConnectionNode>().GetConnectedRoads().Count);

                    int pathLength;
                    List<RoadSegment> path;
                    bool pathExists = QueryIfConnected(sp.GetComponent<ConnectionNode>(), candidate.GetComponent<ConnectionNode>(), out pathLength, out path);
                    if (pathExists) {
                        Debug.Log("[ClearingHouse] Path exists between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);

                        // check if resource types are relevant to each other (supplier stores resource that requester might buy)
                        // What the supplier currently stores
                        foreach (Resources.Type storedResource in sp.GetStoredResourceTypes()) {
                            Debug.Log("[ClearingHouse] Checking for resource (" + storedResource + ") match between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name + "...");
                            if (candidate.RequestBundlesContains(storedResource)) {
                                CandidateData candidateData = new CandidateData(candidate, pathLength, path, RegionMgr.Instance.GetRegionByPos(candidate.transform.position));
                                requestCandidatesData.Add(candidateData);

                                Debug.Log("[ClearingHouse] Resource (" + storedResource + ") match between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);
                                break;
                            }
                        }
                        // What the supplier could produce and store
                        if (sp.GetComponent<Produces>() != null) {
                            foreach (Resources.Type produceResource in sp.GetComponent<Produces>().GetProduceTypes()) {
                                Debug.Log("[ClearingHouse] Checking for resource (" + produceResource + ") match between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name + "...");
                                if (candidate.RequestBundlesContains(produceResource)) {
                                    CandidateData candidateData = new CandidateData(candidate, pathLength, path, RegionMgr.Instance.GetRegionByPos(candidate.transform.position));
                                    requestCandidatesData.Add(candidateData);

                                    Debug.Log("[ClearingHouse] Resource (" + produceResource + ") match between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        Debug.Log("[ClearingHouse] No path exists between supplier " + sp.gameObject.name + " and requester " + candidate.gameObject.name);
                    }
                }

                // option to let it sit
                if (sp.SitOption != null) {
                    // check if the resource can be let to sit at this location
                    foreach (Resources.Type storedResource in sp.GetStoredResourceTypes()) {
                        if (sp.SitOption.RequestsComp.RequestBundlesContains(storedResource)) {
                            CandidateData candidateData = new CandidateData(sp.SitOption.RequestsComp, 0, null, RegionMgr.Instance.GetRegionByPos(sp.SitOption.transform.position));
                            requestCandidatesData.Add(candidateData);
                            break;
                        }
                    }
                }

                // TODO: option for import from outside? Currently occurs only when a request goes unmet

                // consolidate data
                DistributionData distributionData = new DistributionData(requestCandidatesData, new PriorityQueue<CandidateData, float>(), new List<CDataPriorityPair>(), sp.StorageCount());

                // add new entry
                RoutingDict.Add(sp, distributionData);
            }
        }

        // Perform algorithm
        private void Solve() {
            Debug.Log("[ClearingHouse] [Solve] Solving.");
            // Take each request candidates list and add to priority queue using cost equation
            foreach(StoresProduct supplier in m_registeredStoresProduct) {
                // Obtain distribution data, i.e. all possible places they could ship product to plus additional info
                DistributionData distData = RoutingDict[supplier];

                List<CandidateData> duplicateCandidateTracker = new List<CandidateData>();

                // Iterate through candidates and assign an optimality score
                for (int c = 0; c < distData.RequestCandidates.Count; c++) {
                    float optimalityScore = Mathf.Infinity;
                    CandidateData candData = distData.RequestCandidates[c];

                    if (duplicateCandidateTracker.Contains(candData)) {
                        continue;
                    }
                    else {
                        duplicateCandidateTracker.Add(candData);
                    }

                    switch (supplier.GetSupplierType()) {
                        case SupplierType.GrainFarm:
                            // Grain farm is selling
                            // Product: Grain
                            optimalityScore = GrainSellGrain(candData);
                            break;
                        case SupplierType.DairyFarm:
                            // Diary farm is selling
                            // Product: Manure
                            float manureScore = DairySellManure(candData);
                            // Product: Milk
                            float milkScore = DairySellMilk(candData);

                            optimalityScore = Math.Max(manureScore, milkScore);
                            break;
                        case SupplierType.Digester:
                            // Digester is selling
                            // Product: Fertilizer
                            optimalityScore = DigesterSellFertilizer(candData);
                            break;
                        case SupplierType.Storage:
                            // Storage is selling
                            // Product: Manure
                            optimalityScore = StorageSellManure(candData);
                            break;
                            /*
                        case SupplierType.Skimmer:
                            // Skimmer is selling
                            // Product: Fertilizer
                            break;
                        */
                        /*
                        case SupplierType.Importer:
                            Debug.Log("[ClearingHouse] [Solve] Supplier is importer. Object name: " + supplier.gameObject.name);
                            
                            break;
                        */
                        case SupplierType.Unknown:
                            Debug.Log("[ClearingHouse] [Solve] Unknown type of supplier. Object name: " + supplier.gameObject.name);

                            break;
                        default:
                            Debug.Log("[ClearingHouse] [Solve] Unknown type of supplier. Object name: " + supplier.gameObject.name);

                            break;
                    }

                    // TODO: negotiate price between buy price and sell price, factor into optimality

                    Debug.Log("[ClearingHouse] [Solve] Enqueueing " + candData.RequestCandidate.gameObject.name + " with final priority of " + (-optimalityScore) +  " for supplier " + supplier.gameObject.name);
                    // Add candidate and score to priority queue for this supplier
                    // higher offer means lower priority val, which means closer to front of queue (i.e. score of 1 is more optimal than score of 10)
                    distData.OptimalRouting.Enqueue(candData, -optimalityScore);
                }

                Debug.Log("[ClearingHouse] [Solve] Transferring priority queue to list. Queue size: " + distData.OptimalRouting.Count);
                // Transfer priority queue to list
                while (distData.OptimalRouting.Count > 0) {
                    float priority = distData.OptimalRouting.PeekPriority();
                    distData.OptimalList.Add(new CDataPriorityPair(distData.OptimalRouting.Dequeue(), priority));
                }

                OutputSolution();
            }
        }

        // Use results of algorithm to route products from producer to requester.
        // Return optimal buyer and how many units they should/can sell.
        // May be called multiple time to distribute all inventory to multiple buyers.
        public Requests QuerySolution(StoresProduct seller, Resources.Type resourceType, out int unitsSold, out List<RoadSegment> path) {
            Debug.Log("[ClearingHouse] [Solve] Solution queried...");
            
            if (RoutingDict.ContainsKey(seller)) {
                Debug.Log("[ClearingHouse] [Solve] Seller exists in routing dict");
                DistributionData distData = RoutingDict[seller];
                // iterate through possible buyers, starting with most to least optimal
                if (distData.OptimalList.Count > 0) {
                    Debug.Log("[ClearingHouse] [Solve] Optimal list exists. Size: " + distData.OptimalList.Count);
                    int sellUnitsRemaining = seller.StorageCount(resourceType);

                    for (int c = 0; c < distData.OptimalList.Count; c++) {
                        int unitsRequesting = distData.OptimalList[c].CData.RequestCandidate.SingleRequestUnits(resourceType);
                        Debug.Log("[ClearingHouse] found optimal buyer of " + resourceType + " in list for " + seller.gameObject.name + ", requesting " + unitsRequesting + " units: " + distData.OptimalList[c].CData.RequestCandidate.gameObject.name);
                        
                        if (unitsRequesting > 0) {
                            Debug.Log("[ClearingHouse] [Solve] Sold to optimal buyer, buying " + unitsRequesting + " units");
                            // sell as many units as possible to this buyer
                            unitsSold = sellUnitsRemaining >= unitsRequesting ? unitsRequesting : sellUnitsRemaining;
                            path = distData.OptimalList[c].CData.Path;
                            return RoutingDict[seller].OptimalList[c].CData.RequestCandidate;
                        }
                    }
                }
                else {
                    Debug.Log("[ClearingHouse] [Solve] Optimal list does not exist!");
                }
            }

            Debug.Log("[ClearingHouse] [Solve] No optimal buyer.");
            // no optimal buyer (likely not connected to roads)
            unitsSold = 0;
            path = null;
            return null;
        }

        #region Solve Helpers

        /// <summary>
        /// Equation of optimality: offer - costs
        /// offer = (willing-to-pay)
        /// costs = (tiles traveled) * (price per tile) + (purchase tax) + (export tax)
        /// </summary>

        private float GrainSellGrain(CandidateData candData) {
            float offer;
            float costs;

            float willingToPay;

            int tilesTraveled;
            float pricePerTile;
            float purchaseTax = 0;
            float exportTax = 0;

            if (candData.RequestCandidate.GetComponent<DairyFarm>() != null) {
                // Apply DairyFarm Grain offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.CAFOGrain;
            }
            else {
                // Unknown type of offer! Do not sell!
                willingToPay = -Mathf.Infinity;
            }

            tilesTraveled = candData.Distance;
            pricePerTile = RegionMgr.Instance.GlobalSimKnobs.TransportCosts.Grain;
            // purchaseTax // no purchase tax specified for grain
            // exportTax // no export tax specified for grain

            offer = willingToPay;
            costs = tilesTraveled * pricePerTile + purchaseTax + exportTax;

            return offer - costs;
        }

        private float DairySellManure(CandidateData candData) {
            float offer;
            float costs;

            float willingToPay;

            int tilesTraveled;
            float pricePerTile;
            float purchaseTax = 0;
            float exportTax = 0;

            if (candData.RequestCandidate.GetComponent<GrainFarm>() != null) {
                // Apply GrainFarm Manure offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.GrainFarmManure;
            }
            else if (candData.RequestCandidate.GetComponent<Digester>() != null) {
                // Apply Digester Manure offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.DigesterManure;
            }
            else if (candData.RequestCandidate.GetComponent<Storage>() != null) {
                // Offer is probably 0, just moving locations
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.StorageManure;
            }
            else if (candData.RequestCandidate.GetComponent<LetItSit>() != null) {
                Debug.Log("[ClearingHouse] Letting it sit option being considered...");
                // Apply Letting Manure sit tax offer
                willingToPay = -candData.ParentRegion.SimKnobs.SittingManureTax;
            }
            else {
                // Unknown type of offer! Do not sell!
                willingToPay = Mathf.NegativeInfinity;
            }

            Requests candidate = candData.RequestCandidate;
            if (candidate.GetComponent<ExportDepot>() != null) {
                // Apply external manure tax
                exportTax = candData.ParentRegion.SimKnobs.SaleTaxes.ExternalManure;
            }
            /* TODO:
            else if (requester region != supplier region) {
                // Apply external manure tax
                exportTax = candData.ParentRegion.SimKnobs.SaleTaxes.ExternalManure;
            }
            */
            else if (candData.RequestCandidate.GetComponent<Storage>() != null) {
                // no purchase tax necessary, just moving locations
            }
            else {
                // Apply internal manure tax
                purchaseTax = candData.ParentRegion.SimKnobs.SaleTaxes.InternalManure;
            }

            tilesTraveled = candData.Distance;
            pricePerTile = RegionMgr.Instance.GlobalSimKnobs.TransportCosts.Manure;

            offer = willingToPay;
            costs = tilesTraveled * pricePerTile + purchaseTax + exportTax;

            return offer - costs;
        }

        private float DairySellMilk(CandidateData candData) {
            float offer;
            float costs;

            float willingToPay;

            int tilesTraveled;
            float pricePerTile;
            float purchaseTax = 0;
            float exportTax = 0;

            if (candData.RequestCandidate.GetComponent<City>() != null) {
                // Apply City Milk offer
                // TODO: clarify: is this a region-wide value, or does each city have it's own milk tax you can adjust?
                willingToPay = candData.ParentRegion.SimKnobs.CityMaxPayForMilk; // <- region-wide value
            }
            else {
                // Unknown type of offer! Do not sell!
                willingToPay = Mathf.NegativeInfinity;
            }

            Requests candidate = candData.RequestCandidate;
            if (candidate.GetComponent<ExportDepot>() != null) {
                // No external tax specified for milk
            }
            /* TODO:
            else if (requester region != supplier region) {
                // No external tax specified for milk
            }
            */
            else {
                // Apply internal milk tax
                purchaseTax = candData.ParentRegion.SimKnobs.SaleTaxes.InternalMilk;
            }

            tilesTraveled = candData.Distance;
            pricePerTile = RegionMgr.Instance.GlobalSimKnobs.TransportCosts.Milk;

            offer = willingToPay;
            costs = tilesTraveled * pricePerTile + purchaseTax + exportTax;

            return offer - costs;
        }

        private float DigesterSellFertilizer(CandidateData candData) {
            float offer;
            float costs;

            float willingToPay;

            int tilesTraveled;
            float pricePerTile;
            float purchaseTax = 0;
            float exportTax = 0;

            Requests candidate = candData.RequestCandidate;
            if (candidate.GetComponent<GrainFarm>() != null) {
                // Apply GrainFarm Fertilizer offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.GrainFarmFertilizer;
            }
            else if (candidate.GetComponent<ExportDepot>() != null) {
                // Apply ExportDepot Fertilizer offer
                willingToPay = candidate.GetComponent<ExportDepot>().RegionFertilizerOffer;
            }
            else {
                // Unknown type of offer! Do not sell!
                willingToPay = Mathf.NegativeInfinity;
            }

            if (candidate.GetComponent<ExportDepot>() != null) {
                // Apply external fertilizer tax
                exportTax = candData.ParentRegion.SimKnobs.SaleTaxes.ExternalFertilizer;
            }
            else {
                // Apply internal fertilizer tax
                purchaseTax = candData.ParentRegion.SimKnobs.SaleTaxes.InternalFertilizer;
            }

            tilesTraveled = candData.Distance;
            pricePerTile = RegionMgr.Instance.GlobalSimKnobs.TransportCosts.Fertilizer;

            offer = willingToPay;
            costs = tilesTraveled * pricePerTile + purchaseTax + exportTax;

            return offer - costs;
        }

        private float StorageSellManure(CandidateData candData) {
            float offer;
            float costs;

            float willingToPay;

            int tilesTraveled;
            float pricePerTile;
            float purchaseTax = 0;
            float exportTax = 0;

            if (candData.RequestCandidate.GetComponent<GrainFarm>() != null) {
                // Apply GrainFarm Manure offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.GrainFarmManure;
            }
            else if (candData.RequestCandidate.GetComponent<Digester>() != null) {
                // Apply Digester Manure offer
                willingToPay = candData.ParentRegion.SimKnobs.BidBuyPrices.DigesterManure;
            }
            else {
                // Unknown type of offer! Do not sell!
                willingToPay = -Mathf.Infinity;
            }

            Requests candidate = candData.RequestCandidate;
            if (candidate.GetComponent<ExportDepot>() != null) {
                // Apply external manure tax
                exportTax = candData.ParentRegion.SimKnobs.SaleTaxes.ExternalManure;
            }
            /* TODO:
            else if (requester region != supplier region) {
                // Apply external manure tax
                exportTax = candData.ParentRegion.SimKnobs.SaleTaxes.ExternalManure;
            }
            */
            else {
                // Apply internal manure tax
                purchaseTax = candData.ParentRegion.SimKnobs.SaleTaxes.InternalManure;
            }

            tilesTraveled = candData.Distance;
            pricePerTile = RegionMgr.Instance.GlobalSimKnobs.TransportCosts.Manure;

            offer = willingToPay;
            costs = tilesTraveled * pricePerTile + purchaseTax + exportTax;

            return offer - costs;
        }

        #endregion // Solve Helpers

        #region Helpers

        public bool QueryIfConnected(ConnectionNode origin, ConnectionNode endpoint, out int outPathLength, out List<RoadSegment> outPath) {
            List<RoadSegment> roadsConnectedToOrigin = origin.GetComponent<ConnectionNode>().GetConnectedRoads();
            Debug.Log("[ClearingHouse] Querying road if connected... (" + roadsConnectedToOrigin.Count + " roads connected to origin)");

            bool anyPathFound = false;
            List<RoadSegment> shortestPath = new List<RoadSegment>();
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
                        shortestPath = path;
                        shortestPathLength = currPathLength;
                    }
                }
            }

            if (anyPathFound) {
                outPathLength = shortestPathLength;
                outPath = shortestPath;
                return true;
            }
            else {
                // Check if on same tile
                GridMgr currMgr = RegionMgr.Instance.CurrRegion.GridMgr;
                if (currMgr.TileAtPos(origin.transform.position) == currMgr.TileAtPos(endpoint.transform.position)) {
                    outPathLength = 0;
                    outPath = null;
                    return true;
                }

                // nothing found
                outPathLength = -1;
                outPath = null;
                return false;
            }
        }

        #endregion // Helpers

        #region Debug

        private void OutputSolution() {
            Debug.Log("---------------- START Clearing House Solution ---------------- [ClearingHouseOutput]");

            string listOutput = "[ClearingHouseOutput]";

            foreach (StoresProduct supplier in RoutingDict.Keys) {
                listOutput += "\nOptimal buyer list for seller " + supplier.gameObject.name + " :";
                listOutput += "\n - (Name || $Net Profit)";

                DistributionData data = RoutingDict[supplier];
                
                for(int i = 0; i < data.OptimalList.Count; i++) {
                    string candidateOutput = "\n - " + "";

                    candidateOutput += data.OptimalList[i].CData.RequestCandidate.gameObject.name + " || $" + (-data.OptimalList[i].Priority);

                    listOutput += candidateOutput;
                }   
            }

            Debug.Log(listOutput);

            Debug.Log("---------------- END Clearing House Solution ---------------- [ClearingHouseOutput]");
        }

        #endregion // Debug

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[ClearingHouse] Cycle completed");

            // Clearing House recompiles Dictionary, solves problem
            RecompileRoutingDict();
            Solve();
        }

        /// <summary>
        /// Update which producers can supply which requesters based on new road conditions
        /// </summary>
        private void HandleRoadUpdated() {

        }

        /// <summary>
        /// Update when producers have new products to ship (refers to new products, NOT new producer/requester)
        /// </summary>
        private void HandleEconomyUpdated(object sender, EconomyUpdatedEventArgs args) {
            Debug.Log("[ClearingHouse] Economy updated");
        }

        private void HandleRegionActivationCompleted(object sender, EventArgs args) {
            RecompileRoutingDict();
        }

        #endregion // Handlers
    }
}