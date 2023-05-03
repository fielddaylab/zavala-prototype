using BeauRoutine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Lenses;

namespace Zavala
{
    public class UIRequest : MonoBehaviour
    {
        [SerializeField] private Transform m_rootTransform;
        [SerializeField] private CanvasGroup m_group;

        [SerializeField] private Image m_bg;
        [SerializeField] private Image m_unitsBG;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private TMP_Text m_unitsText;

        private Resources.Type m_resourceType;

        private int m_initialUnits; // how many units in total request
        private int m_units;// how many units left to complete request
        private int m_enRouteUnits; // how many units are currently on their way
        private bool m_continuous;

        //private UITimer m_uiTimer;

        private int m_remainingCycles;

        public event EventHandler TimerExpired; // when the timer completes

        private Cycles m_cycleSync;

        private Routine m_TransitionRoutine;

        private void InitBasics(Resources.Type resourceType, int units, bool visible, bool continuous) {
            if (visible) {
                m_resourceIcon.sprite = GameDB.Instance.GetResourceRequestIcon(resourceType);
                m_resourceIcon.SetNativeSize();
            }
            else {
                m_resourceIcon.gameObject.SetActive(false);
                m_unitsText.gameObject.SetActive(false);
                m_bg.gameObject.SetActive(false);
                m_unitsBG.gameObject.SetActive(false);
            }

            m_resourceType = resourceType;
            m_enRouteUnits = 0;
            m_units = m_initialUnits = units;
            m_continuous = continuous;

            UpdateUnitsText();

            m_remainingCycles = -1;

            if (LensMgr.Instance.GetLensMode() != Mode.Default) {
                HideUI();
            }

            EventMgr.Instance.LensModeUpdated += HandleLensModeUpdated;
        }

        // no timeout
        public void Init(Resources.Type resourceType, int units, bool visible, bool continuous) {
            InitBasics(resourceType, units, visible, continuous);
        }

        // with timeout
        public void Init(Resources.Type resourceType, int requestTimeout, Cycles cycleSync, int units, bool visible, bool continuous) {
            InitBasics(resourceType, units, visible, continuous);

            m_cycleSync = cycleSync;
            m_cycleSync.PreCycleCompleted += HandlePreCycleCompleted;
            m_remainingCycles = requestTimeout;

            m_bg.color = GameDB.Instance.UIRequestDefaultColor;

            if (m_rootTransform != null) {
                m_rootTransform.SetScale(0);
                m_TransitionRoutine.Replace(ShowRoutine());
            }

            //m_uiTimer = Instantiate(GameDB.Instance.UITimerPrefabDefault, this.transform).GetComponent<UITimer>();
            //m_uiTimer.Init(requestTimeout, false);
            //m_uiTimer.TimerCompleted += HandleTimerCompleted;
        }

        private void OnDisable() {
            m_TransitionRoutine.Stop();

            EventMgr.Instance.LensModeUpdated -= HandleLensModeUpdated;
        }

        private IEnumerator ShowRoutine() {
            yield return m_rootTransform.ScaleTo(1, 1f).Ease(Curve.CubeIn);
        }

        public IEnumerator Fulfill() {
            yield return m_TransitionRoutine.Replace(FulfillRoutine());
        }

        private IEnumerator FulfillRoutine() {
            yield return m_rootTransform.ScaleTo(0, 1f).Ease(Curve.CubeIn);
        }

        public IEnumerator Fade() {
            yield return m_TransitionRoutine.Replace(FadeRoutine());
        }

        private IEnumerator FadeRoutine() {
            yield return Routine.Combine(
                m_rootTransform.MoveTo(this.transform.position + this.transform.up * 0.25f + this.transform.right * 0.1f, 1f, Axis.XYZ, Space.World),
                m_rootTransform.RotateTo(-30, 1f, Axis.Z),
                m_group.FadeTo(0, 1f).Ease(Curve.CubeIn)
                );
        }

        public Resources.Type GetResourceType() {
            return m_resourceType;
        }

        public int GetFulfillableUnits() {
            return m_units - m_enRouteUnits;
        }

        public int GetInitialUnits() {
            return m_initialUnits;
        }

        public int GetRemainingUnits() {
            return m_units;
        }

        public bool IsContinuous() {
            return m_continuous;
        }

        public void SetEnRoute(int enRouteUnits) {
            m_enRouteUnits += enRouteUnits;

            // TODO: nuance here. If a request is partially fulfilled, remaining products must continue to be tracked as not en-route
            //m_bg.color = GameDB.Instance.UIRequestEnRouteColor;
        }

        public int GetEnRoute() {
            return m_enRouteUnits;
        }

        public void ModifyUnits(int amt) {
            if (!m_continuous) {
                m_units += amt;

                if (amt < 0) {
                    // en route units have been delivered
                    m_enRouteUnits += amt;
                }
            }

            UpdateUnitsText();
        }

        public void UpdateUnitsText() {
            if (m_continuous) {
                m_unitsText.text = "Inf";
            }
            else {
                m_unitsText.text = "" + m_units;
            }
        }

        private void MarkUrgent() {
            m_bg.sprite = GameDB.Instance.UIRequestExpiringBGIcon;
            // m_bg.color = GameDB.Instance.UIRequestExpiringColor;
        }

        private void ShowUI() {
            m_group.alpha = 1;
        }

        private void HideUI() {
            m_group.alpha = 0;
        }

        #region Handlers

        /*
        private void HandleTimerCompleted(object sender, EventArgs e) {
            Debug.Log("[UIRequest] Timer completed");

            RequestExpired?.Invoke(this, EventArgs.Empty);
        }
        */

        private void HandlePreCycleCompleted(object sender, EventArgs e) {
            if (m_remainingCycles == -1) {
                // request has no expiry
                return;
            }
            else {
                // tick cycles
                m_remainingCycles--;

                if (m_remainingCycles == 0) {
                    TimerExpired?.Invoke(this, EventArgs.Empty);
                }
                else if (m_remainingCycles == 1) {
                    if (m_bg != null) {
                        MarkUrgent();
                    }
                }
            }
        }

        private void HandleLensModeUpdated(object sender, LensModeEventArgs args) {
            switch (args.Mode) {
                case Lenses.Mode.Default:
                    ShowUI();
                    break;
                default:
                    HideUI();
                    break;
            }
        }

        #endregion // Handlers
    }
}