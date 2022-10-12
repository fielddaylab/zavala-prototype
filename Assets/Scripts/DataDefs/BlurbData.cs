using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.DataDefs
{
    [CreateAssetMenu(fileName = "NewBlurbData", menuName = "Data/BlurbData")]
    public class BlurbData : ScriptableObject
    {
        // [SerializeField] private Sprite m_icon;
        [SerializeField] private string m_blurbID;
        [SerializeField] private string m_blurbText;

        [HideInInspector] public bool Seen = false;

        /*
        public Sprite Icon {
            get { return m_icon; }
        }
        */
        public string ID {
            get { return m_blurbID; }
        }

        public string Text {
            get { return m_blurbText; }
        }
    }
}
