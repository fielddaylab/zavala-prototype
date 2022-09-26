using System;
using System.Collections;
using System.Collections.Generic;
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

        public event EventHandler AttemptProduce;
    }
}
