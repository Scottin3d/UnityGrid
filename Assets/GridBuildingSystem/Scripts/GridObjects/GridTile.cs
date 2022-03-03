using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GridTile : MonoBehaviour
{
    private GameObject tilePlane;
    private MeshRenderer tileMeshRenderer;
    private Material tilePlaneMaterialBuildable;
    private Material tilePlaneMaterialNotBuildable;

    private Material TileEdgeMaterial;

    private Color buildable = new Color(213f, 214f, 170f, 0.25f);
    private Color notBuildable = new Color(255f, 20f, 0f, 0.25f);

    // 4 cylinder edges
    private GameObject[] tileEdges = new GameObject[4];
    private Vector3[] tileCorners = new Vector3[4];
    private GridXZ<GridTile> grid;
    private int x;
    private int z;

    private bool canBuild;

    private void Awake() {
       
    }

    public GridTile(GridXZ<GridTile> _grid, int _x, int _z) {
        grid = _grid;
        x = _x;
        z = _z;
        this.tilePlaneMaterialBuildable = GridSystem.gridSystem.GetBuildableMaterial();
        this.tilePlaneMaterialNotBuildable = GridSystem.gridSystem.GetNotBuildableMaterial();
        GenerateTile();
        SetActive(false);
    }

    public override string ToString() {
        return x + ", " + z;
    }

    private void GenerateTile() {
        tilePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        float cellSise = grid.GetCellSize();
        tileMeshRenderer = tilePlane.GetComponent<MeshRenderer>();
        tilePlane.transform.SetParent(GameObject.Find("GridSystem")?.transform);
        tilePlane.transform.localScale = new Vector3(cellSise, cellSise, cellSise) / 10f;
        tilePlane.transform.position = grid.GetCenterWorldPosition(x , z) + new Vector3(0f, 0.25f, 0f);
        SetBuild();

        tileCorners[0] = grid.GetWorldPosition(x, z);
        tileCorners[1] = grid.GetWorldPosition(x, z + 1);
        tileCorners[2] = grid.GetWorldPosition(x + 1, z + 1);
        tileCorners[3] = grid.GetWorldPosition(x + 1, z);

        // generate tile edges
        for (int i = 0; i < 4; i++) {
            GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tileEdges[i] = edge;
            Destroy(edge.GetComponent<CapsuleCollider>());

            

            int last = (i == 3) ? 0 : i + 1;
            Utils.Utils.AdjustLine(tileEdges[i], tileCorners[i], tileCorners[last], 0.05f);
            edge.transform.position += new Vector3(0f, 0.25f, 0f);
            //edge.SetActive(false);
        }

    }

    public void SetActive(bool active) {
        tilePlane.SetActive(active);
    }

    public void SetBuild() {
        canBuild = false;
        tileMeshRenderer.material = tilePlaneMaterialNotBuildable;
    }

    public void ClearBuild() {
        canBuild = true;
        tileMeshRenderer.material = tilePlaneMaterialBuildable;
    }

    public bool CanBuild() {
        return canBuild;
    }
}
