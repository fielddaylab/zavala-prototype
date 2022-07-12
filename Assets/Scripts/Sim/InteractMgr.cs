using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Transport;

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

        private static float RAIL_COST = 20;
        private static float HIGHWAY_COST = 20;
        private static float ROAD_COST = 20;
        private static float BRIDGE_COST = 20;



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

                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // check whether starting a new line or completing an existing one
                    if (m_startDrawPos.Equals(UNASSIGNED_V2)) {
                        // starting a new line
                        m_startDrawPos = Input.mousePosition;

                        m_currLine = Instantiate(m_linePrefab, m_linesContainer.transform).GetComponent<TransportLine>();
                        m_currLine.transform.localPosition = m_startDrawPos;
                    }
                    else {
                        // completing a line
                        m_endDrawPos = Input.mousePosition;

                        // check if touches a farm and a sink (if so, reduce algal outbreak)
                        bool reducesOutbreaks = (TouchesOutput(m_startDrawPos) && TouchesSink(m_endDrawPos))
                            || (TouchesSink(m_startDrawPos) && TouchesOutput(m_endDrawPos));

                        AssignDetails(m_interactMode, m_currLine.GetComponent<TransportStructure>(), reducesOutbreaks);
                        m_currLine.GetComponent<TransportStructure>().Build();

                        m_startDrawPos = m_endDrawPos = UNASSIGNED_V2;
                    }
                }
            }

            if (m_interactMode == InteractMode.Transport_Bridge) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // build a bridge
                    var bridge = Instantiate(m_bridgePrefab, m_bridgesContainer.transform);
                    bridge.transform.localPosition = Input.mousePosition;

                    AssignDetails(m_interactMode, bridge.GetComponent<TransportStructure>(), false);
                    bridge.GetComponent<TransportStructure>().Build();
                }
            }

            if (m_interactMode == InteractMode.Transport_Remove) {
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D removableCollider = OverlappingStructure(Input.mousePosition);

                    if (removableCollider != null) {
                        removableCollider.GetComponent<TransportStructure>().Remove();
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

        private bool OnMap(Vector2 pos) {
            Collider2D hitCollider = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("BaseMap"));

            return (hitCollider != null);
        }

        private Collider2D OverlappingStructure(Vector2 pos) {
            Collider2D hitCollider = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("Structure"));

            return hitCollider;
        }

        private bool TouchesSink(Vector2 pos) {
            Collider2D hitCollider = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("Sink"));

            return (hitCollider != null);
        }

        private bool TouchesOutput(Vector2 pos) {
            Collider2D hitCollider = Physics2D.OverlapPoint(pos, 1 << LayerMask.NameToLayer("Output"));

            return (hitCollider != null);
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
        
        private void AssignDetails(InteractMode inMode, TransportStructure structure, bool reduces) {
            switch (inMode) {
                default:
                    break;
                case InteractMode.Default:
                    break;
                case InteractMode.Transport_Rail:
                    structure.SetDetails(RAIL_COST, BuildType.Rail, reduces);
                    break;
                case InteractMode.Transport_Highway:
                    structure.SetDetails(HIGHWAY_COST, BuildType.Highway, reduces);
                    break;
                case InteractMode.Transport_Road:
                    structure.SetDetails(ROAD_COST, BuildType.Road, reduces);
                    break;
                case InteractMode.Transport_Bridge:
                    structure.SetDetails(BRIDGE_COST, BuildType.Bridge, reduces);
                    break;
                case InteractMode.Transport_Remove:
                    break;
            }
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