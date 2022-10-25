using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;
using Zavala.Roads;

namespace Zavala
{
    [RequireComponent(typeof(Inspectable))]
    public class RoadSegment : MonoBehaviour
    {
        private Inspectable m_inspectComponent;

        [SerializeField] private GameObject m_rampPrefab, m_flatPrefab;

        private List<EdgeSegment> m_edges;

        private void Awake() {
            m_inspectComponent = this.GetComponent<Inspectable>();
        }

        private void Start() {
            m_inspectComponent.Init();
        }
    }
}