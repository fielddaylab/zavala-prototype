using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zavala.Interact;

namespace Zavala
{
    public class EventMgr : MonoBehaviour
    {
        #region Interact Events

        public class InteractEvent : UnityEvent<InteractMode> { }
        public static InteractEvent InteractModeUpdated = new InteractEvent();

        #endregion // Interact Events

        #region Indicator Events

        public static UnityEvent IndicatorUpdated = new UnityEvent();

        public class IndicatorEvent : UnityEvent<IndicatorData[]> { }
        public static IndicatorEvent SetNewIndicators = new IndicatorEvent();

        #endregion // Indicator Events

        #region Canvas Events

        public static UnityEvent SimCanvasSubmitted = new UnityEvent();
        public static UnityEvent ResultsCanvasOkayed = new UnityEvent();

        #endregion // Canvas Events

        private void Awake() {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}