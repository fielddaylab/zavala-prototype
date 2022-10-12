using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zavala.Events;
using Zavala.Settings;

public interface IAllVars
{
    void SetRelevantVars(ref AllVars defaultAllVars);

    void HandleAllVarsUpdated(object sender, AllVarsEventArgs args);
}
