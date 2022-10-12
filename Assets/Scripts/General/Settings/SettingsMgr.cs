using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala
{
    public class SettingsMgr : MonoBehaviour
    {
        public static bool VisibleCycles = true;

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
        private float m_stashedTimescale;

        public void Init() {
            m_speedRange = m_fastestSpeed - m_slowestSpeed;

            m_timescaleSlider.onValueChanged.AddListener(HandleTimescaleValueChanged);

            // start at 1
            float startSpeed = 1.0f;
            m_timescaleSlider.value = (startSpeed - m_slowestSpeed) / m_speedRange;
            m_stashedTimescale = m_timescaleSlider.value;

            m_paused = m_stashedPaused = false;
            m_pauseButton.onClick.AddListener(HandlePauseClicked);

            EventMgr.Instance.NarrativeBlurbTriggered += HandleNarrativeBlurbTriggered;
            EventMgr.Instance.NarrativeBlurbClosed += HandleNarrativeBlurbClosed;
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

        #endregion // Handlers
    }
}
