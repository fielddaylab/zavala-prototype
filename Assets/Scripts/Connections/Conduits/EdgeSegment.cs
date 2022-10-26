using UnityEngine;
using Zavala.Roads;

namespace Zavala
{
    public class EdgeSegment : MonoBehaviour
    {
        [SerializeField] private MeshRenderer m_meshRenderer;

        [HideInInspector] public GameObject Connection;
        private RoadBuildDir m_dir;

        private static int HEX_ANGLES = 60;
        private static int UP_OFFSET = 0;
        private static float RAMP_BASE_HEIGHT = 0.19f;

        public void Activate() {
            this.gameObject.SetActive(true);
        }

        public void Deactivate() {
            this.gameObject.SetActive(false);
        }

        #region Rotation

        public void RotateEdge(RoadBuildDir newDir) {
            m_dir = newDir;

            int angleNum = 0;

            switch (newDir) {
                case RoadBuildDir.Up:
                    break;
                case RoadBuildDir.Up_Right:
                    angleNum = 1;
                    break;
                case RoadBuildDir.Down_Right:
                    angleNum = 2;
                    break;
                case RoadBuildDir.Down:
                    angleNum = 3;
                    break;
                case RoadBuildDir.Down_Left:
                    angleNum = 4;
                    break;
                case RoadBuildDir.Up_Left:
                    angleNum = 5;
                    break;
                default:
                    break;
            }

            // normal rotation
            Vector3 currAngles = this.transform.eulerAngles;
            currAngles.y = UP_OFFSET + HEX_ANGLES * angleNum;
            this.transform.eulerAngles = currAngles;
        }

        public void ScaleEdge(float elevationDelta) {
            float newYScale;

            float baseYScale = this.transform.localScale.y;
            newYScale = (baseYScale / RAMP_BASE_HEIGHT) * elevationDelta;

            this.transform.localScale = new Vector3(
                this.transform.localScale.x,
                newYScale,
                this.transform.localScale.z);
        }

        #endregion // Rotation

        public void SetMaterial(Material material) {
            m_meshRenderer.material = material;
        }
    }
}