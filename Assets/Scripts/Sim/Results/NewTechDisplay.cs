using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Zavala
{
    public class NewTechDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_text;

        public TMP_Text NewTechText {
            get { return m_text; }
        }
    }
}
