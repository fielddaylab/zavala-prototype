using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala {
    public class TriggerTest : MonoBehaviour
    {
        [SerializeField] private Button[] m_buttons;

        private void Start() {
            m_buttons[0].onClick.AddListener(HandleB0);
            m_buttons[1].onClick.AddListener(HandleB1);
            m_buttons[2].onClick.AddListener(HandleB2);
        }

        #region Handlers (Triggers)

        private void HandleB0() {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This is a text summary 1!", Advisors.AdvisorID.Ecology));
        }

        private void HandleB1() {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This is a text summary 2!", Advisors.AdvisorID.Economic));
        }

        private void HandleB2() {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorBlurb, new AdvisorBlurbEventArgs("This is a silent update!", Advisors.AdvisorID.Ecology, true));
        }

        #endregion // Handlers
    }
}