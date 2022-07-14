using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zavala.Sim;

namespace Zavala
{

    public class AgrFarmInteractable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform rectTransform;
        private Vector3 lastValidPos;
        private Vector3 startPos;
        private bool hasMoved;

        private void Start() {
            rectTransform = GetComponent<RectTransform>();

            lastValidPos = startPos = rectTransform.anchoredPosition;

            hasMoved = false;
        }

        public void OnDrag(PointerEventData eventData) {
            rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerDown(PointerEventData eventData) {

        }

        public void OnPointerUp(PointerEventData eventData) {
            if (isValidDrop()) {
                lastValidPos = rectTransform.anchoredPosition;

                // update algal outbreaks
                EventMgr.FarmMoved?.Invoke();

                if (lastValidPos != startPos && !hasMoved) {
                    EventMgr.RegisterAction?.Invoke(SimAction.MoveFarm);
                    hasMoved = true;
                }
                else if ((lastValidPos == startPos) && hasMoved) {
                    EventMgr.RemoveAction?.Invoke(SimAction.MoveFarm);
                }
            }
            else {
                rectTransform.anchoredPosition = lastValidPos;
            }
        }

        private bool isValidDrop() {
            Collider2D hitCollider = Physics2D.OverlapPoint(this.gameObject.transform.position, 1 << LayerMask.NameToLayer("BaseMap"));

            if (hitCollider != null) {
                return true;
            }

            return false;
        }
    }
}