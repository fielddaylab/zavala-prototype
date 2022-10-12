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
        [SerializeField] private string m_title;
        private string m_additionalText;
        [SerializeField] private bool m_canRemove;

        private UIInspect m_inspectUI;

        public void Init() {
            if (this.gameObject.layer != LayerMask.NameToLayer("Inspect")) {
                Debug.Log("[Inspectable] Inspectable object " + this.gameObject.name + " not set to layer Inspect!");
            }

            Debug.Log("[Instantiate] Instantiating UIInspect prefab");
            m_inspectUI = Instantiate(GameDB.Instance.UIInspectPrefab, this.transform).GetComponent<UIInspect>();
            m_inspectUI.Init();
            m_inspectUI.Removed += HandleRemoval;
        }

        public void Inspect() {
            bool wasOpen = m_inspectUI.gameObject.activeSelf;

            // close all other inspectables
            EventMgr.Instance.TriggerEvent(ID.InspectableOpened, EventArgs.Empty);

            if (!wasOpen) {
                m_inspectUI.Show(m_title, m_additionalText, m_canRemove);
                Debug.Log("[Inspectable] Inspecting!");
            }
        }

        public void SetAdditionalText(string newText) {
            m_additionalText = newText;

            // refresh text
            if (m_inspectUI != null && m_inspectUI.gameObject.activeSelf) {
                m_inspectUI.Show(m_title, m_additionalText, m_canRemove);
            }
        }

        #region Handlers

        private void HandleRemoval(object sender, EventArgs args) {
            AddOn addOnComp = this.GetComponent<AddOn>();
            if (addOnComp != null) {
                // remove from tile list beneath
                Tile tileUnder = GridMgr.TileAtPos(this.transform.position);

                if (tileUnder != null) {
                    tileUnder.TryRemoveAddOn(addOnComp);

                    ConnectionNode nodeUnder = tileUnder.GetComponent<ConnectionNode>();
                    if (nodeUnder != null) {
                        nodeUnder.RemoveFromRoad();
                    }
                }
            }

            Debug.Log("Removal");
            m_inspectUI.Remove();

            // TODO: Sell?
            Destroy(this.gameObject);
        }

        #endregion // Handlers
    }
}