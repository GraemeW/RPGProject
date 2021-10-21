using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class RPGDebugger : MonoBehaviour
    {
        // Tunables
        [SerializeField] bool enableDebugger = false;
        [SerializeField] float timeScaleFraction = 1.0f;

        private void OnEnable()
        {
            if (enableDebugger)
            {
                Time.timeScale = timeScaleFraction;
            }
        }
    }
}
