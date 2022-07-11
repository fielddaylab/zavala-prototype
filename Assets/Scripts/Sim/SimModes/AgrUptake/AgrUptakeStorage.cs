using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zavala
{

    public class AgrUptakeStorage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform m_rectTransform;
        private Vector3 m_startPos;
        private bool m_wasOnMap;

        private void Start() {
            m_rectTransform = GetComponent<RectTransform>();

            m_startPos = m_rectTransform.anchoredPosition;

            m_wasOnMap = false;
        }

        public void OnDrag(PointerEventData eventData) {
            m_rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerDown(PointerEventData eventData) {

        }

        public void OnPointerUp(PointerEventData eventData) {
            if (OnMap()) {
                if (!m_wasOnMap) {
                    EventMgr.AgrUptakeStorageAdded?.Invoke();
                    m_wasOnMap = true;
                }
            }
            else {
                m_rectTransform.anchoredPosition = m_startPos;

                if (m_wasOnMap) {
                    EventMgr.AgrUptakeStorageRemoved?.Invoke();
                    m_wasOnMap = false;
                }
            }
        }

        private bool OnMap() {
            Collider2D hitCollider = Physics2D.OverlapPoint(this.gameObject.transform.position, 1 << LayerMask.NameToLayer("BaseMap"));

            if (hitCollider != null) {
                return true;
            }

            return false;
        }
    }
}