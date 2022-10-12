using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zavala
{
    public class NarrativeBlurb : MonoBehaviour
    {
        public TMP_Text Text;
        public Button OkayButton;
        public Image HeadImg;

        public void Init(string text) {
            Text.text = text;
        }
    }
}
