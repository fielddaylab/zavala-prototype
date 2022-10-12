using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Roads;

namespace Zavala
{
    [RequireComponent(typeof(Inspectable))]
    public class RoadSegment : MonoBehaviour
    {
        private Inspectable m_inspectComponent;

        [SerializeField] private SpriteRenderer m_sr;
        [HideInInspector] public RoadBuildDir BuildDir;
        [HideInInspector] public bool MalleableDir = false;
        [HideInInspector] public RoadSegmentType SegmentType = RoadSegmentType.Straight;

        private bool m_inDisrepair;

        private static int HEX_ANGLES = 60;
        private static int UP_OFFSET = 90;

        private void Awake() {
            m_inspectComponent = this.GetComponent<Inspectable>();
        }

        private void Start() {
            m_inspectComponent.Init();
        }

        public void ModifyBuildDir(RoadBuildDir newDir, RoadBuildDir prevDir) {
            BuildDir = newDir;

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

            // rotate
            if (SegmentType == RoadSegmentType.Bend || SegmentType == RoadSegmentType.TightBend) {
                // special rotation necessary

                int bendAngleNum = 0;

                switch (prevDir) {
                    case RoadBuildDir.Up:
                        bendAngleNum = 5;
                        break;
                    case RoadBuildDir.Up_Right:
                        bendAngleNum = 0;
                        break;
                    case RoadBuildDir.Down_Right:
                        bendAngleNum = 1;
                        break;
                    case RoadBuildDir.Down:
                        bendAngleNum = 2;
                        break;
                    case RoadBuildDir.Down_Left:
                        bendAngleNum = 3;
                        break;
                    case RoadBuildDir.Up_Left:
                        bendAngleNum = 4;
                        break;
                    default:
                        break;
                }

                Debug.Log("[RoadSegment] Prev Dir of bent road: " + prevDir + " || New Dir of bent road: " + newDir);

                // Always a difference of 6

                // normal bends
                // prevDir - newDir == 1 || prevDir - newDir == -5: no change
                if (prevDir - newDir == -1 || prevDir - newDir == 5) {
                    bendAngleNum += 4;
                }
                // tight bends
                else if (prevDir - newDir == -2 || prevDir - newDir == 4) {
                    bendAngleNum += 4;
                }
                else if (prevDir - newDir == -4 || prevDir - newDir == 2) {
                    bendAngleNum += 5;
                }

                Vector3 currAngles = this.transform.eulerAngles;
                currAngles.y = UP_OFFSET + HEX_ANGLES * ((bendAngleNum) % 6);
                this.transform.eulerAngles = currAngles;
            }
            else {
                // normal rotation
                Vector3 currAngles = this.transform.eulerAngles;
                currAngles.y = UP_OFFSET + HEX_ANGLES * angleNum;
                this.transform.eulerAngles = currAngles;
            }
        }

        public void ModifySegmentType(RoadSegmentType newType) {
            SegmentType = newType;
            Sprite newSprite = RoadMgr.Instance.GetRoadSprite(newType);
            m_sr.sprite = newSprite;
        }

        public void Disrepair() {
            m_sr.color = GameDB.Instance.RoadDamagedColor;
        }

        public void Repair() {
            m_sr.color = Color.white;
        }

    }
}
