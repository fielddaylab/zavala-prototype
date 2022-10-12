using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala.Settings
{
    public struct AllVars
    {
        // City
        public int CityPopStart;
        public float CityCycleTime;
        public int CityRequestTimeout;
        public int CityProduceMoneyAmt;
        public int CityBloomTolerance;

        // Grain Farm


        // Dairy Farm


        // Truck


        // Road


        // Skimmer


        // Digester


    }

    public class SettingsMgr : MonoBehaviour
    {
        [Header("Timescale")]

        [SerializeField] private Button m_pauseButton;
        [SerializeField] private Sprite[] m_pauseSprites;
        private static int PAUSE_INDEX = 0;
        private static int RESUME_INDEX = 1;
        private bool m_paused;
        private bool m_stashedPaused; // whether the game was paused before narrative blurb was triggered

        [SerializeField] private GameObject m_sliderGroup;
        [SerializeField] private Slider m_timescaleSlider;
        [SerializeField] private float m_slowestSpeed, m_fastestSpeed;
        [SerializeField] private Text m_speedText;
        private float m_speedRange;

        [SerializeField] private Button m_restartButton;

        [Space(5)]

        [Header("Adjustment")]
        [SerializeField] private Button m_adjustActivateButton;
        [SerializeField] private GameObject m_adjustCanvas;
        [SerializeField] private Button m_adjustCancelButton, m_adjustRevertButton, m_adjustApplyButton;

        [SerializeField] private TMP_InputField[] m_cityInputs;

        [Space(5)]

        [Header("Default Vars Source Prefabs")]
        [SerializeField] private GameObject m_cityPrefab;

        private AllVars m_defaultVars; // the default vars
        private AllVars m_modifyingVars; // the vars actively being modified
        private AllVars m_currVars; // the currently saved vars

        public static bool VisibleCycles = true;



        public void Init() {
            m_speedRange = m_fastestSpeed - m_slowestSpeed;

            m_timescaleSlider.onValueChanged.AddListener(HandleTimescaleValueChanged);

            // start at 1
            float startSpeed = 1.0f;
            m_timescaleSlider.value = (startSpeed - m_slowestSpeed) / m_speedRange;

            m_paused = m_stashedPaused = false;
            m_pauseButton.onClick.AddListener(HandlePauseClicked);

            EventMgr.Instance.NarrativeBlurbTriggered += HandleNarrativeBlurbTriggered;
            EventMgr.Instance.NarrativeBlurbClosed += HandleNarrativeBlurbClosed;

            m_adjustActivateButton.onClick.AddListener(HandleAdjustActivateClicked);
            m_restartButton.onClick.AddListener(HandleRestartClicked);

            m_adjustCancelButton.onClick.AddListener(HandleAdjustCancelClicked);
            m_adjustRevertButton.onClick.AddListener(HandleAdjustRevertClicked);
            m_adjustApplyButton.onClick.AddListener(HandleAdjustApplyClicked);

            InitDefaultAllVars();
        }

        private void Awake() {
            // update
            EventMgr.Instance.TriggerEvent(ID.AllVarsUpdated, new AllVarsEventArgs(m_currVars));
        }

        private void ApplySliderToTimescale(float newVal) {
            Time.timeScale = m_slowestSpeed + newVal * m_speedRange;

            m_speedText.text = "Game Speed: " + Math.Round(m_slowestSpeed + newVal * m_speedRange, 2) + "x";
        }

        private void PauseSequence() {
            m_timescaleSlider.enabled = false;

            Time.timeScale = 0;
            m_pauseButton.image.sprite = m_pauseSprites[RESUME_INDEX];

            m_sliderGroup.SetActive(false);
        }

        private void UnpauseSequence() {
            m_timescaleSlider.enabled = true;

            ApplySliderToTimescale(m_timescaleSlider.value);
            m_pauseButton.image.sprite = m_pauseSprites[PAUSE_INDEX];

            m_sliderGroup.SetActive(true);
        }

        #region Handlers 

        private void HandleTimescaleValueChanged(float newVal) {
            ApplySliderToTimescale(newVal);
        }

        private void HandlePauseClicked() {
            m_paused = !m_paused;

            if (m_paused) {
                // game was just paused
                PauseSequence();
            }
            else {
                // game was just unpaused
                UnpauseSequence();
            }
        }

        private void HandleNarrativeBlurbTriggered(object sender, NarrativeBlurbEventArgs args) {
            m_stashedPaused = m_paused;
            if (!m_paused) {
                PauseSequence();
            }
        }

        private void HandleNarrativeBlurbClosed(object sender, EventArgs args) {
            if (!m_stashedPaused) {
                UnpauseSequence();
            }
        }

        private void HandleAdjustActivateClicked() {
            PauseSequence();
            DisplayCurrAllVars();

            m_adjustCanvas.SetActive(true);
        }

        private void HandleRestartClicked() {
            SceneManager.LoadScene("GridScene");
        }

        private void HandleAdjustCancelClicked() {
            // reset to current
            m_modifyingVars = m_currVars;

            // hide adjustment canvas
            m_adjustCanvas.SetActive(false);

            UnpauseSequence();
        }

        private void HandleAdjustRevertClicked() {
            m_currVars = m_defaultVars;

            // update
            EventMgr.Instance.TriggerEvent(ID.AllVarsUpdated, new AllVarsEventArgs(m_currVars));

            // hide adjustment canvas
            m_adjustCanvas.SetActive(false);

            UnpauseSequence();
        }

        private void HandleAdjustApplyClicked() {
            SaveModifiedVars();

            // update
            EventMgr.Instance.TriggerEvent(ID.AllVarsUpdated, new AllVarsEventArgs(m_currVars));

            // hide adjustment canvas
            m_adjustCanvas.SetActive(false);

            UnpauseSequence();
        }

        #endregion // Handlers

        #region Helpers

        private void InitDefaultAllVars() {
            // Save default all vars
            m_cityPrefab.GetComponent<City>().SetRelevantVars(ref m_defaultVars);

            // set current to defaults
            m_currVars = m_defaultVars;
        }

        private void SaveModifiedVars() {
            m_modifyingVars = new AllVars();

            // City
            m_modifyingVars.CityPopStart = int.Parse(m_cityInputs[0].text);
            m_modifyingVars.CityCycleTime = float.Parse(m_cityInputs[1].text);
            m_modifyingVars.CityRequestTimeout = int.Parse(m_cityInputs[2].text);
            m_modifyingVars.CityProduceMoneyAmt = int.Parse(m_cityInputs[3].text);
            m_modifyingVars.CityBloomTolerance = int.Parse(m_cityInputs[4].text);

            // TODO: all below
            // Grain Farm


            // Dairy Farm


            // Truck


            // Road


            // Skimmer


            // Digester


            // Save final result
            m_currVars = m_modifyingVars;
        }

        private void DisplayCurrAllVars() {
            // Set all values to default values
            m_cityInputs[0].text = "" + m_currVars.CityPopStart;
            m_cityInputs[1].text = "" + m_currVars.CityCycleTime;
            m_cityInputs[2].text = "" + m_currVars.CityRequestTimeout;
            m_cityInputs[3].text = "" + m_currVars.CityProduceMoneyAmt;
            m_cityInputs[4].text = "" + m_currVars.CityBloomTolerance;
        }

        #endregion // Helpers
    }
}
