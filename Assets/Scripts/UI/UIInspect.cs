using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Tiles;

namespace Zavala
{
    public class UIInspect : MonoBehaviour
    {
        [SerializeField] private Button m_removeButton;

        public event EventHandler Removed;

        public void Init() {
            EventMgr.Instance.InspectableOpened += HandleInspectableOpened;
            this.gameObject.SetActive(false);
        }

        public void Show(bool canRemove) {
            if (canRemove) {
                m_removeButton.onClick.AddListener(HandleRemoveClicked);
            }
            m_removeButton.gameObject.SetActive(canRemove);

            this.gameObject.SetActive(true);
        }

        public void Hide() {
            if (m_removeButton.gameObject.activeSelf) {
                m_removeButton.onClick.RemoveListener(HandleRemoveClicked);
                m_removeButton.gameObject.SetActive(false);
            }

            this.gameObject.SetActive(false);
        }

        public void Remove() {
            Hide();
            EventMgr.Instance.InspectableOpened -= HandleInspectableOpened;
        }

        #region Handlers

        private void HandleRemoveClicked() {
            // remove object
            Removed?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInspectableOpened(object sender, EventArgs args) {
            if (this.gameObject.activeSelf) {
                Hide();
            }
        }

        #endregion // Handlers
    }
}