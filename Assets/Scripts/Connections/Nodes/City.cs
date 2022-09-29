﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Functionalities;

namespace Zavala
{
    [RequireComponent(typeof(ConnectionNode))]
    [RequireComponent(typeof(Requests))]
    [RequireComponent(typeof(Produces))]
    [RequireComponent(typeof(StoresProduct))]
    [RequireComponent(typeof(Cycles))]
    public class City : MonoBehaviour
    {
        private ConnectionNode m_connectionNodeComponent;
        private Requests m_requestsComponent;
        private Produces m_producesComponent;
        private StoresProduct m_storesComponent;
        private Cycles m_cyclesComponent;

        private bool m_firstCycle; // whether this is first cycle. Produces product for free after first cycle

        [SerializeField] private int m_population;

        private void Awake() {
            m_connectionNodeComponent = this.GetComponent<ConnectionNode>();
            m_requestsComponent = this.GetComponent<Requests>();
            m_producesComponent = this.GetComponent<Produces>();
            m_storesComponent = this.GetComponent<StoresProduct>();
            m_cyclesComponent = this.GetComponent<Cycles>();

            m_requestsComponent.RequestFulfilled += HandleRequestFulfilled;
            m_requestsComponent.RequestExpired += HandleRequestExpired;
            m_cyclesComponent.CycleCompleted += HandleCycleCompleted;

            m_firstCycle = true;
        }

        private void StraightToStorage() {
            // produce money per population
            for (int p = 0; p < m_population; p++) {
                List<Resources.Type> newProducts = m_producesComponent.Produce();
                if (newProducts == null) {
                    return;
                }

                for (int i = 0; i < newProducts.Count; i++) {
                    if (!m_storesComponent.TryAddToStorage(newProducts[i])) {
                        Debug.Log("[City] Request fulfilled, but storage full!");
                    }
                    else {
                        m_connectionNodeComponent.UpdateNodeEconomy();
                    }
                }
            }
        }


        #region Handlers

        private void HandleCycleCompleted(object sender, EventArgs e) {
            Debug.Log("[City] Cycle completed");

            // Cities request 1 milk / population
            for (int i = 0; i < m_population; i++) {
                m_requestsComponent.QueueRequest(Resources.Type.Milk);
            }

            if (m_firstCycle) {
                StraightToStorage();
                m_firstCycle = false;
            }
        }

        private void HandleRequestFulfilled(object sender, EventArgs e) {
            Debug.Log("[City] Request fulfilled");

            StraightToStorage();
        }

        private void HandleRequestExpired(object sender, EventArgs e) {
            Debug.Log("[City] Request expired");

            // city shrinks by 1
            m_population--;
            if (m_population < 0) {
                Debug.Log("City is empty!");
                m_population = 0;
            }
        }

        #endregion // Handlers

    }
}
