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
        public int Units { get; set; }

        public ResourceEventArgs(Resources.Type resourceType, int units) {
            ResourceType = resourceType;
            Units = units;
        }
    }
}

namespace Zavala.Functionalities
{
    // individual products have individual production timers (currently timer length is 0)
    public class Produces : MonoBehaviour
    {
        [Serializable]
        public struct ProductBundle {
            public Resources.Type Type;
            public int Units;

            public ProductBundle(Resources.Type type, int units) {
                Type = type;
                Units = units;
            }
        }

        public List<ProductBundle> Products;

        [SerializeField] private bool m_producesMoney;
        [SerializeField] private int m_amt;

        // public event EventHandler AttemptProduce;

        public List<ProductBundle> Produce() {
            if (m_producesMoney) {
                // add money to funds
                EventMgr.Instance.TriggerEvent(Events.ID.ProduceMoney, new Events.ProduceMoneyEventArgs(m_amt, RegionMgr.Instance.GetRegionByPos(this.transform.position)));
            }

            // todo: produce type based on input type
            if (Products.Count > 0) {
                return Products;
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

        public List<Resources.Type> GetProduceTypes() {
            List<Resources.Type> types = new List<Resources.Type>();

            for (int i = 0; i < Products.Count; i++) {
                if (!types.Contains(Products[i].Type)) {
                    types.Add(Products[i].Type);
                }
            }

            return types;
        }

        #endregion // AllVars Gets & Sets
    }
}
