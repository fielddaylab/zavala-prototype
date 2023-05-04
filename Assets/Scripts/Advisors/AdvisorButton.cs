using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;
using BeauUtil;
using static Zavala.Advisors.AdvisorGroup;
using Zavala.Events;

namespace Zavala.Advisors
{
    [RequireComponent(typeof(Button))]
    public class AdvisorButton : MonoBehaviour
    {
        public Button Button;
        public Image IconImage;
        public Image Base;
        public Image Outline;
        public AudioClip Shout;

        public RectTransform Root; // advisor button group

        [SerializeField] private Button m_policyButton;
        [SerializeField] private RectTransform m_policyButtonRect;

        private Routine m_HoverRoutine;

        public void LoadData(AdvisorButtonData data) {
            Button.onClick.RemoveAllListeners();

            IconImage.sprite = data.m_AdvisorImage;
            IconImage.SetNativeSize();
            Base.color = data.BaseColor;
            Outline.color = data.OutlineColor;
            Button.onClick.AddListener(delegate { HandleClick(data.m_AdvisorID); });
            m_policyButton.onClick.AddListener(delegate { HandlePolicyClick(data.m_AdvisorID); });
        }

        private void HandleClick(AdvisorID id) {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorButtonClicked, new AdvisorEventArgs(id));
        }

        private void HandlePolicyClick(AdvisorID id) {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorShown, new AdvisorEventArgs(id));
        }
    }
}