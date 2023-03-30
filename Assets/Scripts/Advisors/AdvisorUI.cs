using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauUtil;
using BeauRoutine;
using UnityEngine.UI;
using TMPro;

namespace Zavala
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AnimatedElement))]
    public class AdvisorUI : MonoBehaviour
    {
        public RectTransform Rect;
        public CanvasGroup Root;
        public AnimatedElement AnimElement;

        [SerializeField] private ChoiceSlot[] m_ChoiceSlots;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TMP_Text m_SummaryText;
        [SerializeField] private Color m_ColorTheme;

        private Routine m_TransitionRoutine;

        public void Show() {
            m_TransitionRoutine.Replace(ShowRoutine());
            m_CloseButton.onClick.AddListener(Hide);
        }

        public void Hide() {
            m_TransitionRoutine.Replace(HideRoutine());

            m_CloseButton.onClick.RemoveListener(Hide);
        }

        #region Routines

        private IEnumerator ShowRoutine() {
            yield return Rect.AnchorPosTo(0, 0.3f, Axis.Y).Ease(Curve.CubeIn);
        }

        private IEnumerator HideRoutine() {
            yield return Rect.AnchorPosTo(-660, 0.3f, Axis.Y).Ease(Curve.CubeIn);
        }

        #endregion // Routines
    }
}
