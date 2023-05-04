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

            EventMgr.Instance.AdvisorSelected += HandleAdvisorSelected;
            EventMgr.Instance.AdvisorSelectToggle += HandleAdvisorSelectToggle;
            EventMgr.Instance.AdvisorsNoReplacement += HandleAdvisorsNoReplacement;
        }

        public Mode GetLensMode() {
            return m_currMode;
        }

        #region Handlers 

        private void HandleAdvisorSelected(object sender, AdvisorEventArgs args) {
            UpdateLens(args.AdvisorID, false);
        }

        private void HandleAdvisorSelectToggle(object sender, AdvisorEventArgs args) {
            UpdateLens(args.AdvisorID, true);
        }

        private void UpdateLens(AdvisorID id, bool toggle) {
            m_phosphGreyscale.enabled = false;

            switch (id) {
                case Advisors.AdvisorID.Ecology:
                    if (toggle && m_currMode == Mode.Phosphorus) { SetDefaultLens(); break; }

                    m_currMode = Mode.Phosphorus;
                    m_phosphGreyscale.enabled = true;

                    EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Phosphorus));
                    break;
                case Advisors.AdvisorID.Economic:
                    if (toggle && m_currMode == Mode.Economic) { SetDefaultLens(); break; }

                    m_currMode = Mode.Economic;

                    EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Economic));
                    break;
                default:
                    break;
            }
        }

        private void SetDefaultLens() {
            m_currMode = Mode.Default;

            EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Default));
        }

        private void HandleAdvisorsNoReplacement(object sender, EventArgs args) {
            m_currMode = Mode.Default;

            m_phosphGreyscale.enabled = false;

            EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Default));
        }

        #endregion // Handlers
    }
}