using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;
using Zavala.Interact;

namespace Zavala.Tiles
{
    public enum Type
    {
        None,
        Land,
        GrainFarm,
        DairyFarm,
        City,
        Lake
    }

    public class Tile : MonoBehaviour {

        #region Member Vars

        // Components

        private Mesh m_originalMesh;
        private Mesh m_originalMeshUnder;
        private MeshFilter m_meshFilterComponent;
        private MeshFilter m_meshFilterComponentUnder;

        private BlocksBuild m_blocksBuildComponent;

        private List<AddOn> m_addOns;

        // Phosph Pips

        private List<PhosphPip> m_pips;
        private List<PhosphPip> m_stagedToAdd, m_stagedToRemove;


        // Relative Elevation

        private List<Tile> m_neighborTiles;

        private float m_elevation;

        #endregion // Member Vars

        #region Callbacks

        private void Awake() {
            m_meshFilterComponent = this.GetComponent<MeshFilter>();
            m_meshFilterComponentUnder = this.transform.GetChild(0).GetComponent<MeshFilter>();
            m_originalMesh = m_meshFilterComponent.mesh;
            m_originalMeshUnder = m_meshFilterComponentUnder.mesh;

            m_blocksBuildComponent = this.GetComponent<BlocksBuild>(); // <- may well be null

            m_addOns = new List<AddOn>();

            m_pips = new List<PhosphPip>();
            m_stagedToAdd = new List<PhosphPip>();
            m_stagedToRemove = new List<PhosphPip>();

            m_elevation = this.transform.localPosition.y;

            GridMgr.TrackTile(this);
        }

        private void Start() {
            RecalculateNeighbors();
        }

        private void OnMouseEnter() {
            switch(InteractMgr.Instance.GetCurrMode()) {
                case Interact.Mode.PlaceItem:
                    if (!AnyBlockers()) {
                        HoverPlaceFilter();
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnMouseExit() {
            m_meshFilterComponent.mesh = m_originalMesh;
            m_meshFilterComponentUnder.mesh = m_originalMeshUnder;
        }

        private void OnDestroy() {
            GridMgr.UntrackTile(this);
        }

        #endregion // Callbacks

        #region Helpers

        private void RecalculateNeighbors() {
            // Generate a list of neighbor tiles
            m_neighborTiles = GridMgr.GetAdjTiles(this);
        }

        private void HoverPlaceFilter() {
            if (m_blocksBuildComponent == null) {
                m_meshFilterComponent.mesh = GameDB.Instance.HoverMesh;
                m_meshFilterComponentUnder.mesh = GameDB.Instance.HoverMesh;
            }
            else {
                // do nothing, or display a red filter
            }
        }

        private bool AnyBlockers() {
            if (m_blocksBuildComponent != null) {
                return true;
            }

            for (int i = 0; i < m_addOns.Count; i++) {
                if (m_addOns[i].gameObject.GetComponent<BlocksBuild>() != null) {
                    return true;
                }
            }

            return false;
        }

        #endregion // Helpers

        #region Debug Highlight

        public void DebugHighlight() {
            m_meshFilterComponent.mesh = GameDB.Instance.HoverMesh;
            m_meshFilterComponentUnder.mesh = GameDB.Instance.HoverMesh;
        }
        public void UndoDebugHighlight() {
            m_meshFilterComponent.mesh = m_originalMesh;
            m_meshFilterComponentUnder.mesh = m_originalMeshUnder;
        }

        #endregion // Debug Highlight

        #region Actions

        public void ClickTile() {
            switch (InteractMgr.Instance.GetCurrMode()) {
                case Interact.Mode.PlaceItem:
                    if (!AnyBlockers()) {
                        // attempt purchase
                        if (ShopMgr.Instance.TryPurchaseSelection()) {
                            GameObject itemInstance = Instantiate(ShopMgr.Instance.GetPurchasePrefab(), this.transform);
                            m_addOns.Add(itemInstance.GetComponent<AddOn>());
                            Debug.Log("purchased the item!");
                        }
                        else {
                            Debug.Log("failed to purchase the item: shop rejection");
                        }
                    }
                    else {
                        Debug.Log("failed to purchase the item: blockers");
                    }

                    break;
                default:
                    break;
            }
        }

        public void ConstructRoad(GameObject roadPrefab) {
            GameObject itemInstance = Instantiate(roadPrefab, this.transform);
            m_addOns.Add(itemInstance.GetComponent<AddOn>());
        }

        #endregion // Actions

        #region Phosphorus Pips

        public void AddPipDirect(PhosphPip pip) {
            m_pips.Add(pip);
        }

        public void RemovePipDirect(PhosphPip pip) {
            m_pips.Remove(pip);
        }

        public int GetPipCount() {
            return m_pips.Count;
        }

        public void StageAddPip(PhosphPip pip) {
            m_stagedToAdd.Add(pip);
        }

        public PhosphPip StageRemovePip(int pipIndex) {
            PhosphPip pipToRemove = m_pips[pipIndex];
            m_stagedToRemove.Add(pipToRemove);
            return pipToRemove;
        }

        public void ApplyStagedTransfer() {
            for (int i = 0; i < m_stagedToRemove.Count; i++) {
                m_pips.Remove(m_stagedToRemove[i]);
            }
            for (int i = 0; i < m_stagedToAdd.Count; i++) {
                // TODO: gradual pip movement
                m_pips.Add(m_stagedToAdd[i]);
                m_stagedToAdd[i].TransferToTile(this);
            }

            // clean up
            m_stagedToAdd.Clear();
            m_stagedToRemove.Clear();
        }

        #endregion // Phosphorus Pips

        #region Getters and Setters

        public List<Tile> GetNeighbors() {
            return m_neighborTiles;
        }

        public float GetElevation() {
            return m_elevation;
        }

        #endregion // Getters and Setters
    }
}
