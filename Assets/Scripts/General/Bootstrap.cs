using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Cards;
using Zavala.Events;
using Zavala.Interact;
using Zavala.Lenses;
using Zavala.Roads;
using Zavala.Settings;
using Zavala.Tiles;

namespace Zavala
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance;

        [SerializeField] private EventMgr m_eventMgr;
        [SerializeField] private GameDB m_gameDB;
        [SerializeField] private PlayerMgr m_playerMgr;
        [SerializeField] private ShopMgr m_shopMgr;
        [SerializeField] private InteractMgr m_interactMgr;
        [SerializeField] private RoadMgr m_roadMgr;
        [SerializeField] private RegionMgr m_regionMgr;
        [SerializeField] private PhosphMgr m_phosphMgr;
        [SerializeField] private LensMgr m_lensMgr;
        [SerializeField] private SettingsMgr m_settingsMgr;
        [SerializeField] private NarrativeMgr m_narrativeMgr;
        [SerializeField] private TileGenerator m_tileGenerator;
        [SerializeField] private LevelMgr m_levelMgr;
        [SerializeField] private CardMgr m_cardMgr;
        [SerializeField] private AdvisorUIMgr m_advisorUIMgr;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
                return;
            }

            m_eventMgr.Init();
            m_gameDB.Init();
            m_shopMgr.Init();
            // m_playerMgr.Init();
            m_interactMgr.Init();
            m_roadMgr.Init();
            m_regionMgr.Init();
            //GridMgr.Init();
            m_phosphMgr.Init();
            m_lensMgr.Init();
            m_settingsMgr.Init();
            m_narrativeMgr.Init();
            m_tileGenerator.Init();
            m_levelMgr.Init();
            m_cardMgr.Init();
            m_advisorUIMgr.Init();
        }

        private void Update() {
            if (Time.timeScale == 0) { return; }
            m_phosphMgr.SimulateRunoff();
        }
    }
}