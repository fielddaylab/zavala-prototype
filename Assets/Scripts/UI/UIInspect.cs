using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Tiles;

namespace Zavala
{
    public class UIInspect : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_titleText;
        [SerializeField] private TMP_Text m_additionalText;
        [SerializeField] private Button m_removeButton;

        public event EventHandler Removed;

        public void Init() {
            this.gameObject.SetActive(false);
        }

        private void OnEnable() {
            EventMgr.Instance.InspectableOpened += HandleInspectableOpened;
        }

        private void OnDisable() {
            EventMgr.Instance.InspectableOpened -= HandleInspectableOpened;
        }

        public void Show(string title, string additionalText, bool canRemove) {
            //title
            m_titleText.text = title;

            // additional text
            m_additionalText.text = additionalText;

            // remove button
            m_removeButton.onClick.RemoveAllListeners();
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
        }

        #region Handlers

        private void HandleRemoveClicked() {
            Debug.Log("Remove clicked");
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