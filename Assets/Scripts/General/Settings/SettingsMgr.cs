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
        public float GrainCycleTime;
        public int GrainRequestTimeout;
        public int GrainImportCost;
        public int GrainPhosphPerManure;
        public int GrainPhosphPerFertilizer;

        // Truck
        public float TruckSpeed;
        public float TruckLeakRate;
        public int TruckLeakAmtManure;
        public int TruckLeakAmtFertilizer;
        public float TruckRoadDmgManure;
        public float TruckRoadDmgFertilizer;

        // Skimmer
        public int SkimmerSkimAmt;
        public float SkimmerCycleTime;
        public int SkimmerExpiredRunoffAmt;

        // Road
        public float RoadStartHealth;
        public float RoadDisrepairThreshold;

        // PhosphMgr


        // Dairy Farm


        // Digester


        // TileGenerator
        public float TileGeneratorCycleTime;
        public TileGenerator.GenerateMode TileGeneratorMode;
    }

    public class SettingsMgr : MonoBehaviour
    {
        public static SettingsMgr Instance;

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
        [SerializeField] private TMP_InputField[] m_grainInputs;
        [SerializeField] private TMP_InputField[] m_truckInputs;
        [SerializeField] private TMP_InputField[] m_skimmerInputs;
        [SerializeField] private TMP_InputField[] m_roadInputs;
        [SerializeField] private TMP_InputField[] m_tileGeneratorInputs;
        [SerializeField] private TMP_Dropdown[] m_tileGeneratorDropdowns;

        [Space(5)]

        [Header("Default Vars Source Prefabs")]
        [SerializeField] private GameObject m_cityPrefab;
        [SerializeField] private GameObject m_grainPrefab;
        [SerializeField] private GameObject m_truckPrefab;
        [SerializeField] private GameObject m_skimmerPrefab;
        [SerializeField] private GameObject m_roadPrefab;
        [SerializeField] private GameObject m_tileGeneratorObj;

        private AllVars m_defaultVars; // the default vars
        private AllVars m_modifyingVars; // the vars actively being modified
        private AllVars m_currVars; // the currently saved vars

        public static bool VisibleCycles = false;


        public void Init() {
            Instance = this;

            m_speedRange = m_fastestSpeed - m_slowestSpeed;

            m_timescaleSlider.onValueChanged.AddListener(HandleTimescaleValueChanged);

            // start at 1
            float startSpeed = 1.0f;
            m_timescaleSlider.value = (startSpeed - m_slowestSpeed) / m_speedRange;

            m_paused = m_stashedPaused = false;
            m_pauseButton.onClick.AddListener(HandlePauseClicked);

            EventMgr.Instance.NarrativeBlurbTriggered += HandleNarrativeBlurbTriggered;
            EventMgr.Instance.NarrativeBlurbClosed += HandleNarrativeBlurbClosed;

            // Note: cannot pause game here with current implementation. Must distinguish between UI movement timescale and simulation timescale.
            // EventMgr.Instance.AdvisorBlurb += HandleAdvisorBlurb;
            // EventMgr.Instance.ChoiceUnlock += HandleChoiceUnlock;


            m_adjustActivateButton.onClick.AddListener(HandleAdjustActivateClicked);
            m_restartButton.onClick.AddListener(HandleRestartClicked);

            m_adjustCancelButton.onClick.AddListener(HandleAdjustCancelClicked);
            m_adjustRevertButton.onClick.AddListener(HandleAdjustRevertClicked);
            m_adjustApplyButton.onClick.AddListener(HandleAdjustApplyClicked);

            InitDefaultAllVars();

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

        private void HandleAdvisorBlurb(object sender, AdvisorBlurbEventArgs args) {
            m_stashedPaused = m_paused;
            if (!m_paused) {
                PauseSequence();
            }
        }

        private void HandleChoiceUnlock(object sender, ChoiceUnlockEventArgs args) {
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
            EventMgr.Instance.TriggerEvent(ID.LevelRestarted, EventArgs.Empty);
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
            // City
            m_cityPrefab.GetComponent<City>().SetRelevantVars(ref m_defaultVars);

            // Grain Farm
            m_grainPrefab.GetComponent<GrainFarm>().SetRelevantVars(ref m_defaultVars);

            // Truck
            m_truckPrefab.GetComponent<Truck>().SetRelevantVars(ref m_defaultVars);

            // Skimmer
            m_skimmerPrefab.GetComponent<Skimmer>().SetRelevantVars(ref m_defaultVars);

            // Road
            m_roadPrefab.GetComponent<RoadSegment>().SetRelevantVars(ref m_defaultVars);

            // TileGenerator
            m_tileGeneratorObj.GetComponent<TileGenerator>().SetRelevantVars(ref m_defaultVars);

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

            // Grain Farm
            m_modifyingVars.GrainCycleTime = float.Parse(m_grainInputs[0].text);
            m_modifyingVars.GrainRequestTimeout = int.Parse(m_grainInputs[1].text);
            m_modifyingVars.GrainImportCost = int.Parse(m_grainInputs[2].text);
            m_modifyingVars.GrainPhosphPerManure = int.Parse(m_grainInputs[3].text);
            m_modifyingVars.GrainPhosphPerFertilizer = int.Parse(m_grainInputs[4].text);

            // Truck
            m_modifyingVars.TruckSpeed = float.Parse(m_truckInputs[0].text);
            m_modifyingVars.TruckLeakRate = float.Parse(m_truckInputs[1].text);
            m_modifyingVars.TruckLeakAmtManure = int.Parse(m_truckInputs[2].text);
            m_modifyingVars.TruckLeakAmtFertilizer = int.Parse(m_truckInputs[3].text);
            m_modifyingVars.TruckRoadDmgManure = float.Parse(m_truckInputs[4].text);
            m_modifyingVars.TruckRoadDmgFertilizer = float.Parse(m_truckInputs[5].text);

            // Skimmer
            m_modifyingVars.SkimmerSkimAmt = int.Parse(m_skimmerInputs[0].text);
            m_modifyingVars.SkimmerCycleTime = float.Parse(m_skimmerInputs[1].text);
            m_modifyingVars.SkimmerExpiredRunoffAmt = int.Parse(m_skimmerInputs[2].text);

            // Road
            m_modifyingVars.RoadStartHealth = float.Parse(m_roadInputs[0].text);
            m_modifyingVars.RoadDisrepairThreshold = float.Parse(m_roadInputs[1].text);

            // Tile Generator
            m_modifyingVars.TileGeneratorCycleTime = float.Parse(m_tileGeneratorInputs[0].text);
            TileGenerator.GenerateMode newMode = TileGenerator.GenerateMode.Replace;
            Debug.Log("[SettingsMgr] new tile dropdown str value: " + m_tileGeneratorDropdowns[0].value);
            if (m_tileGeneratorDropdowns[0].value == 1) { newMode = TileGenerator.GenerateMode.Boundary; }
            m_modifyingVars.TileGeneratorMode = newMode;


            // TODO: all below
            // Dairy Farm


            // Digester


            // Save final result
            m_currVars = m_modifyingVars;
        }

        private void DisplayCurrAllVars() {
            // Set all values to default values
            // City
            m_cityInputs[0].text = "" + m_currVars.CityPopStart;
            m_cityInputs[1].text = "" + m_currVars.CityCycleTime;
            m_cityInputs[2].text = "" + m_currVars.CityRequestTimeout;
            m_cityInputs[3].text = "" + m_currVars.CityProduceMoneyAmt;
            m_cityInputs[4].text = "" + m_currVars.CityBloomTolerance;

            // Grain Farm
            m_grainInputs[0].text = "" + m_currVars.GrainCycleTime;
            m_grainInputs[1].text = "" + m_currVars.GrainRequestTimeout;
            m_grainInputs[2].text = "" + m_currVars.GrainImportCost;
            m_grainInputs[3].text = "" + m_currVars.GrainPhosphPerManure;
            m_grainInputs[4].text = "" + m_currVars.GrainPhosphPerFertilizer;

            // Truck
            m_truckInputs[0].text = "" + m_currVars.TruckSpeed;
            m_truckInputs[1].text = "" + m_currVars.TruckLeakRate;
            m_truckInputs[2].text = "" + m_currVars.TruckLeakAmtManure;
            m_truckInputs[3].text = "" + m_currVars.TruckLeakAmtFertilizer;
            m_truckInputs[4].text = "" + m_currVars.TruckRoadDmgManure;
            m_truckInputs[5].text = "" + m_currVars.TruckRoadDmgFertilizer;

            // Skimmer
            m_skimmerInputs[0].text = "" + m_currVars.SkimmerSkimAmt;
            m_skimmerInputs[1].text = "" + m_currVars.SkimmerCycleTime;
            m_skimmerInputs[2].text = "" + m_currVars.SkimmerExpiredRunoffAmt;

            // Road
            m_roadInputs[0].text = "" + m_currVars.RoadStartHealth;
            m_roadInputs[1].text = "" + m_currVars.RoadDisrepairThreshold;

            // Tile Generator
            m_tileGeneratorInputs[0].text = "" + m_currVars.TileGeneratorCycleTime;
            m_tileGeneratorDropdowns[0].value = m_currVars.TileGeneratorMode == TileGenerator.GenerateMode.Replace ? 0 : 1;

            // Dairy Farm


            // Digester
        }

        #endregion // Helpers
    
        public AllVars GetCurrAllVars() {
            return m_currVars;
        }
    
    }
}
