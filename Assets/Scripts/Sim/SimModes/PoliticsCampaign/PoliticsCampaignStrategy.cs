using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala.Strategy
{
    public enum StratType
    {
        Stop,
        Video
    }

    public struct StratDetails
    {
        //public float Cost;
        public StratType Type;
        public VotingDistrict District;

        public StratDetails(StratType inType, VotingDistrict inDistrict) {
            Type = inType;
            District = inDistrict;
        }
    }

    public class PoliticsCampaignStrategy : MonoBehaviour
    {
        private StratDetails m_stratDetails;

        public void SetDetails(StratType type) {
            m_stratDetails = new StratDetails(type, DetectDistrictOverlapping());
        }

        public StratDetails GetDetails() {
            return m_stratDetails;
        }

        public void Build() {
            EventMgr.StratDeployed?.Invoke(m_stratDetails);
        }

        public void Remove() {
            EventMgr.StratRemoved?.Invoke(m_stratDetails);
            Destroy(this.gameObject);
        }

        private VotingDistrict DetectDistrictOverlapping() {
            Collider2D hitCollider = Physics2D.OverlapPoint(transform.localPosition, 1 << LayerMask.NameToLayer("District"));

            if (hitCollider != null) {
                return hitCollider.GetComponent<VotingDistrict>();
            }
            else {
                Debug.Log("Warning! Placed a campaign stop / ad where there is no district");
                return null;
            }
        }
    }
}

