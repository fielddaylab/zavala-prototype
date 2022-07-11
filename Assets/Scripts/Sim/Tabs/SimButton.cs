using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    [RequireComponent(typeof(Button))]
    public class SimButton : MonoBehaviour
    {
        [SerializeField] private Image m_icon;

        private bool m_isUnlocked;
        private SimButtonData m_currSimButtonData;


        public Image Icon {
            get { return m_icon; }
            set { m_icon = value; }
        }
        public void SetCurrData(SimButtonData data) {
            m_currSimButtonData = data;

            this.GetComponent<Button>().onClick.RemoveAllListeners();
            this.GetComponent<Button>().onClick.AddListener(HandleClick);
        }

        private void HandleClick() {
            EventMgr.SetNewMode?.Invoke(m_currSimButtonData.ModeData);
        }
    }
}