using BeauRoutine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala.Advisors
{
    public class AdvisorUIMgr : MonoBehaviour
    {
        [SerializeField] private AdvisorButton[] m_GlobalButtons;
        [SerializeField] private AdvisorButton[] m_RegionalButtons;

        [SerializeField] AdvisorGroup[] m_GlobalAdvisors;

        [SerializeField] private AdvisorBlurbBox m_AdvisorBlurb;

        private Routine m_RefreshGlobalAdvisorsRoutine;
        private Routine m_GlobalTransitionRoutine;

        private Routine m_SwitchRegionRoutine;
        private Routine m_RegionalTransitionRoutine;

        private int m_NumActiveButtonsRegional;
        private int m_NumActiveButtonsGlobal;

        public void Init() {
            EventMgr.Instance.RegionSwitched += HandleRegionSwitched;
            m_NumActiveButtonsRegional = m_NumActiveButtonsGlobal = 0;

            m_RefreshGlobalAdvisorsRoutine.Replace(RefreshGlobalAdvisors());

            EventMgr.Instance.AdvisorBlurb += HandleAdvisorBlurb;
            EventMgr.Instance.ChoiceUnlock += HandleChoiceUnlock;
            EventMgr.Instance.AdvisorButtonClicked += HandleAdvisorButtonClicked;
        }

        private IEnumerator RefreshGlobalAdvisors() {
            // hide old buttons
            yield return m_GlobalTransitionRoutine.Replace(HideGlobalButtons());

            // set new buttons
            for (int i = 0; i < m_GlobalButtons.Length; i++) {
                if (i < m_GlobalAdvisors.Length) {
                    AdvisorGroup group = m_GlobalAdvisors[i];
                    m_GlobalButtons[i].LoadData(group.ButtonData);
                }
            }

            m_NumActiveButtonsGlobal = m_GlobalAdvisors.Length;

            // show new buttons
            yield return m_GlobalTransitionRoutine.Replace(ShowGlobalButtons(m_GlobalAdvisors.Length));
        }

        private IEnumerator HideGlobalButtons() {
            float time = 0.25f;
            float delay = 0.1f;

            for (int i = m_NumActiveButtonsGlobal - 1; i >= 0; i--) {
                Routine.Start(m_GlobalButtons[i].Root.AnchorPosTo(-100, time, Axis.Y).Ease(Curve.CubeIn).DelayBy(delay * (m_NumActiveButtonsGlobal - 1 - i)));
            }

            yield return (m_NumActiveButtonsGlobal * delay + time + 0.05f);
        }

        private IEnumerator ShowGlobalButtons(int num) {
            float time = 0.25f;
            float delay = 0.1f;

            for (int i = 0; i < num; i++) {
                Routine.Start(m_GlobalButtons[i].Root.AnchorPosTo(0, time, Axis.Y).Ease(Curve.CubeOut).DelayBy(delay * i));
            }

            yield return (num * delay + time + 0.05f);
        }

        private IEnumerator SwitchRegion(AdvisorGroup[] advisors) {
            // hide old buttons
            yield return m_RegionalTransitionRoutine.Replace(HideRegionalButtons());

            // set new buttons
            for (int i = 0; i < m_RegionalButtons.Length; i++) {
                if (i < advisors.Length) {
                    AdvisorGroup group = advisors[i];
                    m_RegionalButtons[i].LoadData(group.ButtonData);
                    Debug.Log("[AdvisorUIMgr] loaded button " + i + " with audio " + group.ButtonData.Shout.name);
                }
            }

            m_NumActiveButtonsRegional = advisors.Length;

            // show new buttons
            yield return m_RegionalTransitionRoutine.Replace(ShowRegionalButtons(advisors.Length));
        }

        private IEnumerator HideRegionalButtons() {
            float time = 0.25f;
            float delay = 0.1f;

            for(int i = m_NumActiveButtonsRegional - 1; i >= 0; i--) {
                Routine.Start(m_RegionalButtons[i].Root.AnchorPosTo(-100, time, Axis.Y).Ease(Curve.CubeIn).DelayBy(delay * (m_NumActiveButtonsRegional - 1 - i)));
            }

            yield return (m_NumActiveButtonsRegional * delay + time + 0.05f);
        }

        private IEnumerator ShowRegionalButtons(int num) {
            float time = 0.25f;
            float delay = 0.1f;

            for (int i = 0; i < num; i++) {
                Routine.Start(m_RegionalButtons[i].Root.AnchorPosTo(0, time, Axis.Y).Ease(Curve.CubeOut).DelayBy(delay * i));
            }

            yield return (num * delay + time + 0.05f);
        }

        #region Handlers

        private void HandleRegionSwitched(object sender, RegionSwitchedEventArgs args) {
            if (args.NewRegion.Advisors.Length > m_RegionalButtons.Length) {
                Debug.Log("[AdvisorUIMgr] Not enough allocated buttons for the nubmer of advisors in curr region!");
                return;
            }

            Debug.Log("[AdvisorUIMgr] Switching regions");

            m_SwitchRegionRoutine.Replace(SwitchRegion(args.NewRegion.Advisors));
        }

        private void HandleAdvisorButtonClicked(object sender, AdvisorEventArgs args) {
            // if showing blurb, clicking button will close blurb
            if (m_AdvisorBlurb.IsShowing()) {
                m_AdvisorBlurb.CloseBlurb();
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorNoReplacement, EventArgs.Empty);

                // if not same advisor, show other advisor
                if (m_AdvisorBlurb.ControllingAdvisor() != args.AdvisorID) {
                    EventMgr.Instance.TriggerEvent(Events.ID.AdvisorSelectToggle, new AdvisorEventArgs(args.AdvisorID));
                }
            }
            else {
                // else advisor lens should be shown
                EventMgr.Instance.TriggerEvent(Events.ID.AdvisorSelectToggle, new AdvisorEventArgs(args.AdvisorID));
            }
        }

        private void HandleAdvisorBlurb(object sender, AdvisorBlurbEventArgs args) {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorSelected, new AdvisorEventArgs(args.AdvisorID));
            m_AdvisorBlurb.ShowBlurb(args);
        }

        private void HandleChoiceUnlock(object sender, ChoiceUnlockEventArgs args) {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorSelected, new AdvisorEventArgs(args.AdvisorID));
            m_AdvisorBlurb.ShowChoiceUnlockBlurb(args);
        }

        #endregion // Handlers
    }
}
