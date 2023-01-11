using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.DataDefs;
using Zavala.Events;
using Zavala.Interact;
using Zavala.Shop.Items;

namespace Zavala
{
    public class ShopMgr : MonoBehaviour
    {
        public static ShopMgr Instance;

        [SerializeField] private Button m_toggleButton;
        [SerializeField] private Text m_moneyText;
        [SerializeField] private UIShopItem[] m_itemUIs;
        [SerializeField] private GameObject m_itemsContainer;

        private Dictionary<Shop.Items.Type, ShopItemData> m_shopItemMap;

        public ShopItemData[] ShopItems;

        private ShopItemData m_selectedItem;


        public void Init() {
            Instance = this;

            EventMgr.Instance.RegionUpdatedMoney += HandleRegionUpdatedMoney;
            EventMgr.Instance.InteractModeUpdated += HandleInteractModeUpdated;
            EventMgr.Instance.RegionSwitched += HandleRegionSwitched;

            if (m_itemUIs.Length != ShopItems.Length) {
                Debug.Log("[ShopMgr] Unequal number of data defs and ui slots!");
            }
            else {
                for (int i = 0; i < ShopItems.Length; i++) {
                    m_itemUIs[i].InitData(ShopItems[i]);
                }
            }

            m_toggleButton.onClick.AddListener(ToggleShopExpand);
            m_itemsContainer.SetActive(false);

            Debug.Log("[DebugNulls] Selected item set to null in init");
            m_selectedItem = null;
        }

        public bool TryPurchaseSelection() {
            return TryPurchaseHelper(m_selectedItem.Cost);
        }

        public bool TryPurchaseRoad(int roadLength) {
            return TryPurchaseHelper(m_selectedItem.Cost * roadLength);
        }

        public GameObject GetPurchasePrefab() {
            Debug.Log("[DebugNulls] Getting purchase prefab...");
            Debug.Log("[DebugNulls] selected item is null?: " + (m_selectedItem == null));
            Debug.Log("[DebugNulls] Selected item prefab is null?: " + (m_selectedItem.Prefab == null));
            return m_selectedItem.Prefab;
        }

        public bool TryPurchaseImport(int cost) {
            return TryPurchaseHelper(cost);
        }

        private bool TryPurchaseHelper(int purchaseCost) {
            Debug.Log("[ShopMgr] TryPuchaseHelper begin");
            if (purchaseCost <= RegionMgr.Instance.CurrRegion.GetMoney()) {
                // player has enough money
                EventMgr.Instance.TriggerEvent(Events.ID.PurchaseSuccessful, new PurchaseSuccessfulEventArgs(purchaseCost, RegionMgr.Instance.CurrRegion));
                UpdateText();
                return true;
            }
            else {
                // purchase failure
                return false;
            }
        }


        private void UpdateText() {
            m_moneyText.text = "" + RegionMgr.Instance.CurrRegion.GetMoney();
        }

        public static ShopItemData GetShopItemData(Shop.Items.Type itemType) {
            // initialize the map if it does not exist
            if (Instance.m_shopItemMap == null) {
                Instance.m_shopItemMap = new Dictionary<Shop.Items.Type, ShopItemData>();
                foreach (ShopItemData data in Instance.ShopItems) {
                    Instance.m_shopItemMap.Add(data.ItemType, data);
                }
            }
            if (Instance.m_shopItemMap.ContainsKey(itemType)) {
                return Instance.m_shopItemMap[itemType];
            }
            else {
                throw new KeyNotFoundException(string.Format("No Shop Item " +
                    "with id `{0}' is in the database", itemType
                ));
            }
        }

        private void ToggleShopExpand() {
            m_itemsContainer.SetActive(!m_itemsContainer.activeSelf);
        }

        public void SelectShopItem(ShopItemData data) {
            Debug.Log("Selected " + data.ItemType.ToString());

            m_selectedItem = data;
            Debug.Log("[DebugNulls] New selected item set (" + data.ItemType.ToString() + "). Prefab is null?: " + (data.Prefab == null));

            EventMgr.Instance.TriggerEvent(Events.ID.InteractModeUpdated, new InteractModeEventArgs(GetInteractMode(data.ItemType)));
        }

        #region Handlers

        private void HandleRegionUpdatedMoney(object sender, RegionUpdatedMoneyEventArgs args) {
            if (args.Region != RegionMgr.Instance.CurrRegion) { return; }
            UpdateText();
        }

        private void HandleInteractModeUpdated(object sender, InteractModeEventArgs args) {
            if (args.Mode == Mode.Select) {
                Debug.Log("[DebugNulls] Selected item set to null when interact mode switched to Select");
                m_selectedItem = null;
            }
        }

        private void HandleRegionSwitched(object sender, RegionSwitchedEventArgs args) {
            UpdateText();
        }

        #endregion // Handlers

        public static Interact.Mode GetInteractMode(Shop.Items.Type itemType) {
            switch (itemType) {
                case Shop.Items.Type.Road:
                    return Interact.Mode.DrawRoad;
                case Shop.Items.Type.Digester:
                    return Interact.Mode.PlaceItem;
                case Shop.Items.Type.Skimmer:
                    return Interact.Mode.PlaceItem;
                case Shop.Items.Type.Storage:
                    return Interact.Mode.PlaceItem;
                default:
                    return Interact.Mode.Select;
            }
        }
    }
}
