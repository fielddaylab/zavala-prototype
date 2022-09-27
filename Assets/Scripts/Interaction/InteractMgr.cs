
using UnityEngine;
using UnityEngine.UI;

// PLACEHOLDER from last iteration. Currently non-functional

namespace Zavala.Interact
{
    public enum Mode
    {
        Default,
        Transport_Rail,
        Transport_Highway,
        Transport_Road,
        Transport_Bridge,
        Transport_Remove,
        Finance_Exchange_Basic,
        Finance_Exchange_Digester,
        Finance_Exchange_Remove,
        Politics_Campaign_Stop,
        Politics_Campaign_Video,
        Politics_Campaign_Remove,
    }

    public class InteractMgr : MonoBehaviour
    {
        [Header("Shared")]

        [SerializeField] private Texture2D m_removeCursor;

        private Texture2D m_defaultCursor = null;
        private Interact.Mode m_interactMode;


        [Header("Transport")]

        [SerializeField] private Texture2D m_drawCursor;
        [SerializeField] private Texture2D m_bridgeCursor;

        [SerializeField] private Sprite m_railIcon;
        [SerializeField] private Sprite m_highwayIcon;
        [SerializeField] private Sprite m_roadIcon;

        [SerializeField] private GameObject m_linePrefab;
        [SerializeField] private GameObject m_bridgePrefab;
        [SerializeField] private GameObject m_linesContainer;
        [SerializeField] private GameObject m_bridgesContainer;
        private Vector2 m_startDrawPos;
        private Vector2 m_endDrawPos;

        private static Vector2 UNASSIGNED_V2 = new Vector2(-9999, -9999);

        private static float RAIL_COST = 20;
        private static float HIGHWAY_COST = 20;
        private static float ROAD_COST = 20;
        private static float BRIDGE_COST = 20;


        [Header("Exchange")]

        [SerializeField] private Texture2D m_exchangeBasicCursor;
        [SerializeField] private Texture2D m_exchangeDigestCursor;

        [SerializeField] private GameObject m_exchangeBasicPrefab, m_exchangeDigestPrefab;
        [SerializeField] private GameObject m_exchangeContainer;

        private static float EXCHANGE_BASIC_COST = 20;
        private static float EXCHANGE_DIGEST_COST = 30;
        private static float EXCHANGE_BASIC_JOBS = 10;
        private static float EXCHANGE_DIGEST_JOBS = 12;


        [Header("Policy")]

        [SerializeField] private Texture2D m_stopCursor;
        [SerializeField] private Texture2D m_videoCursor;

        [SerializeField] private GameObject m_stopPrefab, m_videoPrefab;
        [SerializeField] private GameObject m_campaigningsContainer;


        #region Callbacks

        public void Init() {

        }

        private void Awake() {
            //EventMgr.InteractModeUpdated.AddListener(HandleInteractModeUpdated);
            //EventMgr.SimCanvasSubmitted.AddListener(HandleSimCanvasSubmitted);

            m_startDrawPos = m_endDrawPos = UNASSIGNED_V2;
        }

        private void OnDestroy() {
            // EventMgr.InteractModeUpdated.RemoveListener(HandleInteractModeUpdated);
        }

        private void Update() {
            bool drawing = (m_interactMode == Interact.Mode.Transport_Rail)
                || (m_interactMode == Interact.Mode.Transport_Highway)
                || (m_interactMode == Interact.Mode.Transport_Road);

            if (drawing) {
                if (!m_startDrawPos.Equals(UNASSIGNED_V2)) {
                    // draw line from start to end point
                    // Stretch(m_currLine.Image.gameObject, m_startDrawPos, Input.mousePosition, true);
                }

                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // check whether starting a new line or completing an existing one
                    if (m_startDrawPos.Equals(UNASSIGNED_V2)) {
                        // starting a new line
                        m_startDrawPos = Input.mousePosition;

                        //m_currLine = Instantiate(m_linePrefab, m_linesContainer.transform).GetComponent<TransportLine>();
                        //m_currLine.transform.localPosition = m_startDrawPos;
                    }
                    else {
                        // completing a line
                        m_endDrawPos = Input.mousePosition;

                        // check if touches a farm and a sink (if so, reduce algal outbreak)
                        bool reducesOutbreaks = (TouchesOutput(m_startDrawPos) && TouchesSink(m_endDrawPos))
                            || (TouchesSink(m_startDrawPos) && TouchesOutput(m_endDrawPos));

                        //AssignBuildDetails(m_interactMode, m_currLine.GetComponent<TransportStructure>(), reducesOutbreaks);
                        //m_currLine.GetComponent<TransportStructure>().Build();

                        m_startDrawPos = m_endDrawPos = UNASSIGNED_V2;
                    }
                }
            }

            if (m_interactMode == Interact.Mode.Transport_Bridge) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // build a bridge
                    var bridge = Instantiate(m_bridgePrefab, m_bridgesContainer.transform);
                    bridge.transform.localPosition = Input.mousePosition;

                    //AssignBuildDetails(m_interactMode, bridge.GetComponent<TransportStructure>(), false);
                    //bridge.GetComponent<TransportStructure>().Build();
                }
            }

            if (m_interactMode == Interact.Mode.Transport_Remove) {
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D removableCollider = OverlappingStructure(Input.mousePosition);

                    if (removableCollider != null) {
                        //removableCollider.GetComponent<TransportStructure>().Remove();
                    }
                }
            }

            if (m_interactMode == Interact.Mode.Finance_Exchange_Basic) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // build an exchange
                    var exchange = Instantiate(m_exchangeBasicPrefab, m_exchangeContainer.transform);
                    exchange.transform.localPosition = Input.mousePosition;

                    //AssignExchangeDetails(m_interactMode, exchange.GetComponent<FinanceExchangeStructure>());
                    //exchange.GetComponent<FinanceExchangeStructure>().Build();
                }
            }

            if (m_interactMode == Interact.Mode.Finance_Exchange_Digester) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // build an exchange digester
                    var exchange = Instantiate(m_exchangeDigestPrefab, m_exchangeContainer.transform);
                    exchange.transform.localPosition = Input.mousePosition;

                    //AssignExchangeDetails(m_interactMode, exchange.GetComponent<FinanceExchangeStructure>());
                    //exchange.GetComponent<FinanceExchangeStructure>().Build();
                }
            }

            if (m_interactMode == Interact.Mode.Finance_Exchange_Remove) {
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D removableCollider = OverlappingStructure(Input.mousePosition);

                    if (removableCollider != null) {
                        //removableCollider.GetComponent<FinanceExchangeStructure>().Remove();
                    }
                }
            }


            if (m_interactMode == Interact.Mode.Politics_Campaign_Stop) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // plan a stop
                    var stop = Instantiate(m_stopPrefab, m_campaigningsContainer.transform);
                    stop.transform.localPosition = Input.mousePosition;

                    //AssignStratDetails(m_interactMode, stop.GetComponent<PoliticsCampaignStrategy>());
                    //stop.GetComponent<PoliticsCampaignStrategy>().Build();
                }
            }

            if (m_interactMode == Interact.Mode.Politics_Campaign_Video) {
                if (Input.GetMouseButtonDown(0) && OnMap(Input.mousePosition)) {
                    // run ad campaign
                    var video = Instantiate(m_videoPrefab, m_campaigningsContainer.transform);
                    video.transform.localPosition = Input.mousePosition;

                    //AssignStratDetails(m_interactMode, video.GetComponent<PoliticsCampaignStrategy>());
                    //video.GetComponent<PoliticsCampaignStrategy>().Build();
                }
            }

            if (m_interactMode == Interact.Mode.Politics_Campaign_Remove) {
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D removableCollider = OverlappingStructure(Input.mousePosition);

                    if (removableCollider != null) {
                        //removableCollider.GetComponent<PoliticsCampaignStrategy>().Remove();
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

        private void UpdateCursor(Interact.Mode inMode) {
            Texture2D newCursor = m_defaultCursor;
            Vector3 offset = Vector3.zero;

            switch (inMode) {
                default:
                    break;
                case Interact.Mode.Default:
                    break;
                case Interact.Mode.Transport_Rail:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_railIcon;
                    break;
                case Interact.Mode.Transport_Highway:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_highwayIcon;
                    break;
                case Interact.Mode.Transport_Road:
                    newCursor = m_drawCursor;
                    offset = new Vector2(0, newCursor.height);
                    m_linePrefab.GetComponent<Image>().sprite = m_roadIcon;
                    break;
                case Interact.Mode.Transport_Bridge:
                    newCursor = m_bridgeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.Transport_Remove:
                    newCursor = m_removeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                case Interact.Mode.Finance_Exchange_Basic:
                    newCursor = m_exchangeBasicCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.Finance_Exchange_Digester:
                    newCursor = m_exchangeDigestCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.Finance_Exchange_Remove:
                    newCursor = m_removeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
                case Interact.Mode.Politics_Campaign_Stop:
                    newCursor = m_stopCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.Politics_Campaign_Video:
                    newCursor = m_videoCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.Politics_Campaign_Remove:
                    newCursor = m_removeCursor;
                    offset = new Vector2(newCursor.width / 2, newCursor.height / 2);
                    break;
            }

            Cursor.SetCursor(newCursor, offset, CursorMode.ForceSoftware);
        }

        /*
        private void AssignBuildDetails(Interact.Mode inMode, TransportStructure structure, bool reduces) {
            switch (inMode) {
                default:
                    break;
                case Interact.Mode.Default:
                    break;
                case Interact.Mode.Transport_Rail:
                    structure.SetDetails(RAIL_COST, BuildType.Rail, reduces);
                    break;
                case Interact.Mode.Transport_Highway:
                    structure.SetDetails(HIGHWAY_COST, BuildType.Highway, reduces);
                    break;
                case Interact.Mode.Transport_Road:
                    structure.SetDetails(ROAD_COST, BuildType.Road, reduces);
                    break;
                case Interact.Mode.Transport_Bridge:
                    structure.SetDetails(BRIDGE_COST, BuildType.Bridge, reduces);
                    break;
                case Interact.Mode.Transport_Remove:
                    break;
            }
        }

        private void AssignExchangeDetails(Interact.Mode inMode, FinanceExchangeStructure exchangeStructure) {
            switch (inMode) {
                default:
                    break;
                case Interact.Mode.Finance_Exchange_Basic:
                    exchangeStructure.SetDetails(EXCHANGE_BASIC_COST, ExchangeType.Basic, EXCHANGE_BASIC_JOBS);
                    break;
                case Interact.Mode.Finance_Exchange_Digester:
                    exchangeStructure.SetDetails(EXCHANGE_DIGEST_COST, ExchangeType.Digester, EXCHANGE_DIGEST_JOBS);
                    break;
                case Interact.Mode.Finance_Exchange_Remove:
                    break;
            }
        }

        private void AssignStratDetails(Interact.Mode inMode, PoliticsCampaignStrategy strat) {
            switch (inMode) {
                default:
                    break;
                case Interact.Mode.Politics_Campaign_Stop:
                    strat.SetDetails(StratType.Stop);
                    break;
                case Interact.Mode.Politics_Campaign_Video:
                    strat.SetDetails(StratType.Video);
                    break;
                case Interact.Mode.Politics_Campaign_Remove:
                    break;
            }
        }
        */

        #region Event Handlers

        private void HandleInteractModeUpdated(Interact.Mode newMode) {
            m_interactMode = newMode;

            UpdateCursor(newMode);
        }

        private void HandleSimCanvasSubmitted() {
            m_interactMode = Interact.Mode.Default;

            UpdateCursor(m_interactMode);
        }

        #endregion // Event Handlers
    }
}