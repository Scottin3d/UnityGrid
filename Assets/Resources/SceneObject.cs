using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : GenericPoolableObject
{
    [SerializeField] private Transform pivot;
    public Transform Pivot => pivot;
    [SerializeField] private SceneObjectVisual visual;

    public void SetVisual(SceneObjectVisual v) {
        visual = v;
    }
}
