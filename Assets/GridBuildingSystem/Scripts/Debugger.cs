using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGrid.Debug;

public class Debugger : MonoBehaviour
{
    public static Debugger instance;
    private bool isDebugOn = false;
    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var saveables = new List<IDebugger>();
            isDebugOn = !isDebugOn;

            var rootObjs = SceneManager.GetSceneAt(0).GetRootGameObjects();
            foreach (var root in rootObjs)
            {
                saveables.AddRange(root.GetComponentsInChildren<IDebugger>(true));
            }

            foreach (var item in saveables)
            {
                if (isDebugOn)
                {
                    item.DebugOn();
                }
                else
                {
                    item.DebugOff();
                }
            }
        }
    }
}