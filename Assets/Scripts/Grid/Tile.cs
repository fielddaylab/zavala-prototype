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

        private Mesh m_originalMesh;
        private Mesh m_originalMeshUnder;
        private MeshFilter m_meshFilterComponent;
        private MeshFilter m_meshFilterComponentUnder;

        private BlocksBuild m_blocksBuildComponent;

        private List<AddOn> m_addOns;

        private void Awake() {
            m_meshFilterComponent = this.GetComponent<MeshFilter>();
            m_meshFilterComponentUnder = this.transform.GetChild(0).GetComponent<MeshFilter>();
            m_originalMesh = m_meshFilterComponent.mesh;
            m_originalMeshUnder = m_meshFilterComponentUnder.mesh;

            m_blocksBuildComponent = this.GetComponent<BlocksBuild>(); // <- may well be null

            m_addOns = new List<AddOn>();
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

        private void HoverPlaceFilter() {
            if (m_blocksBuildComponent == null) {
                m_meshFilterComponent.mesh = GameDB.Instance.HoverMesh;
                m_meshFilterComponentUnder.mesh = GameDB.Instance.HoverMesh;
            }
            else {
                // do nothing, or display a red filter
            }
        }

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
    }
}
