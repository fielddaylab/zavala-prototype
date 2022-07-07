﻿using System.Collections;
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

        void Start() {
            m_submitButton.onClick.AddListener(delegate { EventMgr.SimCanvasSubmitted?.Invoke(); });

            UpdateInteractMode(InteractMode.Farm);
        }

        void OnDestroy() {
            m_submitButton.onClick.RemoveAllListeners();
        }

        // TEMP
        private void UpdateInteractMode(InteractMode newMode) {
            EventMgr.InteractModeUpdated?.Invoke(newMode);
        }
    }
}