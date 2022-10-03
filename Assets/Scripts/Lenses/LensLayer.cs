using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zavala
{
    [RequireComponent(typeof(Renderer))]
    public class LensLayer : MonoBehaviour
    {
        public enum RenderLayer
        {
            Default,
            Phosphorus
        }

        [SerializeField] private RenderLayer m_renderLayer;

        [SerializeField] private Renderer[] m_renders;

        // Render Queue Vals (anything above lens mask queue val will not be rendered)
        private static int PHOSPH_FILTER = 2012;
        private static int DEFAULT_FILTER = 2002;

        // phosph == 2012
        // default == 2002
            // lens mask == 2021 (defualt + phosph)
            // lens mask == 2011 (default)

        private void Awake() {
            switch (m_renderLayer) {
                case RenderLayer.Phosphorus:
                    foreach (Renderer render in m_renders) {
                        render.material.renderQueue = PHOSPH_FILTER; // set their renderQueue to after the phosphorous mask blocks others
                    }
                    break;
                case RenderLayer.Default:
                    foreach (Renderer render in m_renders) {
                        render.material.renderQueue = DEFAULT_FILTER; // set their renderQueue to after the phosphorous mask blocks others
                    }
                    break;
                default:
                    break;
            }
        }
    }
}