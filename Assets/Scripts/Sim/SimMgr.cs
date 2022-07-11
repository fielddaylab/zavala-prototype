using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Interact;

namespace Zavala.Sim
{
    public enum SimAction
    {
        MoveFarm,
        BuildRoad
        // etc.
    }

    public class SimMgr : MonoBehaviour
    {
        [SerializeField] private Button m_submitButton;
        [SerializeField] private SimModeUI[] m_simModeGroups;

        void Start() {
            m_submitButton.onClick.AddListener(delegate { EventMgr.SimCanvasSubmitted?.Invoke(); });

            UpdateInteractMode(InteractMode.Farm);

            EventMgr.SetNewMode?.AddListener(OnNewModeSet);
        }

        void OnDestroy() {
            m_submitButton.onClick.RemoveAllListeners();
        }

        // TEMP
        private void UpdateInteractMode(InteractMode newMode) {
            EventMgr.InteractModeUpdated?.Invoke(newMode);
        }

        private void OnNewModeSet(SimModeData data) {
            EventMgr.SetNewIndicators?.Invoke(data.IndicatorData);
            foreach(SimModeUI simGroup in m_simModeGroups) {
                if (simGroup.ID == data.ID) {
                    simGroup.gameObject.SetActive(true);
                }
                else {
                    simGroup.gameObject.SetActive(false);
                }
            }

            // TODO: activate/de-activate selectively?
        }
    }
}
