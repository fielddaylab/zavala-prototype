using System;
using System.Collections;
using System.Runtime.CompilerServices;
using BeauRoutine;
using BeauUtil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala {
    public sealed class AnimatedElement : MonoBehaviour {
        [HideInInspector] public Transform Transform;
        [HideInInspector] public RectTransform RectTransform;
        public CanvasGroup Group;
        public Graphic Graphic;
        public TMP_Text Text;
        public ColorGroup Colors;

        public Routine Animation;
        [NonSerialized] public TextAlignment Alignment;
        public object Data;

        #if UNITY_EDITOR

        private void Reset() {
            Transform = transform;
            RectTransform = Transform as RectTransform;
            Group = GetComponent<CanvasGroup>();
            Graphic = GetComponent<Graphic>();
            Colors = GetComponent<ColorGroup>();
        }

        private void OnValidate() {
            Transform = transform;
            RectTransform = Transform as RectTransform;
            if (!Text) {
                Text = Graphic as TMP_Text;
            }
        }

        #endif // UNITY_EDITOR

        static public void SetRaycasts(AnimatedElement element, bool raycasts) {
            if (element.Group) {
                element.Group.blocksRaycasts = raycasts;
            } else if (element.Graphic) {
                element.Graphic.raycastTarget = raycasts;
            } else if (element.Colors) {
                element.Colors.BlocksRaycasts = raycasts;
            }
        }

        static public void SetAlpha(AnimatedElement element, float alpha) {
            if (element.Group) {
                element.Group.alpha = alpha;
            } else if (element.Graphic) {
                element.Graphic.SetAlpha(alpha);
            } else if (element.Colors) {
                element.Colors.SetAlpha(alpha);
            }
        }

        static public float GetAlpha(AnimatedElement element) {
            if (element.Group) {
                return element.Group.alpha;
            } else if (element.Graphic) {
                return element.Graphic.GetAlpha();
            } else if (element.Colors) {
                return element.Colors.GetAlpha();
            } else {
                return 1;
            }
        }

        static public Tween FadeTo(AnimatedElement element, float alpha, float duration) {
            if (element.Group) {
                return element.Group.FadeTo(alpha, duration);
            } else if (element.Graphic) {
                return element.Graphic.FadeTo(alpha, duration);
            } else if (element.Colors) {
                return Tween.Float(element.Colors.GetAlpha(), alpha, element.Colors.SetAlpha, duration);
            } else {
                return null;
            }
        }

        static public void SwapText(AnimatedElement element, string newText) {
            if (element.Text && element.Text.text != newText) {
                element.Text.text = newText;
            }
        }

        static public IEnumerator SwapText(AnimatedElement element, string newText, float duration) {
            if (element.Text && element.Text.text != newText) {
                if (string.IsNullOrEmpty(newText)) {
                    yield return element.Text.FadeTo(0, duration);
                    element.Text.text = string.Empty;
                    element.Text.SetAlpha(1);
                } else if (!string.IsNullOrEmpty(element.Text.text)) {
                    yield return element.Text.FadeTo(0, duration / 2);
                    element.Text.text = newText;
                    yield return element.Text.FadeTo(1, duration / 2);
                } else {
                    element.Text.text = newText;
                    element.Text.SetAlpha(0);
                    yield return element.Text.FadeTo(1, duration);
                }
            }
        }

        [MethodImpl(256)]
        static public bool IsActive(AnimatedElement element) {
            return element.gameObject.activeSelf;
        }

        [MethodImpl(256)]
        static public void SetActive(AnimatedElement element, bool active) {
            element.gameObject.SetActive(active);
        }

        static public IEnumerator Show(AnimatedElement element, float duration, bool? raycasts = true) {
            if (!IsActive(element)) {
                SetAlpha(element, 0);
                SetActive(element, true);
            }
            if (raycasts.HasValue) {
                SetRaycasts(element, false);
            }
            yield return FadeTo(element, 1, duration);
            if (raycasts.HasValue) {
                SetRaycasts(element, raycasts.Value);
            }
        }

        static public void Show(AnimatedElement element, bool? raycasts = true) {
            SetAlpha(element, 1);
            if (raycasts.HasValue) {
                SetRaycasts(element, raycasts.Value);
            }
            SetActive(element, true);
        }

        static public IEnumerator Hide(AnimatedElement element, float duration, bool? raycasts = false) {
            if (IsActive(element)) {
                if (raycasts.HasValue) {
                    SetRaycasts(element, raycasts.Value);
                }
                yield return FadeTo(element, 0, duration);
                SetActive(element, false);
            }
        }

        static public void Hide(AnimatedElement element, bool? raycasts = false) {
            SetActive(element, false);
            SetAlpha(element, 0);
            if (raycasts.HasValue) {
                SetRaycasts(element, raycasts.Value);
            }
        }
    }
}