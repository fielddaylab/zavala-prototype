using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zavala.Roads;
using static UnityEditor.ObjectChangeEventStream;

namespace Zavala
{
    public class EdgeSegment : MonoBehaviour
    {
        public GameObject Connection;
        private RoadBuildDir m_dir;

        private static int HEX_ANGLES = 60;
        private static int UP_OFFSET = 0;

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

        #endregion // Rotation
    }
}