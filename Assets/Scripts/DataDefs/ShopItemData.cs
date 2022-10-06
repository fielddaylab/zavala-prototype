using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.DataDefs
{
    [CreateAssetMenu(fileName = "NewShopItemData", menuName = "Data/ShopItemData")]
    public class ShopItemData : ScriptableObject
    {
        [SerializeField] private Shop.Items.Type m_itemType;
        [SerializeField] private int m_cost;
        [SerializeField] private Sprite m_icon;
        [SerializeField] private GameObject m_prefab;
        [SerializeField] private string m_label;

        public Shop.Items.Type ItemType {
            get { return m_itemType; }
        }
        public int Cost {
            get { return m_cost; }
        }
        public Sprite Icon {
            get { return m_icon; }
        }
        public GameObject Prefab {
            get { return m_prefab; }
        }
        public string Label {
            get { return m_label; }
        }
    }
}
