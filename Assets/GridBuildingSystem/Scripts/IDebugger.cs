using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGrid
{
    public interface IDebugger
    {
        void HandleDebugToggle(bool b);
        bool IsDugOn();
    }

    
}
