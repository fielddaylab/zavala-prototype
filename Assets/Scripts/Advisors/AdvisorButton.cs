using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;
using BeauUtil;
using static System.TimeZoneInfo;
using static Zavala.AdvisorGroup;

namespace Zavala
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(AudioSource))]
    public class AdvisorButton : MonoBehaviour
    {
        public Button Button;
        public Image IconImage;
        public Image Base;
        public Image Outline;
        public AudioClip Shout;

        public RectTransform Root;

        [SerializeField] private AudioSource m_AudioSrc;

        private Routine m_HoverRoutine;

        public void LoadData(AdvisorButtonData data) {
            Button.onClick.RemoveAllListeners();

            Shout = data.Shout;
            IconImage.sprite = data.m_AdvisorImage;
            IconImage.SetNativeSize();
            Base.color = data.BaseColor;
            Outline.color = data.OutlineColor;
            Button.onClick.AddListener(delegate { data.m_UI.Show(); });
            Button.onClick.AddListener(PlayShout);
        }

        private void PlayShout() {
            m_AudioSrc.PlayOneShot(Shout);
        }

        public void BeginHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(15, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }

        public void EndHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(0, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }
    }
}