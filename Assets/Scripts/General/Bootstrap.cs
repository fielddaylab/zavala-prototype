using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Interact;

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
            m_playerMgr.Init();
            m_interactMgr.Init();
        }
    }
}