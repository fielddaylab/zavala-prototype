using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        private void Awake() {
            m_meshFilterComponent = this.GetComponent<MeshFilter>();
            m_meshFilterComponentUnder = this.transform.GetChild(0).GetComponent<MeshFilter>();
            m_originalMesh = m_meshFilterComponent.mesh;
            m_originalMeshUnder = m_meshFilterComponentUnder.mesh;
        }

        private void OnMouseEnter() {
            switch(InteractMgr.Instance.GetCurrMode()) {
                case Interact.Mode.PlaceRoad:
                    HoverPlaceFilter();
                    break;
                case Interact.Mode.PlaceDigester:
                    HoverPlaceFilter();
                    break;
                case Interact.Mode.PlaceSkimmer:
                    HoverPlaceFilter();
                    break;
                case Interact.Mode.PlaceStorage:
                    HoverPlaceFilter();
                    break;
                default:
                    break;
            }
        }

        private void OnMouseExit() {
            Debug.Log("Mouse is exiting!");

            m_meshFilterComponent.mesh = m_originalMesh;
            m_meshFilterComponentUnder.mesh = m_originalMeshUnder;
        }

        private void HoverPlaceFilter() {
            m_meshFilterComponent.mesh = GameDB.Instance.HoverMesh;
            m_meshFilterComponentUnder.mesh = GameDB.Instance.HoverMesh;
        }
    }
}
