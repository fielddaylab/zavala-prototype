using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;

namespace Zavala
{
    public class TransportUI : SimModeUI
    {
        public override void Open() {
            InitIndicatorVals();

            EventMgr.InteractModeUpdated?.Invoke(InteractMode.Default);
        }

        private void InitIndicatorVals() {
            // TODO: this
        }
    }
}