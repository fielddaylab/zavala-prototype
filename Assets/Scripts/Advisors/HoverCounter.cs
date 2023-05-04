using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    public class HoverCounter : MonoBehaviour
    {
        public RectTransform Root; // advisor button group

        [SerializeField] private RectTransform m_policyButtonRect;

        private int m_counted;

        private void OnEnable() {
            m_counted = 0;
        }

        public void RegisterHover() {
            int prevVal = m_counted;

            m_counted++;

            if (m_counted > 0 && prevVal == 0) {
                BeginHoverRoutine();
            }
        }

        public void DeregisterHover() {
            m_counted--;

            if (m_counted == 0) {
                EndHoverRoutine();
            }
        }

        public void BeginHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(15, 0.15f, Axis.Y).Ease(Curve.CubeOut));
            Routine.Start(m_policyButtonRect.AnchorPosTo(30, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }

        public void EndHoverRoutine() {
            Routine.Start(Root.AnchorPosTo(0, 0.15f, Axis.Y).Ease(Curve.CubeOut));
            Routine.Start(m_policyButtonRect.AnchorPosTo(0, 0.15f, Axis.Y).Ease(Curve.CubeOut));
        }
    }
}