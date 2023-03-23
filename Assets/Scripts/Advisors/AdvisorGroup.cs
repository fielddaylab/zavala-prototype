using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{


    public class AdvisorGroup : MonoBehaviour
    {
        [Serializable]
        public struct AdvisorButtonData {
            public Sprite m_AdvisorImage;
            public AdvisorUI m_UI;
            public AudioClip Shout; // audio that plays on select
        }

        public AdvisorButtonData ButtonData;
    }
}
