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


        public void Init() {
            Instance = this;

            EventMgr.Instance.PlayerReceivedMoney += HandlePlayerReceivedMoney;

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
        }

        public bool TryPurchase(ShopItemData itemData) {
            // reduce m_moneyUnits

            UpdateText();

            return true;
        }


        private void UpdateText() {
            m_moneyText.text = "" + PlayerMgr.Instance.GetMoney();
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

            EventMgr.Instance.TriggerEvent(Events.ID.InteractModeUpdated, new InteractModeEventArgs(GetInteractMode(data.ItemType)));
        }

        #region Handlers

        private void HandlePlayerReceivedMoney(object sender, EventArgs args) {
            UpdateText();
        }

        #endregion // Handlers

        public static Interact.Mode GetInteractMode(Shop.Items.Type itemType) {
            switch (itemType) {
                case Shop.Items.Type.Road:
                    return Interact.Mode.PlaceRoad;
                    break;
                case Shop.Items.Type.Digester:
                    return Interact.Mode.PlaceDigester;
                    break;
                case Shop.Items.Type.Skimmer:
                    return Interact.Mode.PlaceSkimmer;
                    break;
                case Shop.Items.Type.Storage:
                    return Interact.Mode.PlaceStorage;
                    break;
                default:
                    return Interact.Mode.DefaultSelect;
                    break;
            }
        }
    }
}
