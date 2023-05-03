using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using Zavala.Cards;

namespace Zavala
{
    public class SkimmerMgr : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_fleet;

        public void SetSkimmerPolicy(Severity severity) {
            int activationNum = 0;

            switch (severity) {
                case Severity.None:
                    break;
                case Severity.Low:
                    activationNum = 1;
                    break;
                case Severity.Medium:
                    activationNum = 2;
                    break;
                case Severity.High:
                    activationNum = 3;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < activationNum; i++) {
                m_fleet[i].SetActive(true);
            }
            for (int i = activationNum; i < m_fleet.Length; i++) {
                m_fleet[i].SetActive(false);
            }
        }
    }
}
