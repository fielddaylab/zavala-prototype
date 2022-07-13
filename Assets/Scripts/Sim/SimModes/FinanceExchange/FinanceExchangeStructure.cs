using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Exchange
{
    public enum ExchangeType
    {
        Basic,
        Digester
    }

    public struct ExchangeDetails
    {
        public float Cost;
        public ExchangeType Type;
        public float Jobs;

        public ExchangeDetails(float inCost, ExchangeType inType, float inJobs) {
            Cost = inCost;
            Type = inType;
            Jobs = inJobs;
        }
    }

    public class FinanceExchangeStructure : MonoBehaviour
    {
        private ExchangeDetails m_exchangeDetails;

        public void SetDetails(float cost, ExchangeType type, float jobs) {
            m_exchangeDetails = new ExchangeDetails(cost, type, jobs);
        }

        public ExchangeDetails GetDetails() {
            return m_exchangeDetails;
        }

        public void Build() {
            EventMgr.ExchangeBuilt?.Invoke(m_exchangeDetails);
        }

        public void Remove() {
            EventMgr.ExchangeRemoved?.Invoke(m_exchangeDetails);
            Destroy(this.gameObject);
        }
    }
}

