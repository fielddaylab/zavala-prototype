﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Settings;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    [RequireComponent(typeof(DamagesRoad))]
    public class Truck : MonoBehaviour, IAllVars
    {
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private float m_speed;
        [SerializeField] private float m_leakRate;

        private Resources.Type m_resourceType;
        private Requests m_recipient;
        private Road m_roadToFollow;

        // Road Traversal
        private int m_startRoadSegmentIndex;
        private int m_destRoadSegmentIndex;
        private int m_currRoadSegmentIndex;
        private Tile m_immediateNextDest;

        private float m_yBuffer;

        private bool m_delivered;

        private GeneratesPhosphorus m_generatesComponent;
        private DamagesRoad m_damagesComponent;

        // Audio
        private AudioSource m_audioSource;

        [SerializeField] private AudioClip m_engineStartClip;
        [SerializeField] private AudioClip m_engineContinueClip;
        [SerializeField] private AudioClip m_engineEndClip;

        private enum EngineState
        {
            Start,
            Continue,
            End
        }

        private EngineState m_engineState;

        public void Init(Resources.Type resourceType, StoresProduct supplier, Requests recipient, Road road) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_yBuffer = this.gameObject.transform.localPosition.y;

            m_delivered = false;

            m_resourceType = resourceType;
            m_recipient = recipient;
            m_roadToFollow = road;
            m_startRoadSegmentIndex = m_currRoadSegmentIndex = m_roadToFollow.GetStartIndex(supplier);
            m_destRoadSegmentIndex = m_roadToFollow.GetEndIndex(recipient);
            m_immediateNextDest = m_roadToFollow.GetTileAtIndex(m_startRoadSegmentIndex);
            this.transform.position = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);

            m_audioSource = this.GetComponent<AudioSource>();
            m_engineState = EngineState.Start;
            m_audioSource.clip = m_engineStartClip;
            m_audioSource.Play();

            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();
            m_damagesComponent = this.GetComponent<DamagesRoad>();

            EventMgr.Instance.AllVarsUpdated += HandleAllVarsUpdated;
            ProcessUpdatedVars(SettingsMgr.Instance.GetCurrAllVars());
        }

        private void OnDisable() {
            EventMgr.Instance.AllVarsUpdated -= HandleAllVarsUpdated;
        }

        private void Update() {
            if (Time.timeScale == 0) { return; }
            TraverseRoad();
            UpdateAudio();
        }

        private void TraverseRoad() {
            Vector3 nextDestPos = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);
            if (Vector3.Distance(this.transform.position, nextDestPos) > .1f) {
                // move to immediate next dest
                Debug.Log("[Truck] Moving towards next destination");
                this.transform.Translate((nextDestPos - this.transform.position).normalized * m_speed * Time.deltaTime);
            }
            else {
                // update immediate next dest
                if (m_currRoadSegmentIndex == m_destRoadSegmentIndex) {
                    if (!m_delivered) {
                        Deliver();
                        m_damagesComponent.DamageRoad(m_roadToFollow, m_resourceType);
                        Debug.Log("[Truck] Delivered to final destination");
                    }
                }
                else {
                    if (m_currRoadSegmentIndex < m_destRoadSegmentIndex) {
                        m_currRoadSegmentIndex++;
                    }
                    else {
                        m_currRoadSegmentIndex--;
                    }
                    m_immediateNextDest = m_roadToFollow.GetTileAtIndex(m_currRoadSegmentIndex);

                    // leak phosphorus (TODO: leak according to time, or per tile?)
                    // if per time, reference cycles component
                    // if per tile, 25% chance per tile
                    if (Random.Range(0.0f, 1.0f) <= m_leakRate) {
                        m_generatesComponent.GeneratePipBatch(m_immediateNextDest, m_resourceType);
                    }

                    Debug.Log("[Truck] Updated immediate next destination");
                }
            }
        }

        private void UpdateAudio() {
            switch (m_engineState) {
                case EngineState.Start:
                    if (!m_audioSource.isPlaying) {
                        m_engineState = EngineState.Continue;
                        m_audioSource.clip = m_engineContinueClip;
                        m_audioSource.loop = true;
                        m_audioSource.Play();
                    }
                    break;
                case EngineState.Continue:

                    break;
                case EngineState.End:
                    if (!m_audioSource.isPlaying) {
                        if (m_audioSource.loop) {
                            m_audioSource.clip = m_engineEndClip;
                            m_audioSource.loop = false;
                            m_audioSource.Play();
                        }
                        else {
                            // remove truck 
                            Destroy(this.gameObject);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Deliver() {
            m_recipient.ReceiveRequestedProduct(m_resourceType);
            m_delivered = true;

            m_engineState = EngineState.End;
            m_audioSource.Stop();

            // hide truck
            m_canvas.gameObject.SetActive(false);
        }

        #region All Vars Settings

        public void SetRelevantVars(ref AllVars defaultVars) {
            defaultVars.TruckSpeed = m_speed;

            defaultVars.TruckLeakRate = m_leakRate;
            defaultVars.TruckLeakAmtManure = this.GetComponent<GeneratesPhosphorus>().GetAmtForResource(Resources.Type.Manure);
            if (defaultVars.TruckLeakAmtManure == -1) {
                Debug.Log("[Truck] Truck does not leak when transporting manure.");
            }
            defaultVars.TruckLeakAmtFertilizer = this.GetComponent<GeneratesPhosphorus>().GetAmtForResource(Resources.Type.Fertilizer);
            if (defaultVars.TruckLeakAmtFertilizer == -1) {
                Debug.Log("[Truck] Truck does not leak when transporting fertilizer.");
            }
            defaultVars.TruckRoadDmgManure = this.GetComponent<DamagesRoad>().GetAmtForResource(Resources.Type.Manure);
            if (defaultVars.TruckRoadDmgManure == -1) {
                Debug.Log("[Truck] Truck does not damage road for manure.");
            }
            defaultVars.TruckRoadDmgFertilizer = this.GetComponent<DamagesRoad>().GetAmtForResource(Resources.Type.Fertilizer);
            if (defaultVars.TruckRoadDmgFertilizer == -1) {
                Debug.Log("[Truck] Truck does not damage road for fertilizer.");
            }
        }

        public void HandleAllVarsUpdated(object sender, AllVarsEventArgs args) {
            ProcessUpdatedVars(args.UpdatedVars);
        }

        #endregion // All Vars Settings

        #region AllVars Helpers

        private void ProcessUpdatedVars(AllVars updatedVars) {
            m_speed = updatedVars.TruckSpeed;

            m_leakRate = updatedVars.TruckLeakRate;
            this.GetComponent<GeneratesPhosphorus>().SetAmtForResource(Resources.Type.Manure, updatedVars.TruckLeakAmtManure);
            this.GetComponent<GeneratesPhosphorus>().SetAmtForResource(Resources.Type.Fertilizer, updatedVars.TruckLeakAmtFertilizer);
            this.GetComponent<DamagesRoad>().SetAmtForResource(Resources.Type.Manure, updatedVars.TruckRoadDmgManure);
            this.GetComponent<DamagesRoad>().SetAmtForResource(Resources.Type.Fertilizer, updatedVars.TruckRoadDmgFertilizer);
        }

        #endregion // AllVars Helpers
    }
}
