using BeauRoutine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Advisors;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Sim;

namespace Zavala
{
    public class SlotClickable : MonoBehaviour
    {
        private bool m_handVisible;

        List<SlotCard> m_displayCards;

        [SerializeField] AdvisorUI m_advisorUI;

        private SlotCard m_selectedCard;

        private Routine m_transitionRoutine;
        private bool m_hiding;

        private void OnEnable() {
            m_handVisible = false;
            m_hiding = false;
            m_selectedCard = null;
        }

        public void SetAdvisorUI(AdvisorUI ui) {
            m_advisorUI = ui;
        }

        public void HandleClick(SimLeverID slotType, bool isGlobal) {
            // show selections to replace
            if (m_handVisible) {
                HideHand();
            }
            else {
                EventMgr.Instance.ChoiceSlotUpdated += HandleChoiceSlotUpdated;

                // query cardmgr for relevant cards for this slot type
                List<CardData> availableCards = CardMgr.Instance.GetOptions(slotType);

                m_displayCards = new List<SlotCard>();

                foreach (CardData data in availableCards) {
                    if (m_selectedCard != null && data.CardID == m_selectedCard.GetCardData().CardID) { continue; } // filter out current selction

                    GameObject cardObj = Instantiate(CardMgr.Instance.SlotCardPrefab, this.transform);
                    SlotCard card = cardObj.GetComponent<SlotCard>();

                    card.Init(data, m_advisorUI, this, isGlobal);
                    m_displayCards.Add(card);
                }

                ShowHand();
            }
        }

        private void ShowHand() {
            m_transitionRoutine.Replace(ShowHandRoutine());
        }

        private void HideHand() {
            EventMgr.Instance.ChoiceSlotUpdated -= HandleChoiceSlotUpdated;

            if (!m_hiding) {
                m_transitionRoutine.Replace(HideHandRoutine());
            }
        }


        private IEnumerator ShowHandRoutine() {
            float offset = 0.5f;
            float leftMost = 55 * (m_displayCards.Count - 1);
            float rotatedMost = 7.5f * (m_displayCards.Count - 1);
            float topMost = 155;
            for (int i = 0; i < m_displayCards.Count; i++) {
                Transform cardTransform = m_displayCards[i].transform;
                Routine.Start(
                    Routine.Combine(
                        cardTransform.MoveTo(new Vector3(
                            cardTransform.position.x - leftMost + 110 * i,
                            cardTransform.position.y + topMost - 22 * Mathf.Abs((i + offset) - (m_displayCards.Count / 2.0f)),
                            cardTransform.position.z
                        ), .3f, Axis.XY),
                        cardTransform.RotateTo(cardTransform.rotation.z + rotatedMost - 15f * i, .3f, Axis.Z)
                    )
                );
            }
            m_handVisible = true;

            yield return null;
        }


        private IEnumerator HideHandRoutine() {
            m_hiding = true;

            for (int i = 0; i < m_displayCards.Count; i++) {
                Transform cardTransform = m_displayCards[i].transform;
                Routine.Start(
                    Routine.Combine(
                    cardTransform.MoveTo(this.transform.position, .3f, Axis.XY),
                    cardTransform.RotateTo(0, .3f, Axis.Z)
                    )
                );
            }

            yield return 0.35f;

            for (int i = 0; i < m_displayCards.Count; i++) {
                if (!m_displayCards[i].GetActivated()) {
                    Destroy(m_displayCards[i].gameObject);
                }
            }
            m_displayCards.Clear();

            m_handVisible = false;
            m_hiding = false;
        }

        #region Handlers

        private void HandleChoiceSlotUpdated(object sender, ChoiceSlotEventArgs args) {
            if (m_selectedCard != null) {
                Destroy(m_selectedCard.gameObject);
            }

            HideHand();

            m_selectedCard = args.Card;
        }

        #endregion // Handlers
    }
}