using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimModeUI : MonoBehaviour
{
    [Header("SimModeUI")]

    [SerializeField] protected string m_id;

    public string ID {
        get { return m_id; }
    }

    public abstract void Open();
}
