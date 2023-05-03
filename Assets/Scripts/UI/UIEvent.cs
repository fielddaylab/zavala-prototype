using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;
using Zavala.Functionalities;
using Zavala.Lenses;

namespace Zavala
{
    public class UIEvent : MonoBehaviour
    {
        [SerializeField] private Transform m_rootTransform;
        [SerializeField] private CanvasGroup m_group;

        [SerializeField] private Image m_bg;
        private SimEventType m_eventType;

        public void Init(SimEventType type) {
            Sprite eventSprite = null;
            switch(type) {
                case SimEventType.ExcessRunoff:
                    eventSprite = GameDB.Instance.UIEventEcologyIcon;
                    if (LensMgr.Instance.GetLensMode() != Mode.Phosphorus) {
                        HideUI();
                    }
                    break;
                case SimEventType.PopDecline:
                    eventSprite = GameDB.Instance.UIEventEconomicIcon;
                    if (LensMgr.Instance.GetLensMode() != Mode.Economic) {
                        HideUI();
                    }
                    break;
                default:
                    break;
            }

            m_bg.sprite = eventSprite;

            m_eventType = type;

            EventMgr.Instance.LensModeUpdated += HandleLensModeUpdated;
        }

        private void OnDisable() {
            EventMgr.Instance.LensModeUpdated -= HandleLensModeUpdated;
        }

        private void ShowUI() {
            m_group.alpha = 1;
        }

        private void HideUI() {
            m_group.alpha = 0;
        }

        private void HandleLensModeUpdated(object sender, LensModeEventArgs args) {
            switch (args.Mode) {
                case Lenses.Mode.Default:
                    switch(m_eventType) {
                        default:
                            HideUI();
                            break;
                    }
                    break;
                case Lenses.Mode.Phosphorus:
                    switch (m_eventType) {
                        case SimEventType.ExcessRunoff:
                            ShowUI();
                            break;
                        default:
                            HideUI();
                            break;
                    }
                    break;

                case Lenses.Mode.Economic:
                    switch (m_eventType) {
                        case SimEventType.PopDecline:
                            ShowUI();
                            break;
                        default:
                            HideUI();
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}