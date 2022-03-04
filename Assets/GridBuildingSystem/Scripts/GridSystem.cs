using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem gridSystem;
    [SerializeField] private Material tilePlaneMaterialBuildable;
    [SerializeField] private Material tilePlaneMaterialNotBuildable;

    private void Awake() {
        gridSystem = this;
    }

    public Material GetBuildableMaterial() {
        return tilePlaneMaterialBuildable;
    }

    public Material GetNotBuildableMaterial() {
        return tilePlaneMaterialNotBuildable;
    }

   
}
