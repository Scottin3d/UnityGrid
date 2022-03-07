using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGrid.Debug
{
    public interface IDebugger
    {

        void DebugOn();
        void DebugOff();
        bool IsDugOn();
    }

    public abstract class AbstractDebugger : MonoBehaviour, IDebugger
    {
        public abstract void DebugOn();
        public abstract void DebugOff();
        public abstract bool IsDugOn();
    }

    
}
