using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Lenses;

namespace Zavala
{
    public class PhosphPip : MonoBehaviour
    {
        private SpriteRenderer m_renderer;

        private void Awake() {
            m_renderer = this.GetComponent<SpriteRenderer>();
        }

        private void Start() {
            m_renderer.enabled = LensMgr.Instance.GetLensMode() == Lenses.Mode.Phosphorus;

            EventMgr.Instance.LensModeUpdated += HandleLensModeUpdated;
        }

        #region Handlers

        private void HandleLensModeUpdated(object sender, LensModeEventArgs args) {
            switch(args.Mode) {
                case Lenses.Mode.Default:
                    m_renderer.enabled = false;
                    break;
                case Lenses.Mode.Phosphorus:
                    m_renderer.enabled = true;
                    break;
                default:
                    break;
            }
        }

        #endregion // Handlers
    }
}
