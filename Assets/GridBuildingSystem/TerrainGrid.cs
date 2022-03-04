using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class TerrainGrid : MonoBehaviour {
    [Header("Terrain")]
    public Terrain terrain;
    private GameObject gridObject;
    private Mesh terrainMesh;
    private MeshRenderer gridMeshRenderer;
    private Vector3 gridOrigin;
    public float gridOffset = 0.1f;

    private bool debug = false;
    private GameObject debugParent;
    [Header("Grid Properties")]
    private int gridWidth;
    private int gridHeight;
    public float gridCellSize;

    [Header("Layer Materials")]
    public Material[] gridLayerMaterials;
    public List<Grid.Layer> gridLayers;


    public Texture2D colorTex;
    public Texture2D buildableColorTex;

    void Start() {
        // validate and init values
        ValidateInitialValues();
        Generate();
        InitGridLayerBuildable();
        InitGridLayer();

        gridMeshRenderer.material = gridLayerMaterials[0];
    }

    private void ValidateInitialValues() {
        gridCellSize = (gridCellSize < 1) ? 1 : gridCellSize;
        gridOrigin = terrain.transform.position;
        gridOrigin.y += gridOffset;
        gridLayers = new List<Grid.Layer>();

    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            ColorMap();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            gridMeshRenderer.material = gridLayerMaterials[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            gridMeshRenderer.material = gridLayerMaterials[1];
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            debug = !debug;
            debugParent.SetActive(debug);
        }

    }

    

    private void ColorMap() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = Vector3.zero;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) {
            mousePosition = raycastHit.point;
            int totX = gridWidth * (int)gridCellSize;
            int totZ = gridHeight * (int)gridCellSize;

            float xPos = (mousePosition - Vector3.zero).x * gridCellSize;
            float zPos = (mousePosition - Vector3.zero).z * gridCellSize;
            int x = Mathf.FloorToInt(xPos);
            int z = Mathf.FloorToInt(zPos);

            colorTex.SetPixel(z, x, Color.white);
            colorTex.Apply();
            gridLayerMaterials[0].SetTexture("ColorMap", colorTex);
        }

    }

    private void Generate() {
        gridObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridObject.transform.SetParent(transform);
        gridObject.transform.position = gridOrigin;
        int xSize = gridWidth = (int)(terrain.terrainData.size.x * gridCellSize);
        int ySize = gridHeight = (int)(terrain.terrainData.size.z * gridCellSize);
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        // MeshFilter
        MeshFilter meshFilter = gridObject.GetComponent<MeshFilter>();
        terrainMesh = meshFilter.mesh;

        // vertices
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(y / gridCellSize, 0f, x / gridCellSize);
                float terrainHeights = terrain.SampleHeight(vertices[i]);
                vertices[i].y = terrainHeights;
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uv;

        // triangles
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + 1;
                triangles[ti + 2] = vi + xSize + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + xSize + 2;
                triangles[ti + 5] = vi + xSize + 1;
            }
        }
        terrainMesh.triangles = triangles;
        terrainMesh.RecalculateNormals();

        // MeshRenderer
        gridMeshRenderer = gridObject.GetComponent<MeshRenderer>();
        gridMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        gridMeshRenderer.material = gridLayerMaterials[0];
        gridLayerMaterials[0].SetFloat("GridSize", gridWidth);

        // Collider
        MeshCollider meshCollider = gridObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = terrainMesh;
        debugParent = new GameObject();
        debugParent.transform.position = Vector3.zero;
        debugParent.name = "DebugParent";
        Utils.MeshUtils.ShowSurfaceNormals(terrainMesh, debugParent.transform);
        debugParent.SetActive(debug);

        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        
    }

    private void InitGridLayerBuildable() {
        Grid.BuildableLayer layer = new Grid.BuildableLayer(15f, terrainMesh, gridLayerMaterials[1], gridWidth, gridHeight, gridCellSize, FilterMode.Point);
        gridLayers.Add(layer);
        layer.InitLayer();
        gridLayerMaterials[1] = layer.LayerMaterial;
        gridLayerMaterials[1].SetFloat("GridSize", gridWidth * gridCellSize);

    }

    private void InitGridLayer() { 
        Grid.GridLayer layer = new Grid.GridLayer(terrainMesh, gridLayerMaterials[0], 
                                                 gridWidth, gridHeight, gridCellSize, 
                                                 FilterMode.Point);
        gridLayers.Add(layer);
        gridLayerMaterials[0] = layer.LayerMaterial;
        gridLayerMaterials[0].SetFloat("GridSize", gridWidth * gridCellSize);
    }
}

public class TerrainGridTile {

    public int x;
    public int z;
    GameObject tile;

    public TerrainGridTile(int x, int z, GameObject tile) {
        this.x = x;
        this.z = z;
        this.tile = tile;
    }
}
