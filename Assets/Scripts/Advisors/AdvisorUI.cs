using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauUtil;
using BeauRoutine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Zavala.Events;
using System;
using Zavala.Cards;

namespace Zavala.Advisors
{
    public enum AdvisorID
    {
        Other,
        Ecology,
        Economic,
        Goose,
        Dream,
        Kami
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AnimatedElement))]
    [RequireComponent(typeof(AudioSource))]
    public class AdvisorUI : MonoBehaviour
    {
        public RectTransform Rect;
        public CanvasGroup Root;
        public AnimatedElement AnimElement;

        [SerializeField] private AdvisorID m_advisorID;
        [SerializeField] private bool m_IsGlobal;

        [SerializeField] private ChoiceSlot[] m_ChoiceSlots;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TMP_Text m_SummaryText;
        [SerializeField] private string m_DefaultText;

        public AudioClip Shout;

        [SerializeField] private AudioSource m_AudioSrc;

        private List<SlotCard> m_newCards;

        public Color ColorTheme;

        private Routine m_TransitionRoutine;

        private void OnEnable() {
            for (int i = 0; i < m_ChoiceSlots.Length; i++) {
                m_ChoiceSlots[i].SetGlobal(m_IsGlobal);
            }

            EventMgr.Instance.AdvisorBlurb += HandleAdvisorBlurb;
            EventMgr.Instance.ChoiceUnlock += HandleChoiceUnlock;
            EventMgr.Instance.AdvisorShown += HandleAdvisorShown;

            m_SummaryText.text = m_DefaultText;
        }

        public void Show() {
            m_TransitionRoutine.Replace(ShowRoutine());
            m_CloseButton.onClick.AddListener(Hide);
            PlayShout();
        }

        public void Hide() {
            // remove unlocked cards, if any
            if (m_newCards != null) {
                for (int i = 0; i < m_newCards.Count; i++) {
                    Destroy(m_newCards[i].gameObject);
                }

                for (int i = 0; i < m_ChoiceSlots.Count(); i++) {
                    m_ChoiceSlots[i].ActivateButton();
                }
            }
            m_newCards = null;

            m_TransitionRoutine.Replace(HideRoutine());

            m_CloseButton.onClick.RemoveListener(Hide);
        }

        private void PlayShout() {
            m_AudioSrc.PlayOneShot(Shout);
        }

        #region Routines

        private IEnumerator ShowRoutine() {
            yield return Rect.AnchorPosTo(0, 0.3f, Axis.Y).Ease(Curve.CubeIn);
        }

        private IEnumerator HideRoutine() {
            yield return Rect.AnchorPosTo(-660, 0.3f, Axis.Y).Ease(Curve.CubeIn);

            m_SummaryText.text = m_DefaultText;
        }

        #endregion // Routines

        #region Handlers

        private void HandleAdvisorBlurb(object sender, AdvisorBlurbEventArgs args) {
            if (args.AdvisorID != m_advisorID) {
                // hide this so only blurbing advisor is showing
                Hide();
                return;
            }

            m_SummaryText.text = args.Text;

            if (!args.IsSilent) {
                Show();
            }
        }

        private void HandleChoiceUnlock(object sender, ChoiceUnlockEventArgs args) {
            if (args.AdvisorID != m_advisorID) {
                // hide this so only blurbing advisor is showing
                Hide();
                return;
            }

            // generate instances of unlocked cards
            List<CardData> availableCards = new List<CardData>();

            m_newCards = new List<SlotCard>();

            for (int i = 0; i < args.ToUnlock.Count; i++) {
                CardData data = CardMgr.Instance.GetCardData(args.ToUnlock[i]);

                GameObject cardObj = Instantiate(CardMgr.Instance.SlotCardPrefab, m_ChoiceSlots[i].transform);
                SlotCard card = cardObj.GetComponent<SlotCard>();

                card.Display(data, this);
                m_newCards.Add(card);
            }

            for (int i = 0; i < m_ChoiceSlots.Count(); i++) {
                m_ChoiceSlots[i].DeactivateButton();
            }

            m_SummaryText.text = args.Text;

            Show();
        }

        private void HandleAdvisorShown(object sender, AdvisorEventArgs args) {
            if (args.AdvisorID == m_advisorID) {
                Show();
            }
            else {
                Hide();
            }
        }

        #endregion // Handlers
    }
}
