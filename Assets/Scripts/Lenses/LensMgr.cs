using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala.Lenses
{
    public enum Mode {
        Default,
        Phosphorus
    }

    public class LensMgr : MonoBehaviour
    {
        public static LensMgr Instance;

        [SerializeField] private Button m_lensButton;
        [SerializeField] private Image m_phosphGreyscale;

        private Mode m_currMode;

        public void Init() {
            Instance = this;

            m_currMode = Mode.Default;
            m_phosphGreyscale.enabled = false;

            m_lensButton.onClick.AddListener(OnLensButtonClicked);
        }

        public Mode GetLensMode() {
            return m_currMode;
        }

        #region Handlers 

        private void OnLensButtonClicked() {
            if (m_currMode == Mode.Default) {
                m_currMode = Mode.Phosphorus;
            }
            else {
                m_currMode = Mode.Default;
            }
            m_phosphGreyscale.enabled = m_currMode == Mode.Phosphorus;

            if (m_currMode == Mode.Phosphorus) {
                EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Phosphorus));
            }
            else {
                EventMgr.Instance.TriggerEvent(Events.ID.LensModeUpdated, new LensModeEventArgs(Lenses.Mode.Default));
            }

        }

        #endregion // Handlers
    }
}