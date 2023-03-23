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
        public Image Image;
        public AudioClip Shout;

        public RectTransform Root;

        [SerializeField] private AudioSource m_AudioSrc;

        private Routine m_HoverRoutine;

        public void LoadData(AdvisorButtonData data) {
            Button.onClick.RemoveAllListeners();

            Shout = data.Shout;
            Image.sprite = data.m_AdvisorImage;
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