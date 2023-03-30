using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Zavala.Cards;
using Zavala.Sim;

namespace Zavala
{
    public class SlotClickable : MonoBehaviour
    {
        public void HandleClick(SimLeverID slotType) {
            // query cardmgr for relevant cards for this slot type
            List<CardData> availableCards = CardMgr.Instance.GetOptions(slotType);

            // TODO: filter out current selection

            Debug.Log("[SlotCard] Available cards: " + JsonConvert.SerializeObject(availableCards));
        }
    }
}