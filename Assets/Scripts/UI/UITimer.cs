using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;

namespace Zavala
{
    public class UITimer : MonoBehaviour
    {
        public Image Image;

        private float m_totalTime;
        private float m_currTime; // how much time is left in the cycle

        public event EventHandler TimerCompleted; // when the timer completes

        public void Init(float totalTime, bool visible) {
            Image.enabled = visible;
            m_totalTime = m_currTime = totalTime;
        }

        private void Update() {
            Tick();
        }

        public void Tick() {
            m_currTime -= Time.deltaTime;
            if (m_currTime < 0) {
                m_currTime = 0;
                UpdateProgress();

                CloseTimer();
            }
            else {
                UpdateProgress();
            }
        }

        private void UpdateProgress() {
            Image.fillAmount = m_currTime / m_totalTime;
        }

        private void CloseTimer() {
            TimerCompleted.Invoke(this, EventArgs.Empty);
        }
    }
}