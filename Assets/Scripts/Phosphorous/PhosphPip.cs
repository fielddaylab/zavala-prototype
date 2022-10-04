using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Lenses;
using Zavala.Tiles;

namespace Zavala
{
    public class PhosphPip : MonoBehaviour
    {
        private SpriteRenderer m_renderer;

        private Vector3 m_centerOffset; // this pip's offset from the center of the tiles it moves to

        private Tile m_currTile; // the tile this pip is under

        private static float TILE_BOUNDS = 0.2f;
        private static float RELATIVE_Y = 0.1f; // relative y to the tile it flows under

        public void Init(Tile parentTile) {
            m_renderer = this.GetComponent<SpriteRenderer>();
            
            float xOffset = Random.Range(-TILE_BOUNDS, TILE_BOUNDS);
            float zOffset = Random.Range(-TILE_BOUNDS, TILE_BOUNDS);

            m_centerOffset = new Vector3(xOffset, RELATIVE_Y, zOffset);

            TransferToTile(parentTile);

            m_currTile = parentTile;
            m_currTile.AddPipDirect(this);
        }

        private void Start() {
            m_renderer.enabled = LensMgr.Instance.GetLensMode() == Lenses.Mode.Phosphorus;

            EventMgr.Instance.LensModeUpdated += HandleLensModeUpdated;
        }

        #region Handlers

        private void HandleLensModeUpdated(object sender, LensModeEventArgs args) {
            switch(args.Mode) {
                case Lenses.Mode.Default:
                    m_renderer.enabled = false;
                    break;
                case Lenses.Mode.Phosphorus:
                    m_renderer.enabled = true;
                    break;
                default:
                    break;
            }
        }

        #endregion // Handlers

        public void TransferToTile(Tile newTile) {
            this.transform.position = new Vector3(
                newTile.transform.position.x + m_centerOffset.x,
                newTile.transform.position.y,
                newTile.transform.position.z + m_centerOffset.z);
        }
    }
}
