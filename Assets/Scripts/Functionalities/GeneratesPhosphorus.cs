using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Tiles;

namespace Zavala.Functionalities
{
    public class GeneratesPhosphorus : MonoBehaviour
    {
        [SerializeField] private int m_amt;

        public void GeneratePipBatch(Tile destTile) {
            for (int i = 0; i < m_amt; i++) {
                PhosphPip newPip = Instantiate(GameDB.Instance.PipPrefab).GetComponent<PhosphPip>();
                newPip.Init(destTile);
            }
        }
    }
}