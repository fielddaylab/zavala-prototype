using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Interact;

namespace Zavala
{
    public class TransportUI : SimModeUI
    {

        private static int PRIVATE_SPENDING_INDEX = 0;
        private static int GOVT_SPENDING_INDEX = 1;
        private static int OUTBREAK_INDEX = 2;


        public override void Open() {
            InitIndicatorVals();

            EventMgr.InteractModeUpdated?.Invoke(InteractMode.Default);
        }

        private void InitIndicatorVals() {
            IndicatorMgr.Instance.SetIndicatorValue(PRIVATE_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(GOVT_SPENDING_INDEX, 0);
            IndicatorMgr.Instance.SetIndicatorValue(OUTBREAK_INDEX, 0.9f);
        }
    }
}