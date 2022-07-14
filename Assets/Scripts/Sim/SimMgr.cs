using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Interact;

namespace Zavala.Sim
{
    public enum SimAction
    {
        Null,
        // used for factions
        MoveFarm,
        LowerPhosphorousOutput,
        RaisePhosphorousOutput,
        IncreaseUptake,
        BuildStorage,

        BuildRail,
        BuildHighway,
        BuildRoad,
        BuildBridge,

        IncreaseGovernmentPolicy,
        IncreasePrivatePolicy,
        DecreaseGovernmentPolicy,
        DecreasePrivatePolicy,

        BuildBasicExchange,
        BuildDigesterExchange,

        DistrictStop,
        DistrictAd
    }

    public class SimMgr : MonoBehaviour
    {
        [SerializeField] private Button m_submitButton;
        [SerializeField] private SimModeUI[] m_simModeGroups;

        private SimModeData m_currMode;

        void Start() {
            m_submitButton.onClick.AddListener(delegate { EventMgr.SimCanvasSubmitted?.Invoke(); });

            UpdateInteractMode(InteractMode.Default);

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

            SimModeUI toOpen = null;

            foreach(SimModeUI simGroup in m_simModeGroups) {
                if (simGroup.ID == data.ID) {
                    toOpen = simGroup;
                }
                else {
                    simGroup.gameObject.SetActive(false);
                    simGroup.Close();
                }
            }

            if (toOpen != null) {
                toOpen.gameObject.SetActive(true);
                toOpen.Open();
            }

            m_currMode = data;

            ResultsMgr.Instance.LoadSimData(m_currMode);
        }
    }
}
