using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Roads;
using Zavala.Settings;
using Zavala.Tiles;

namespace Zavala
{
    public class Road : MonoBehaviour, IAllVars
    {
        private List<ConnectionNode> m_end1Nodes; // arbitrary start
        private List<ConnectionNode> m_end2Nodes; // arbitrary end

        //private List<RoadSegment> m_segments;
        private List<Tile> m_tileSegments;
        private List<RoadSegment_OLD> m_roadSegments;

        public event EventHandler EconomyUpdated;

        [SerializeField] private float m_roadStartHealth;
        private float m_baseHealth; // road health
        private float m_currHealth; // road health
        private bool m_isUsable;
        private bool m_inDisrepair;

        [SerializeField] private float m_disrepairThreshold;


        private void Awake() {
            m_roadSegments = new List<RoadSegment_OLD>();
            m_inDisrepair = false;

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;
            ProcessUpdatedVars(SettingsMgr.Instance.GetCurrAllVars());
        }

        private void OnDisable() {
            EventMgr.Instance.AllVarsUpdated -= HandleAllVarsUpdated;
        }

        #region Road Creation

        /*
        public void SetStartConnectionNodes(List<ConnectionNode> nodes) {
            m_end1Nodes = nodes;
        }

        public void SetEndConnectionNodes(List<ConnectionNode> nodes) {
            m_end2Nodes = nodes;
        }

        public void SetSegments(List<Tile> segments) {
            m_tileSegments = new List<Tile>();

            for (int i = 0; i < segments.Count; i++) {
                m_tileSegments.Add(segments[i]);
            }
        }

        public void FinalizeConnections() {
            FinalizeConnectionsInList(m_end1Nodes);
            FinalizeConnectionsInList(m_end2Nodes);

            m_isUsable = true;

            // set health
            m_baseHealth = m_currHealth = m_roadStartHealth;

            UpdateEconomy();
        }

        private void FinalizeConnectionsInList(List<ConnectionNode> nodeList) {
            for (int i = 0; i < nodeList.Count; i++) {
                nodeList[i].AddRoad(this);
            }
        }

        public void NormalizeSegmentHeights() {
            float highestHeight = 0;
            for (int i = 0; i < m_roadSegments.Count; i++) {
                if (m_roadSegments[i].gameObject.transform.position.y > highestHeight) {
                    highestHeight = m_roadSegments[i].gameObject.transform.position.y;
                }
            }

            for (int i = 0; i < m_roadSegments.Count; i++) {
                Vector3 newPos = m_roadSegments[i].gameObject.transform.position;
                newPos.y = highestHeight;
                m_roadSegments[i].gameObject.transform.position = newPos;
            }
        }

        
        // prev tile exists
        public void ConstructRoadSegmentInstance(GameObject tileUnderRoadObj, bool malleableDir) {
            RoadSegment_OLD prevSegment;
            if (m_roadSegments.Count > 0) {
                prevSegment = m_roadSegments[m_roadSegments.Count - 1];
            }
            else {
                prevSegment = null;
            }
            RoadSegmentType placeType = prevSegment == null ? RoadSegmentType.End : RoadSegmentType.Straight;
            GameObject toPlace = RoadMgr.Instance.GetRoadSegmentPrefab();

            Debug.Log("[Instantiate] Instantiating road segment prefab");
            RoadSegment_OLD roadSegmentInstance = Instantiate(toPlace, tileUnderRoadObj.transform).GetComponent<RoadSegment_OLD>();
            roadSegmentInstance.MalleableDir = malleableDir;
            roadSegmentInstance.ModifySegmentType(placeType);

            if (prevSegment != null) {
                // orient prev tile if malleable
                if (prevSegment.MalleableDir) {
                    RoadBuildDir prevDir = CalcBuildDirByPos(prevSegment.transform.position, roadSegmentInstance.transform.position);
                    prevSegment.ModifyBuildDir(prevDir, prevDir);
                }

                // orient this tile
                RoadBuildDir currDir = CalcBuildDirByPos(prevSegment.transform.position, roadSegmentInstance.transform.position);
                roadSegmentInstance.ModifyBuildDir(currDir, currDir);

                // determine prev tile type
                if (prevSegment.SegmentType != RoadSegmentType.End) {
                    Debug.Log("[Curving] Prev Dir: " + prevSegment.BuildDir + " || Curr Dir: " + currDir);
                    Debug.Log("[Curving] Difference: " + ((int)currDir - (int)prevSegment.BuildDir));
                    if (Mathf.Abs((int)currDir - (int)prevSegment.BuildDir) == 0 || Mathf.Abs((int)currDir - (int)prevSegment.BuildDir) == 3) {
                        Debug.Log("[Curving] Road should be straight");
                        // straight
                        if (prevSegment.SegmentType != RoadSegmentType.Straight) {
                            // convert
                            ConvertRoadSegment(RoadSegmentType.Straight, m_roadSegments.Count - 1, currDir);
                        }
                    }
                    else if (Mathf.Abs((int)currDir - (int)prevSegment.BuildDir) == 2 || Mathf.Abs((int)currDir - (int)prevSegment.BuildDir) == 4) {
                        Debug.Log("[Curving] Road should be tight curve");
                        // tight corner -- roundabout for now
                        if (prevSegment.SegmentType != RoadSegmentType.TightBend) {
                            // convert
                            ConvertRoadSegment(RoadSegmentType.TightBend, m_roadSegments.Count - 1, currDir);
                        }
                    }
                    else {
                        // curved
                        Debug.Log("[Curving] Road should be curved");
                        if (prevSegment.SegmentType != RoadSegmentType.Bend) {
                            // convert
                            ConvertRoadSegment(RoadSegmentType.Bend, m_roadSegments.Count - 1, currDir);
                        }
                    }
                }
            }

            m_roadSegments.Add(roadSegmentInstance);
        }

        */

        

        /*
        public RoadSegmentType GetSegmentType(int roadSegmentIndex) {
            return m_roadSegments[roadSegmentIndex].SegmentType;
        }
        */

        /*
        public void OrientRoadSegment(int roadSegmentIndex, RoadBuildDir dir) {
            m_roadSegments[roadSegmentIndex].ModifyBuildDir(dir, dir);
        }
        */

        /*
        public void ConvertRoadSegment(RoadSegmentType placeType, int roadSegmentIndex) {
            RoadSegment_OLD toConvert = m_roadSegments[roadSegmentIndex];

            RoadBuildDir prevDir;
            RoadBuildDir newDir;
            if (m_roadSegments.Count > 1) {
                prevDir = m_roadSegments[roadSegmentIndex - 1].BuildDir;
                newDir = prevDir;
            }
            else {
                prevDir = toConvert.BuildDir;
                newDir = 0;
            }

            if (placeType == RoadSegmentType.End && m_roadSegments.Count > 1) {
                // flip dir for final endpoint
                newDir = (RoadBuildDir)(((int)newDir + 3) % 6);
            }
            toConvert.ModifySegmentType(placeType);
            toConvert.ModifyBuildDir(newDir, prevDir);
        }
        */

        /*
        public void ConvertRoadSegment(RoadSegmentType placeType, int roadSegmentIndex, RoadBuildDir newDir) {
            RoadSegment_OLD toConvert = m_roadSegments[roadSegmentIndex];

            RoadBuildDir prevDir = toConvert.BuildDir;

            //m_roadSegments.RemoveAt(m_roadSegments.Count - 1);
            //Destroy(toConvert);

            //GameObject toPlace = RoadMgr.Instance.GetRoadPrefab(placeType);

            //RoadSegment roadSegmentInstance = Instantiate(toPlace, tileUnderRoadObj.transform).GetComponent<RoadSegment>();
            if (placeType == RoadSegmentType.End) {
                // flip dir for final endpoint
                newDir = (RoadBuildDir)(((int)newDir + 3) % 6);
            }
            toConvert.ModifySegmentType(placeType);
            toConvert.ModifyBuildDir(newDir, prevDir);
            //m_roadSegments.Add(roadSegmentInstance);
        }
        */

        /*
        public void RemoveRoadSegmentInstance(int segmentIndex) {
            GameObject segmentInstance = m_roadSegments[segmentIndex].gameObject;
            m_roadSegments.RemoveAt(segmentIndex);
            Destroy(segmentInstance);
        }
        */

        #endregion // Road Creation

        #region Queries

        // Wether a connected node has the specified resource in storage
        public bool ResourceOnRoad(Resources.Type resourceType, GameObject requester) {
            if (!m_isUsable) {
                return false;
            }
            if (ResourceInList(m_end1Nodes, resourceType, requester)) {
                return true;
            }
            if (ResourceInList(m_end2Nodes, resourceType, requester)) {
                return true;
            }

            return false;
        }

        public StoresProduct GetSupplierOnRoad(Resources.Type resourceType, out Resources.Type foundResourceType) {
            if (!m_isUsable) {
                foundResourceType = Resources.Type.None;
                return null;
            }
            StoresProduct supplier = GetSupplierInList(m_end1Nodes, resourceType, out foundResourceType);
            if (supplier != null) {
                return supplier;
            }
            supplier = GetSupplierInList(m_end2Nodes, resourceType, out foundResourceType);
            if (supplier != null) {
                return supplier;
            }

            return null;
        }

        public Tile GetTileAtIndex(int index) {
            return m_tileSegments[index];
        }

        private bool ResourceInList(List<ConnectionNode> nodeList, Resources.Type resourceType, GameObject requester) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                Resources.Type foundResourceType;
                if (storeComponent != null && storeComponent.StorageContains(resourceType, out foundResourceType) && storeComponent.gameObject != requester) {
                    return true;
                }
            }
            return false;
        }

        private StoresProduct GetSupplierInList(List<ConnectionNode> nodeList, Resources.Type resourceType, out Resources.Type foundResourceType) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent != null && storeComponent.StorageContains(resourceType, out foundResourceType)) {
                    return storeComponent;
                }
            }
            foundResourceType = Resources.Type.None;
            return null;
        }

        private bool IsSupplierInList(List<ConnectionNode> nodeList, StoresProduct supplier) {
            for (int i = 0; i < nodeList.Count; i++) {
                StoresProduct storeComponent = nodeList[i].gameObject.GetComponent<StoresProduct>();
                if (storeComponent == supplier) {
                    return true;
                }
            }
            return false;
        }

        private bool IsRecipientInList(List<ConnectionNode> nodeList, Requests recipient) {
            for (int i = 0; i < nodeList.Count; i++) {
                Requests requestComponent = nodeList[i].gameObject.GetComponent<Requests>();
                if (requestComponent == recipient) {
                    return true;
                }
            }
            return false;
        }

        #endregion //  Queries

        #region Triggers 

        public void RemoveConnection(ConnectionNode toRemove) {
            while (m_end1Nodes.Contains(toRemove)) {
                m_end1Nodes.Remove(toRemove);
            }
            while (m_end2Nodes.Contains(toRemove)) {
                m_end2Nodes.Remove(toRemove);
            }
        }

        public void UpdateEconomy() {
            EconomyUpdated?.Invoke(this, EventArgs.Empty);
        }

        

        #endregion // Triggers

        public int GetStartIndex(StoresProduct supplier) {
            if (IsSupplierInList(m_end1Nodes, supplier)) {
                return 0;
            }
            else {
                return m_tileSegments.Count - 1;
            }
        }

        public int GetEndIndex(Requests recipient) {
            if (IsRecipientInList(m_end1Nodes, recipient)) {
                return 0;
            }
            else {
                return m_tileSegments.Count - 1;
            }
        }

        #region All Vars Settings

        public void SetRelevantVars(ref AllVars defaultVars) {
            defaultVars.RoadStartHealth = m_roadStartHealth;
            defaultVars.RoadDisrepairThreshold = m_disrepairThreshold;
        }

        public void HandleAllVarsUpdated(object sender, AllVarsEventArgs args) {
            ProcessUpdatedVars(args.UpdatedVars);
        }

        #endregion // All Vars Settings

        #region AllVars Helpers

        private void ProcessUpdatedVars(AllVars updatedVars) {
            m_roadStartHealth = m_baseHealth = m_currHealth = updatedVars.RoadStartHealth;
            m_disrepairThreshold = updatedVars.RoadDisrepairThreshold;
        }

        #endregion // AllVars Helpers
    }
}
