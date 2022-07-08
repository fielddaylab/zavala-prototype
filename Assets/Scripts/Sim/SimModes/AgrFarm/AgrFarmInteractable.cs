using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AgrFarmInteractable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 lastValidPos;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();

        lastValidPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData) {

    }

    public void OnPointerUp(PointerEventData eventData) {
        if (isValidDrop()) {
            lastValidPos = rectTransform.anchoredPosition;
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
