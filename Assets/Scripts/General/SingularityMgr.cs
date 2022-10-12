using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zavala
{
    public class SingularityMgr : MonoBehaviour
    {
        [SerializeField] private string m_firstScene;

        void Start() {
            SceneManager.LoadScene(m_firstScene);
        }
    }
}