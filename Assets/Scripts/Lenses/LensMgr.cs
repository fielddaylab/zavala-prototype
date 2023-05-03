using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Advisors;
using Zavala.Events;

namespace Zavala.Lenses
{
    public enum Mode {
        Default,
        Phosphorus,
        Economic
    }

    public class LensMgr : MonoBehaviour
    {
        public static LensMgr Instance;

        [SerializeField] private Image m_phosphGreyscale;

        private Mode m_currMode;

        public void Init() {
            Instance = this;

            m_currMode = Mode.Default;
            m_phosphGreyscale.enabled = false;

            EventMgr.Instance.AdvisorBlurb += HandleAdvisorBlurb;
            EventMgr.Instance.ChoiceUnlock += HandleChoiceUnlock;
            EventMgr.Instance.AdvisorShown += HandleAdvisorShown;
            EventMgr.Instance.AdvisorsNoReplacement += HandleAdvisorsNoReplacement;
        }

        public Mode GetLensMode() {
            return m_currMode;
        }

        #region Handlers 

        private void HandleAdvisorShown(object sender, AdvisorEventArgs args) {
            UpdateLens(args.AdvisorID);
        }

        private void HandleAdvisorBlurb(object sender, AdvisorBlurbEventArgs args) {
            UpdateLens(args.AdvisorID);
        }

        private void HandleChoiceUnlock(object sender, ChoiceUnlockEventArgs args) {
            UpdateLens(args.AdvisorID);
        }

        private void UpdateLens(AdvisorID id) {
            m_phosphGreyscale.enabled = false;

            switch (id) {
                case Advisors.AdvisorID.Ecology:
                    m_currMode = Mode.Phosphorus;
                    m_phosphGreyscale.enabled = true;

                    EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Phosphorus));
                    break;
                case Advisors.AdvisorID.Economic:
                    m_currMode = Mode.Economic;

                    EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Economic));
                    break;
                default:
                    break;
            }
        }

        private void HandleAdvisorsNoReplacement(object sender, EventArgs args) {
            m_currMode = Mode.Default;

            m_phosphGreyscale.enabled = false;

            EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Default));
        }

        #endregion // Handlers
    }
}