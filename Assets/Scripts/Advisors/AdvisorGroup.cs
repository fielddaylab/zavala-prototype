using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala.Advisors
{
    public class AdvisorGroup : MonoBehaviour
    {
        [Serializable]
        public struct AdvisorButtonData {
            public Sprite m_AdvisorImage;
            public AdvisorID m_AdvisorID;
            public AudioClip Shout; // audio that plays on select
            public Color BaseColor;
            public Color OutlineColor;
        }

        public AdvisorButtonData ButtonData;
    }
}
