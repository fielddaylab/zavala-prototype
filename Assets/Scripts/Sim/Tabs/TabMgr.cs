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

        private void Start() {
            InitTabs();

            // open first tab
            m_simTabs[0].GetComponent<Button>().onClick.Invoke();
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
    }
}