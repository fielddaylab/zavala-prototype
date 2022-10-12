using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Tiles;

namespace Zavala
{
    public class Inspectable : MonoBehaviour
    {
        [SerializeField] private bool m_canRemove;

        private UIInspect m_inspectUI;

        private void Start() {
            if (this.gameObject.layer != LayerMask.NameToLayer("Inspect")) {
                Debug.Log("[Inspectable] Inspectable object " + this.gameObject.name + " not set to layer Inspect!");
            }

            m_inspectUI = Instantiate(GameDB.Instance.UIInspectPrefab, this.transform).GetComponent<UIInspect>();
            m_inspectUI.Init();
            m_inspectUI.Removed += HandleRemoval;
        }

        public void Inspect() {
            bool wasOpen = m_inspectUI.gameObject.activeSelf;

            // close all other inspectables
            EventMgr.Instance.TriggerEvent(ID.InspectableOpened, EventArgs.Empty);

            if (!wasOpen) {
                m_inspectUI.Show(m_canRemove);
                Debug.Log("[Inspectable] Inspecting!");
            }
        }

        #region Handlers

        private void HandleRemoval(object sender, EventArgs args) {
            AddOn addOnComp = this.GetComponent<AddOn>();
            if (addOnComp != null) {
                // remove from tile list beneath
                Tile tileUnder = GridMgr.OverTile(this.transform.position);

                if (tileUnder != null) {
                    tileUnder.TryRemoveAddOn(addOnComp);
                }
            }

            m_inspectUI.Remove();

            // TODO: Sell?
            Destroy(this.gameObject);
        }

        #endregion // Handlers
    }
}