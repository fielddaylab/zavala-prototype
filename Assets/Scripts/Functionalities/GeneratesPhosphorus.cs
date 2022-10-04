using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala.Functionalities
{
    public class GeneratesPhosphorus : MonoBehaviour
    {
        [SerializeField] private int m_amt;

        private void Start() {
            if (this.GetComponent<Cycles>() != null) {
                this.GetComponent<Cycles>().CycleCompleted += HandleCycleCompleted;
            }
        }

        private void GeneratePip() {
            PhosphPip newPip = Instantiate(GameDB.Instance.PipPrefab).GetComponent<PhosphPip>();
            newPip.Init(this.GetComponent<Tile>());
        }

        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs args) {
            for (int i = 0; i < m_amt; i++) {
                GeneratePip();
            }
        }

        #endregion // Handlers
    }
}