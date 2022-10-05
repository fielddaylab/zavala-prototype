using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zavala.Functionalities;
using Zavala.Tiles;

namespace Zavala
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(GeneratesPhosphorus))]
    public class Truck : MonoBehaviour
    {
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private Image m_resourceIcon;
        [SerializeField] private float m_speed;
        [SerializeField] private float m_leakRate;

        private Resources.Type m_resourceType;
        private Requests m_recipient;
        private Road m_roadToFollow;

        // Road Traversal
        private int m_startRoadSegmentIndex;
        private int m_destRoadSegmentIndex;
        private int m_currRoadSegmentIndex;
        private Tile m_immediateNextDest;

        private float m_yBuffer;

        private bool m_delivered;

        private GeneratesPhosphorus m_generatesComponent;

        // Audio
        private AudioSource m_audioSource;

        [SerializeField] private AudioClip m_engineStartClip;
        [SerializeField] private AudioClip m_engineContinueClip;
        [SerializeField] private AudioClip m_engineEndClip;

        private enum EngineState {
            Start,
            Continue,
            End
        }

        private EngineState m_engineState;

        public void Init(Resources.Type resourceType, StoresProduct supplier, Requests recipient, Road road) {
            m_resourceIcon.sprite = GameDB.Instance.GetResourceIcon(resourceType);
            m_resourceIcon.SetNativeSize();

            m_yBuffer = this.gameObject.transform.localPosition.y;

            m_delivered = false;

            m_resourceType = resourceType;
            m_recipient = recipient;
            m_roadToFollow = road;
            m_startRoadSegmentIndex = m_currRoadSegmentIndex = m_roadToFollow.GetStartIndex(supplier);
            m_destRoadSegmentIndex = m_roadToFollow.GetEndIndex(recipient);
            m_immediateNextDest = m_roadToFollow.GetTileAtIndex(m_startRoadSegmentIndex);
            this.transform.position = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);

            m_audioSource = this.GetComponent<AudioSource>();
            m_engineState = EngineState.Start;
            m_audioSource.clip = m_engineStartClip;
            m_audioSource.Play();

            m_generatesComponent = this.GetComponent<GeneratesPhosphorus>();
        }

        private void Update() {
            TraverseRoad();
            UpdateAudio();
        }

        private void TraverseRoad() {
            Vector3 nextDestPos = m_immediateNextDest.transform.position + new Vector3(0, m_yBuffer, 0);
            if (Vector3.Distance(this.transform.position, nextDestPos) > .1f) {
                // move to immediate next dest
                Debug.Log("[Truck] Moving towards next destination");
                this.transform.Translate((nextDestPos - this.transform.position).normalized * m_speed * Time.deltaTime);
            }
            else {
                // leak phosphorus (TODO: leak according to time, or per tile?)
                    // if per time, reference cycles component
                    // if per tile, 25% chance per tile
                if (Random.Range(0.0f, 1.0f) <= m_leakRate) {
                    m_generatesComponent.GeneratePipBatch(m_immediateNextDest);
                }

                // update immediate next dest
                if (m_currRoadSegmentIndex == m_destRoadSegmentIndex) {
                    if (!m_delivered) {
                        Deliver();
                        Debug.Log("[Truck] Delivered to final destination");
                    }
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

        private void UpdateAudio() {
            switch(m_engineState) {
                case EngineState.Start:
                    if (!m_audioSource.isPlaying) {
                        m_engineState = EngineState.Continue;
                        m_audioSource.clip = m_engineContinueClip;
                        m_audioSource.loop = true;
                        m_audioSource.Play();
                    }
                    break;
                case EngineState.Continue:

                    break;
                case EngineState.End:
                    if (!m_audioSource.isPlaying) {
                        if (m_audioSource.loop) {
                            m_audioSource.clip = m_engineEndClip;
                            m_audioSource.loop = false;
                            m_audioSource.Play();
                        }
                        else {
                            // remove truck 
                            Destroy(this.gameObject);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Deliver() {
            m_recipient.ReceiveRequestedProduct(m_resourceType);
            m_delivered = true;

            m_engineState = EngineState.End;
            m_audioSource.Stop();

            // hide truck
            m_canvas.gameObject.SetActive(false);
        }
    }
}
