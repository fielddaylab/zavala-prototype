using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    [CreateAssetMenu(fileName = "NewSimButtonData", menuName = "Data/SimButtonData")]
    public class SimButtonData : ScriptableObject
    {
        [SerializeField] private Sprite m_sprite;
        [SerializeField] private SimModeData m_modeData;

        public Sprite Sprite {
            get { return m_sprite; }
        }
        public SimModeData ModeData {
            get { return m_modeData; }
        }
    }
}
