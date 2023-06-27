using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngrySquirrels.Tools
{
    /// <summary>
    /// Helper to prevent a GameObject to be destroyed.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
