using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Settings;

namespace Zavala.Advisors
{

    public class AdvisorBlurbBox : MonoBehaviour
    {
        public RectTransform Rect;
        public CanvasGroup Root;

        [SerializeField] private TMP_Text m_blurbText;
        [SerializeField] private Button m_newPoliciesButton;

        private Routine m_TransitionRoutine;

        private AdvisorBlurbEventArgs m_blurbArgs;
        private ChoiceUnlockEventArgs m_choiceUnlockArgs;

        private AdvisorID m_controllingAdvisor;

        private bool m_showing;

        public void ShowBlurb(AdvisorBlurbEventArgs args) {
            if (args.IsSilent) { return; }

            m_showing = true;
            m_blurbArgs = args;
            m_controllingAdvisor = args.AdvisorID;
            m_blurbText.text = args.Text;
            m_TransitionRoutine.Replace(ShowRoutine());

            m_newPoliciesButton.gameObject.SetActive(false);
        }

        public void ShowChoiceUnlockBlurb(ChoiceUnlockEventArgs args) {
            m_showing = true;
            m_choiceUnlockArgs = args;
            m_controllingAdvisor = args.AdvisorID;
            m_blurbText.text = args.Text;
            m_TransitionRoutine.Replace(ShowRoutine());

            m_newPoliciesButton.gameObject.SetActive(true);
            m_newPoliciesButton.onClick.AddListener(HandleNewPoliciesClick);
        }

        public void CloseBlurb() {
            m_TransitionRoutine.Replace(HideRoutine());
            m_showing = false;

            m_newPoliciesButton.onClick.RemoveAllListeners();
        }

        public bool IsShowing() {
            return m_showing;
        }

        public AdvisorID ControllingAdvisor() {
            return m_controllingAdvisor;
        }

        #region Routines

        private IEnumerator ShowRoutine() {
            yield return Rect.AnchorPosTo(-150, 0.3f, Axis.Y).Ease(Curve.CubeIn);
        }

        private IEnumerator HideRoutine() {
            yield return Rect.AnchorPosTo(-660, 0.3f, Axis.Y).Ease(Curve.CubeIn);
        }

        #endregion // Routines

        #region Handlers

        private void HandleNewPoliciesClick() {
            EventMgr.Instance.TriggerEvent(Events.ID.ViewNewPolicies, m_choiceUnlockArgs);

            CloseBlurb();
        }

        #endregion // Handlers
    }
}