using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala
{
    public class AdvisorUIMgr : MonoBehaviour
    {
        [SerializeField] private AdvisorButton[] m_Buttons;

        private Routine m_SwitchRegionRoutine;
        private Routine m_TransitionRoutine;

        private int m_NumActiveButtons;

        public void Init() {
            EventMgr.Instance.RegionSwitched += HandleRegionSwitched;
            m_NumActiveButtons = 0;
        }

        private IEnumerator SwitchRegion(AdvisorGroup[] advisors) {
            // hide old buttons
            yield return m_TransitionRoutine.Replace(HideButtons());

            // set new buttons
            for (int i = 0; i < m_Buttons.Length; i++) {
                if (i < advisors.Length) {
                    AdvisorGroup group = advisors[i];
                    m_Buttons[i].LoadData(group.ButtonData);
                    Debug.Log("[AdvisorUIMgr] loaded button " + i + " with audio " + group.ButtonData.Shout.name);
                }
            }

            m_NumActiveButtons = advisors.Length;

            // show new buttons
            yield return m_TransitionRoutine.Replace(ShowButtons(advisors.Length));
        }

        private IEnumerator HideButtons() {
            float time = 0.25f;
            float delay = 0.1f;

            for(int i = m_NumActiveButtons - 1; i >= 0; i--) {
                Routine.Start(m_Buttons[i].Root.AnchorPosTo(-100, time, Axis.Y).Ease(Curve.CubeIn).DelayBy(delay * (m_NumActiveButtons - 1 - i)));
            }

            yield return (m_NumActiveButtons * delay + time + 0.05f);
        }

        private IEnumerator ShowButtons(int num) {
            float time = 0.25f;
            float delay = 0.1f;

            for (int i = 0; i < num; i++) {
                Routine.Start(m_Buttons[i].Root.AnchorPosTo(0, time, Axis.Y).Ease(Curve.CubeOut).DelayBy(delay * i));
            }

            yield return (num * delay + time + 0.05f);
        }

        #region Handlers

        private void HandleRegionSwitched(object sender, RegionSwitchedEventArgs args) {
            if (args.NewRegion.Advisors.Length > m_Buttons.Length) {
                Debug.Log("[AdvisorUIMgr] Not enough allocated buttons for the nubmer of advisors in curr region!");
                return;
            }

            Debug.Log("[AdvisorUIMgr] Switching regions");

            m_SwitchRegionRoutine.Replace(SwitchRegion(args.NewRegion.Advisors));
        }

        #endregion // Handlers
    }
}
