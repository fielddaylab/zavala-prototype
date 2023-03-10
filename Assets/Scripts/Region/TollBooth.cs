using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala;
using Zavala.Roads;

namespace Zavala
{
    [RequireComponent(typeof(Inspectable))]
    public class TollBooth : MonoBehaviour
    {
        private Inspectable m_inspectComponent;

        private void OnMouseEnter() {
            Debug.Log("[Toll] mouse enter");
            RoadMgr.Instance.SetLastKnownToll(this);
        }

        private void OnMouseExit() {
            Debug.Log("[Toll] mouse exit");

            RoadMgr.Instance.RemoveLastKnownToll(this);
        }

        private void Awake() {
            m_inspectComponent = this.GetComponent<Inspectable>();
        }

        private void Start() {
            m_inspectComponent.Init();
        }
    }
}
