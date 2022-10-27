using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Events;

namespace Zavala
{
    public class LevelActivationUI : MonoBehaviour
    {
        [SerializeField] private Button[] m_levelButtons;

        public void Init() {
            for (int b = 0; b < m_levelButtons.Length; b++) {
                int levelIndex = b;
                m_levelButtons[b].onClick.AddListener(delegate {
                    EventMgr.Instance.TriggerEvent(ID.RegionToggled, new RegionToggleEventArgs(levelIndex));
                    m_levelButtons[levelIndex].interactable = false;
                });
            }

            // deactivate region 0 button
            m_levelButtons[0].interactable = false;
        }
    }
}
