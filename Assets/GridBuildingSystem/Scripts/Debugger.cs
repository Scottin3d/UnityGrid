using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGrid
{
    public class Debugger : MonoBehaviour
    {
        public static Debugger instance;
        public static event Action<bool> OnDebugToggle;
        private bool isDebugOn = false;
        private void Awake()
        {
            instance = this;
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                isDebugOn = !isDebugOn;
                OnDebugToggle?.Invoke(isDebugOn);
            }
        }
    }
}