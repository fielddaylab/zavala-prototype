using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauUtil;
using BeauRoutine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Zavala
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AnimatedElement))]
    public class AdvisorUI : MonoBehaviour
    {
        public RectTransform Rect;
        public CanvasGroup Root;
        public AnimatedElement AnimElement;

        [SerializeField] private bool m_IsGlobal;

        [SerializeField] private ChoiceSlot[] m_ChoiceSlots;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TMP_Text m_SummaryText;

        public Color ColorTheme;

        private Routine m_TransitionRoutine;

        private void OnEnable() {
            for (int i = 0; i < m_ChoiceSlots.Length; i++) {
                m_ChoiceSlots[i].SetGlobal(m_IsGlobal);
            }
        }

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
