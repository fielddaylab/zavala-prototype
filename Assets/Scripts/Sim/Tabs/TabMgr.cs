using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class TabMgr : MonoBehaviour
    {
        [SerializeField] private SimTab[] m_simTabs;
        [SerializeField] private SimButton[] m_simButtons;
        [SerializeField] private Sprite m_defaultLockIcon;

        [SerializeField] private Button m_drawerButton;
        private bool m_drawerOpen;
        private static float DRAWER_OPEN_POS = -350f;
        private static float DRAWER_CLOSE_POS = -485f;

        private void Start() {
            InitTabs();

            // open first tab
            m_simTabs[0].GetComponent<Button>().onClick.Invoke();

            m_drawerOpen = true;
            m_drawerButton.onClick.AddListener(ToggleDrawer);
        }


        private void InitTabs() {
            Debug.Log("[TabMgr] Initializing tabs");
            foreach (SimTab tab in m_simTabs) {
                Button tabButton = tab.GetComponent<Button>();
                tabButton.onClick.AddListener(delegate { LoadSimButtons(tab.GetSimButtonData()); });
            }
        }

        private void LoadSimButtons(SimButtonData[] simButtonData) {

            int buttonIndex = 0;

            bool isUnlocked = true; // TODO: centralize this and make unique for each mode

            foreach (SimButtonData buttonData in simButtonData) {
                m_simButtons[buttonIndex].Icon.sprite = isUnlocked ? buttonData.Sprite : m_defaultLockIcon;
                m_simButtons[buttonIndex].SetCurrData(buttonData);
                m_simButtons[buttonIndex].gameObject.SetActive(true);

                buttonIndex++;
            }

            while (buttonIndex < m_simButtons.Length) {
                m_simButtons[buttonIndex].gameObject.SetActive(false);
                buttonIndex++;
            }
        }

        private void ToggleDrawer() {
            m_drawerOpen = !m_drawerOpen;
            this.gameObject.SetActive(m_drawerOpen);

            Vector3 currScale = m_drawerButton.GetComponent<RectTransform>().localScale;
            m_drawerButton.GetComponent<RectTransform>().localScale = new Vector3(-currScale.x, currScale.y, currScale.z);

            Vector3 currPos = m_drawerButton.GetComponent<RectTransform>().localPosition;
            float newX = m_drawerOpen ? DRAWER_OPEN_POS : DRAWER_CLOSE_POS;
            m_drawerButton.GetComponent<RectTransform>().localPosition = new Vector3(newX, currPos.y, currPos.z);
        }
    }
}