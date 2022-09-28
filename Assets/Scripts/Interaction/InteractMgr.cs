
using UnityEngine;
using UnityEngine.UI;

// PLACEHOLDER from last iteration. Currently non-functional

namespace Zavala.Interact
{
    public enum Mode
    {
        DefaultSelect,
        PlaceRoad,
        PlaceDigester,
        PlaceSkimmer,
        PlaceStorage,
        PhosphorousSelect
    }

    public class InteractMgr : MonoBehaviour
    {
        private Interact.Mode m_interactMode;

        private Texture2D m_defaultCursor = null;
        [SerializeField] private Texture2D m_placeCursor;
        // [SerializeField] private Texture2D m_removeCursor;


        #region Callbacks

        public void Init() {

        }

        private void Awake() {
            //EventMgr.InteractModeUpdated.AddListener(HandleInteractModeUpdated);
        }

        private void OnDestroy() {
            // EventMgr.InteractModeUpdated.RemoveListener(HandleInteractModeUpdated);
        }

        private void Update() {

            /*
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
            */
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
                case Interact.Mode.DefaultSelect:
                    break;
                case Interact.Mode.PlaceRoad:
                    newCursor = m_placeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.PlaceDigester:
                    newCursor = m_placeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.PlaceSkimmer:
                    newCursor = m_placeCursor;
                    offset = new Vector2(0, newCursor.height);
                    break;
                case Interact.Mode.PhosphorousSelect:
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

        #endregion // Event Handlers
    }
}