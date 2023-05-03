using BeauRoutine;
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Zavala.Events;
using static Zavala.Functionalities.Requests;

namespace Zavala.Functionalities
{
    public enum SimEventType {
        ExcessRunoff,
        PopDecline
    }

    public class TriggersEvents : MonoBehaviour
    {
        private List<UIEvent> m_activeEvents;

        private Vector3 m_initialQueuePos;

        [SerializeField] private float m_iconOffsetZ = 0.25f;
        [SerializeField] private bool m_queueEvents = false;

        private void OnEnable() {
            m_activeEvents = new List<UIEvent>();

            m_initialQueuePos = Vector3.zero;
        }

        private void Start() {
            m_initialQueuePos = GameDB.Instance.UIEventPrefab.transform.localPosition;
        }

        public void QueueEvent(SimEventType type) {
            Debug.Log("[Event] Queueing new event");

            if (m_initialQueuePos == Vector3.zero) {
                m_initialQueuePos = GameDB.Instance.UIEventPrefab.transform.localPosition;
            }

            QueueEventCore(type);
        }

        private void QueueEventCore(SimEventType type) {
            // init and display
            Debug.Log("[Instantiate] Instantiating UIEvent prefab");
            UIEvent newEvent = Instantiate(GameDB.Instance.UIEventPrefab, this.transform).GetComponent<UIEvent>();

            newEvent.Init(type);

            if (!m_queueEvents) {
                if (m_activeEvents.Count > 0) {
                    // remove existing event
                    for (int i = 0; i < m_activeEvents.Count; i++) {
                        Destroy(m_activeEvents[i].gameObject);
                    }
                    m_activeEvents.Clear();
                }

                m_activeEvents.Add(newEvent);
            }
            else {
                // add to events
                m_activeEvents.Add(newEvent);
                RedistributeQueue();
            }
        }

        private void RedistributeQueue() {
            Routine.Start(RedistributeRoutine());
        }

        private IEnumerator RedistributeRoutine() {
            for (int i = 0; i < m_activeEvents.Count; i++) {
                // order requests with newer on the left and older on the right
                UIEvent request = m_activeEvents[i];

                yield return MoveQueueItem(request.transform, i);
            }
        }

        private IEnumerator MoveQueueItem(Transform toMove, int index) {
            Vector3 newPos = new Vector3(
                m_initialQueuePos.x,
                m_initialQueuePos.y,
                m_initialQueuePos.z - (m_activeEvents.Count - index) * m_iconOffsetZ
                );

            yield return toMove.MoveTo(newPos, .2f, Axis.XYZ, Space.Self);
        }
    }
}
