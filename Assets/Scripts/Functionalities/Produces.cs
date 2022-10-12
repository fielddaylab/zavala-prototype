using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Zavala.Events;

namespace Zavala.Resources
{
    public enum Type
    {
        None,
        Fertilizer,
        Grain,
        Milk,
        Manure,
        SoilEnricher
    }

    public class ResourceEventArgs : EventArgs
    {
        public Resources.Type ResourceType { get; set; }

        public ResourceEventArgs(Resources.Type resourceType) {
            ResourceType = resourceType;
        }
    }
}

namespace Zavala.Functionalities
{
    // individual products have individual production timers (currently timer length is 0)
    public class Produces : MonoBehaviour
    {
        public List<Resources.Type> ProduceTypes;

        [SerializeField] private bool m_producesMoney;
        [SerializeField] private int m_amt;

        // public event EventHandler AttemptProduce;

        public List<Resources.Type> Produce() {
            if (m_producesMoney) {
                // add money to funds
                EventMgr.Instance.TriggerEvent(Events.ID.ProduceMoney, new Events.ProduceMoneyEventArgs(m_amt));
            }

            // todo: produce type based on input type
            if (ProduceTypes.Count > 0) {
                return ProduceTypes;
            }
            else {
                return null;
            }
        }

        #region AllVars Gets & Sets

        public void SetProduceAmt(int newVal) {
            m_amt = newVal;
        }

        public int GetProduceAmt() {
            return m_amt;
        }

        #endregion // AllVars Gets & Sets
    }
}
