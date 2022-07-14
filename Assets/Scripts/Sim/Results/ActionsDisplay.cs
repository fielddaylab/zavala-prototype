using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zavala.Sim;

namespace Zavala
{
    public class ActionsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_actionText;
        [SerializeField] private TMP_Text m_interpretText;

        public TMP_Text ActionText {
            get { return m_actionText; }
        }
        public TMP_Text InterpretText {
            get { return m_interpretText; }
        }
    }
}