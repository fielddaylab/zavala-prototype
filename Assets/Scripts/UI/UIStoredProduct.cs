using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class UIStoredProduct : MonoBehaviour
    {
        [SerializeField] private Image m_resourceIcon;

        public void Init(Resources.Type resourceType) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();
        }
    }
}
