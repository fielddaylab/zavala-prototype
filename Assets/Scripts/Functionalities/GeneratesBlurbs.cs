using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.DataDefs;

namespace Zavala.Functionalities
{
    public class GeneratesBlurbs : MonoBehaviour
    {
        public bool TryGenerateBlurb(string blurbID) {
            BlurbData toGenerate = NarrativeMgr.GetBlurbData(blurbID);
            if (toGenerate.Seen) {
                return false;
            }

            Debug.Log("[Instantiate] Instantiating blurb icon");
            UIBlurbIcon blurb = Instantiate(GameDB.Instance.UIBlurbIconPrefab, this.transform).GetComponent<UIBlurbIcon>();
            blurb.Init(toGenerate);
            toGenerate.Seen = true;
            return true;
        }
    }
}