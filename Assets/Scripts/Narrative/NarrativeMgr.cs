using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.DataDefs;
using Zavala.Events;

namespace Zavala
{
    public class NarrativeMgr : MonoBehaviour
    {
        public static NarrativeMgr Instance;

        [SerializeField] private Canvas m_blurbCanvas;
        [SerializeField] private NarrativeBlurb m_blurb;

        [SerializeField] private BlurbData[] m_blurbsData;
        private Dictionary<string, BlurbData> m_blurbMap;

        public void Init() {
            Instance = this;
            EventMgr.Instance.NarrativeBlurbTriggered += HandleNarrativeBlurbTriggered;

            m_blurbCanvas.gameObject.SetActive(false);
        }

        private void ShowNarrationCanvas() {
            m_blurb.OkayButton.onClick.AddListener(HandleOkayClicked);

            m_blurbCanvas.gameObject.SetActive(true);
        }

        private void HideNarrationCanvas() {
            m_blurb.OkayButton.onClick.RemoveListener(HandleOkayClicked);

            m_blurbCanvas.gameObject.SetActive(false);
        }

        public static BlurbData GetBlurbData(string id) {
            // initialize the map if it does not exist
            if (Instance.m_blurbMap == null) {
                Instance.m_blurbMap = new Dictionary<string, BlurbData>();
                foreach (BlurbData data in Instance.m_blurbsData) {
                    data.Seen = false;
                    Instance.m_blurbMap.Add(data.ID, data);
                }
            }
            if (Instance.m_blurbMap.ContainsKey(id)) {
                return Instance.m_blurbMap[id];
            }
            else {
                throw new KeyNotFoundException(string.Format("No Blurb " +
                    "with id `{0}' is in the database", id
                ));
            }
        }

        #region Handlers

        private void HandleNarrativeBlurbTriggered(object sender, NarrativeBlurbEventArgs args) {
            // initialize the blurb
            m_blurb.Init(args.BlurbText);

            // show the canvas
            ShowNarrationCanvas();
        }

        private void HandleOkayClicked() {
            HideNarrationCanvas();

            EventMgr.Instance.TriggerEvent(ID.NarrativeBlurbClosed, EventArgs.Empty);
        }


        #endregion // Handlers
    }
}