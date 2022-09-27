using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Zavala.Resources
{
    public enum Type
    {
        None,
        Fertilizer,
        Grain,
        Milk,
        Manure
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

        public event EventHandler AttemptProduce;

        public List<Resources.Type> Produce() {
            if (m_producesMoney) {
                // add money to funds
                Debug.Log("Added $" + m_amt + " to your funds!");
                return null;
            }

            // todo: produce type based on input type
            if (ProduceTypes.Count > 0) {
                return ProduceTypes;
            }
            else {
                return null;
            }
        }
    }
}
