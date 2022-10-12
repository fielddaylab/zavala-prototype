using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.DataDefs;
using Zavala.Events;

namespace Zavala
{
    public class UIBlurbIcon : MonoBehaviour
    {
        [SerializeField] private Button m_button;
        private string m_text;

        public void Init(BlurbData data) {
            m_button.onClick.AddListener(HandleBlurbIconClicked);
            m_text = data.Text;
        }

        #region Handlers 

        private void HandleBlurbIconClicked() {
            EventMgr.Instance.TriggerEvent(Events.ID.NarrativeBlurbTriggered, new NarrativeBlurbEventArgs(m_text));
            Destroy(this.gameObject);
        }

        #endregion // Handlers
    }
}