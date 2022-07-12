using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    [RequireComponent(typeof(Image))]
    public class TransportLine : MonoBehaviour
    {
        private Image m_image;

        public Image Image {
            get { return m_image; }
        }

        private void Awake() {
            m_image = GetComponent<Image>();
        }
    }
}
