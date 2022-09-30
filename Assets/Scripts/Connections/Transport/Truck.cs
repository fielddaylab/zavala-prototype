using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    public class Truck : MonoBehaviour
    {
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private float m_speed;

        private Resources.Type m_resourceType;
        private Requests m_recipient;
        private Road m_roadToFollow;

        // Road Traversal
        private int m_startRoadSegmentIndex;
        private int m_destRoadSegmentIndex;
        private int m_currRoadSegmentIndex;
        private Tile m_immediateNextDest;

        private float m_yBuffer;

        public void Init(Resources.Type resourceType, StoresProduct supplier, Requests recipient, Road road) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_yBuffer = this.gameObject.transform.localPosition.y;

            m_resourceType = resourceType;
            m_recipient = recipient;
            m_roadToFollow = road;
            m_startRoadSegmentIndex = m_currRoadSegmentIndex = m_roadToFollow.GetStartIndex(supplier);
            m_destRoadSegmentIndex = m_roadToFollow.GetEndIndex(recipient);
            m_immediateNextDest = m_roadToFollow.GetTileAtIndex(m_startRoadSegmentIndex);
            this.transform.position = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);
        }

        private void Update() {
            TraverseRoad();
        }

        private void TraverseRoad() {
            Vector3 nextDestPos = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);
            if (Vector3.Distance(this.transform.position, nextDestPos) > .1f) {
                // move to immediate next dest
                Debug.Log("[Truck] Moving towards next destination");
                this.transform.Translate((nextDestPos - this.transform.position).normalized * m_speed * Time.deltaTime);
            }
            else {
                // update immediate next dest
                if (m_currRoadSegmentIndex == m_destRoadSegmentIndex) {
                    Debug.Log("[Truck] Arrived at final destination");

                    Deliver();
                }
                else {
                    if (m_currRoadSegmentIndex < m_destRoadSegmentIndex) {
                        m_currRoadSegmentIndex++;
                    }
                    else {
                        m_currRoadSegmentIndex--;
                    }
                    m_immediateNextDest = m_roadToFollow.GetTileAtIndex(m_currRoadSegmentIndex);

                    Debug.Log("[Truck] Updated immediate next destination");
                }
            }
        }

        private void Deliver() {
            m_recipient.ReceiveRequestedProduct(m_resourceType);

            // remove truck
            Destroy(this.gameObject);
        }
    }
}
