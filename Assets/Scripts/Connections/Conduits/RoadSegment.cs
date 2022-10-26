using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Roads;
using Zavala.Functionalities;
using Zavala.Settings;
using Zavala.Events;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(Inspectable))]
    public class RoadSegment : MonoBehaviour, IAllVars
    {
        private Inspectable m_inspectComponent;

        [SerializeField] private GameObject m_rampPrefab, m_flatPrefab;

        [SerializeField] private float m_roadStartHealth;
        private float m_baseHealth; // road health
        private float m_currHealth; // road health
        private bool m_isUsable;
        private bool m_inDisrepair;

        [SerializeField] private float m_disrepairThreshold;
        [SerializeField] private MeshRenderer m_meshRenderer;

        private EdgeSegment[] m_edges;

        private static int NUM_HEX_EDGES = 6;

        private void Awake() {
            m_inspectComponent = this.GetComponent<Inspectable>();

            m_edges = new EdgeSegment[NUM_HEX_EDGES];

            m_isUsable = true;

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;

            ProcessUpdatedVars(SettingsMgr.Instance.GetCurrAllVars());
        }

        private void OnDisable() {
            EventMgr.Instance.AllVarsUpdated -= HandleAllVarsUpdated;
        }

        private void Start() {
            m_inspectComponent.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="elevationDelta">how much higher the connected tile is than this center segments</param>
        public void ActivateEdge(RoadBuildDir dir, float elevationDelta, GameObject connectsTo) {
            int sideIndex = 0;
            switch (dir) {
                case RoadBuildDir.Up:
                    break;
                case RoadBuildDir.Up_Right:
                    sideIndex = 1;
                    break;
                case RoadBuildDir.Down_Right:
                    sideIndex = 2;
                    break;
                case RoadBuildDir.Down:
                    sideIndex = 3;
                    break;
                case RoadBuildDir.Down_Left:
                    sideIndex = 4;
                    break;
                case RoadBuildDir.Up_Left:
                    sideIndex = 5;
                    break;
                default:
                    break;
            }

            GameObject newEdgeObj = elevationDelta > 0 ? m_rampPrefab : m_flatPrefab;
            if (m_edges[sideIndex] != null) {
                // remove old prefab
                Destroy(m_edges[sideIndex].gameObject);
            }
            // Instantiate new prefab
            EdgeSegment newEdge = Instantiate(newEdgeObj, this.transform).GetComponent<EdgeSegment>();
            newEdge.Connection = connectsTo;
            m_edges[sideIndex] = newEdge;
            newEdge.RotateEdge(dir);
            if (elevationDelta > 0) {
                newEdge.ScaleEdge(elevationDelta);
            }
        }

        public void DeactivateEdge(RoadBuildDir dir) {
            int sideIndex = 0;
            switch (dir) {
                case RoadBuildDir.Up:
                    break;
                case RoadBuildDir.Up_Right:
                    sideIndex = 1;
                    break;
                case RoadBuildDir.Down_Right:
                    sideIndex = 2;
                    break;
                case RoadBuildDir.Down:
                    sideIndex = 3;
                    break;
                case RoadBuildDir.Down_Left:
                    sideIndex = 4;
                    break;
                case RoadBuildDir.Up_Left:
                    sideIndex = 5;
                    break;
                default:
                    break;
            }

            if (m_edges[sideIndex] != null) {
                // remove old prefab
                Destroy(m_edges[sideIndex].gameObject);
            }
        }

        #region External

        public bool ResourceInEdges(Resources.Type resourceType, GameObject requester, out StoresProduct supplier, out Resources.Type foundResourceType) {
            for (int e = 0; e < m_edges.Length; e++) {
                if (!m_isUsable) { continue; }
                if (m_edges[e] == null) { continue; }

                StoresProduct storeComponent = m_edges[e].Connection.GetComponent<StoresProduct>();
                supplier = storeComponent;
                if (storeComponent != null && storeComponent.StorageContains(resourceType, out foundResourceType) && storeComponent.gameObject != requester) {
                    Debug.Log("[RoadSegment] Resource (" + resourceType + ") in edges in the form of (" + foundResourceType + "). Requester: " + requester + " || Supplier: " + storeComponent.gameObject);
                    return true;
                }

                // check add-ons
                if (m_edges[e].Connection.GetComponent<Tile>() != null) {
                    List<AddOn> addOns = m_edges[e].Connection.GetComponent<Tile>().GetAddOns();
                    for (int a = 0; a < addOns.Count; a++) {
                        storeComponent = addOns[a].GetComponent<StoresProduct>();
                        
                        if (storeComponent != null && storeComponent.StorageContains(resourceType, out foundResourceType) && storeComponent.gameObject != requester) {
                            Debug.Log("[RoadSegment] Resource (" + resourceType + ") in edges in the form of (" + foundResourceType + "). Requester: " + requester + " || Supplier: " + storeComponent.gameObject);
                            supplier = storeComponent;
                            return true;
                        }
                    }
                }
            }

            supplier = null;
            foundResourceType = Resources.Type.None;
            return false;
        }

        public List<RoadSegment> GetConnectedRoads() {
            List<RoadSegment> connectedRoads = new List<RoadSegment>();

            for (int e = 0; e < m_edges.Length; e++) {
                if (m_edges[e] == null) { continue; }

                RoadSegment roadComponent = m_edges[e].Connection.GetComponent<RoadSegment>();
                if (roadComponent != null) {
                    connectedRoads.Add(roadComponent);
                }
            }

            return connectedRoads;
        }

        public void ApplyDamage(float dmg) {
            m_currHealth -= dmg;

            Debug.Log("[Road] Curr health: " + m_currHealth);
            if (!m_inDisrepair && (m_currHealth <= m_disrepairThreshold * m_baseHealth)) {
                Disrepair();
            }
            if (m_currHealth <= 0) {
                m_currHealth = 0;

                // Destroy(this.gameObject);
                this.gameObject.SetActive(false);

                m_isUsable = false;
            }
        }

        public void Disrepair() {
            Debug.Log("[RoadSegment] road entering disrepair!");

            // visually indicate disrepair
            m_meshRenderer.material = GameDB.Instance.RoadDisrepairMaterial;
            for (int e = 0; e < m_edges.Length; e++) {
                if (m_edges[e] == null) { continue; }
                m_edges[e].SetMaterial(GameDB.Instance.RoadDisrepairMaterial);
            }


            m_inDisrepair = true;

            // TODO: trigger repair needed
        }

        public bool IsUsable() {
            return m_isUsable;
        }

        #endregion // External

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