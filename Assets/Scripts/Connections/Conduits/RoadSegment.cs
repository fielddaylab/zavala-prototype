using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Roads;

namespace Zavala
{
    public class RoadSegment : MonoBehaviour
    {
        [HideInInspector] public RoadBuildDir BuildDir;
        [HideInInspector] public bool MalleableDir = false;
        [HideInInspector] public RoadSegmentType SegmentType = RoadSegmentType.Straight;

        private static int HEX_ANGLES = 60;

        public void ModifyBuildDir(RoadBuildDir newDir) {
            BuildDir = newDir;

            int angleNum = 0;

            switch(newDir) {
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
            if (SegmentType == RoadSegmentType.Bend) {
                // special rotation necessary
                Vector3 currAngles = this.transform.eulerAngles;

                if (angleNum < 3) {
                    angleNum += 3;
                }
                currAngles.y = HEX_ANGLES * ((angleNum) % 6);
                this.transform.eulerAngles = currAngles;
            }
            else {
                // normal rotation
                Vector3 currAngles = this.transform.eulerAngles;
                currAngles.y = HEX_ANGLES * angleNum;
                this.transform.eulerAngles = currAngles;
            }

        }

    }
}
