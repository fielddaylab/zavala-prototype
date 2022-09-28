using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.DataDefs;
using Zavala.Interact;

namespace Zavala.Shop.Items
{
    public enum Type
    {
        Road,
        Digester,
        Skimmer,
        Storage
    }

    public class UIShopItem : MonoBehaviour
    {
        [SerializeField] private Button m_button;

        [SerializeField] private Image m_icon;
        [SerializeField] private Text m_costText;

        private ShopItemData m_data;

        public void InitData(ShopItemData data) {
            m_data = data;
            m_icon.sprite = data.Icon;
            m_icon.SetNativeSize();
            m_costText.text = "" + data.Cost;

            m_button.onClick.AddListener(delegate { ShopMgr.Instance.SelectShopItem(m_data); });
        }

        public ShopItemData GetData() {
            return m_data;
        }
    }
}