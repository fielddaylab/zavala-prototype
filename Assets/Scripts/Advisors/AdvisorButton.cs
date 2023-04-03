using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;
using BeauUtil;
using static System.TimeZoneInfo;
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

        public RectTransform Root;

        private Routine m_HoverRoutine;

        public void LoadData(AdvisorButtonData data) {
            Button.onClick.RemoveAllListeners();

            IconImage.sprite = data.m_AdvisorImage;
            IconImage.SetNativeSize();
            Base.color = data.BaseColor;
            Outline.color = data.OutlineColor;
            Button.onClick.AddListener(delegate { HandleClick(data.m_AdvisorID); });
        }

        private void HandleClick(AdvisorID id) {
            EventMgr.Instance.TriggerEvent(Events.ID.AdvisorShown, new AdvisorEventArgs(id));
        }

        public void BeginHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(15, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }

        public void EndHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(0, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }
    }
}