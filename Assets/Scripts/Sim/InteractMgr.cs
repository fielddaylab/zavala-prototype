using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala.Interact
{
    public enum InteractMode
    {
        Default,
        //Phosphorous,
        Transport_Rail,
        Transport_Highway,
        Transport_Road,
        Transport_Bridge,
        Transport_Remove
        // etc.
    }

    public class InteractMgr : MonoBehaviour
    {
        private Texture2D m_defaultCursor = null;
        [SerializeField] private Texture2D m_drawCursor;
        [SerializeField] private Texture2D m_bridgeCursor;
        [SerializeField] private Texture2D m_removeCursor;

        [SerializeField] private Sprite m_railIcon;
        [SerializeField] private Sprite m_highwayIcon;
        [SerializeField] private Sprite m_roadIcon;

        private InteractMode m_interactMode;

        [SerializeField] private GameObject m_linePrefab;
        [SerializeField] private GameObject m_bridgePrefab;
        [SerializeField] private GameObject m_linesContainer;
        [SerializeField] private GameObject m_bridgesContainer;
        private Vector2 m_startDrawPos;
        private Vector2 m_endDrawPos;
        private TransportLine m_currLine;

        private static Vector2 UNASSIGNED_V2 = new Vector2(-9999, -9999);


        #region Unity Callbacks

        private void Awake() {
            EventMgr.InteractModeUpdated.AddListener(HandleInteractModeUpdated);

            m_startDrawPos = m_endDrawPos = UNASSIGNED_V2;
        }

        private void OnDestroy() {
            EventMgr.InteractModeUpdated.RemoveListener(HandleInteractModeUpdated);
        }

        private void Update() {
            bool drawing = (m_interactMode == InteractMode.Transport_Rail)
                || (m_interactMode == InteractMode.Transport_Highway)
                || (m_interactMode == InteractMode.Transport_Road);

            if (drawing) {
                if (!m_startDrawPos.Equals(UNASSIGNED_V2)) {
                    // draw line from start to end point
                    Stretch(m_currLine.Image.gameObject, m_startDrawPos, Input.mousePosition, true);
                }

                if (Input.GetMouseButtonDown(0) && OnMap()) {
                    // check whether starting a new line or completing an existing one
                    if (m_startDrawPos.Equals(UNASSIGNED_V2)) {
                        // starting a new line
                        m_startDrawPos = Input.mousePosition;

                        m_currLine = Instantiate(m_linePrefab, m_linesContainer.transform).GetComponent<TransportLine>();
                        m_currLine.transform.localPosition = m_startDrawPos;
                    }
                    else {
                        // completing a line
                        m_startDrawPos = m_endDrawPos = UNASSIGNED_V2;
                    }
                }
            }

            if (m_interactMode == InteractMode.Transport_Bridge) {
                if (Input.GetMouseButtonDown(0) && OnMap()) {
                    // build a bridge
                    var bridge = Instantiate(m_bridgePrefab, m_bridgesContainer.transform);
                    bridge.transform.localPosition = Input.mousePosition;
                }
            }

            if (m_interactMode == InteractMode.Transport_Remove) {
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D removableCollider = OverlappingStructure();

                    if (removableCollider != null) {
                        removableCollider.GetComponent<TransportStructure>().Remove();
                    }
                    else {
                    }
                }
            }
        }

        private void Stretch(GameObject obj, Vector3 initialPosition, Vector3 finalPosition, bool mirrorZ) {
            float width = obj.GetComponent<Image>().rectTransform.rect.width;
            Vector3 centerPos = (initialPosition + finalPosition) / 2f;
            obj.transform.position = centerPos;
            Vector3 direction = finalPosition - initialPosition;
            direction = Vector3.Normalize(direction);
            obj.transform.right = direction;
            if (mirrorZ) obj.transform.right *= -1f;
            Vector3 scale = new Vector3(1, 1, 1);
            scale.x = Vector3.Distance(initialPosition, finalPosition) / width;
            obj.transform.localScale = scale;
        }

        private bool OnMap() {
            Collider2D hitCollider = Physics2D.OverlapPoint(Input.mousePosition, 1 << LayerMask.NameToLayer("BaseMap"));

            if (hitCollider != null) {
                return true;
            }

            return false;
        }

        private Collider2D OverlappingStructure() {
            Collider2D hitCollider = Physics2D.OverlapPoint(Input.mousePosition, 1 << LayerMask.NameToLayer("Structure"));

            return hitCollider;
        }

        #endregion // Unity Callbacks

        private void UpdateCursor(InteractMode inMode) {
            Texture2D newCursor = m_defaultCursor;
            Vector3 offset = Vector3.zero;

            switch (inMode) {
                default:
                    break;
                case InteractMode.Default:
                    break;
                case InteractMode.Transport_Rail:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_railIcon;
                    break;
                case InteractMode.Transport_Highway:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_highwayIcon;
                    break;
                case InteractMode.Transport_Road:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_roadIcon;
                    break;
                case InteractMode.Transport_Bridge:
                    newCursor = m_bridgeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case InteractMode.Transport_Remove:
                    newCursor = m_removeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
            }

            Cursor.SetCursor(newCursor, offset, CursorMode.ForceSoftware);
        }

        #region Event Handlers

        private void HandleInteractModeUpdated(InteractMode newMode) {
            m_interactMode = newMode;
            Debug.Log("[InteractMgr] New interact mode received: " + newMode);

            UpdateCursor(newMode);
        }

        #endregion // Event Handlers
    }
}