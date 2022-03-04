using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class TerrainGrid : MonoBehaviour {
    
    private bool debug = false;
    private GameObject debugParent;
    [Header("Grid Properties")]
    public int gridWidth;
    public int gridHeight;
    public float gridCellSize;

    [Header("Layer Materials")]
    public Material[] gridLayerMaterials;
    public List<GridLayerBuildable> gridLayers;

    public Material gridMaterial;
    public Material buildableGridLayer;
    private TerrainGridTile[,] terrainGridTiles;
    public Terrain terrain;
    private Mesh terrainMesh;
    GameObject gridObject;

    public Texture2D colorTex;
    public Texture2D buildableColorTex;
    public Texture2D BRGBWStrip;
    // Start is called before the first frame update
    void Start() {
        Generate();
        //StartCoroutine(GenerateMesh());

        /*
        int materialGridSize = (int)gridMaterial.GetFloat("GridSize");
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        InitGridLayerBuildable();
        gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[0];

        StartCoroutine(SetColor());
        */
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            ColorMap();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { 
            gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[1];
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            debug = !debug;
            debugParent.SetActive(debug);
        }

    }

    IEnumerator SetColor() {
        for (int x = 0, z = 0; x < gridWidth; z++) {
            if (z >= gridHeight) {
                z = 0;
                x++;
            }

            gridLayers[0].SetColor(x, z, Color.white);
            yield return new WaitForSeconds(0.25f);
        }
    }



    private void InitGridLayerBuildable() {
        GridLayerBuildable layer = new GridLayerBuildable(15f, terrainMesh, gridLayerMaterials[1], gridWidth, gridHeight, FilterMode.Point);
        gridLayers.Add(layer);
        layer.InitLayer();
        gridLayerMaterials[1] = layer.GridLayerMaterial;
    }

    private void ColorMap() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mousePosition = Vector3.zero;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f)) {
            mousePosition = raycastHit.point;
            int x = Mathf.FloorToInt((mousePosition - Vector3.zero).x / 1f);
            int z = Mathf.FloorToInt((mousePosition - Vector3.zero).z / 1f);
            colorTex.SetPixel(x, z, Color.white);
            colorTex.Apply();
            gridMaterial.SetTexture("ColorMap", colorTex);
        }

    }

    private void Generate() {
        gridObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridObject.transform.SetParent(transform);
        int xSize = gridWidth = (int)terrain.terrainData.size.x;
        int ySize = gridHeight = (int)terrain.terrainData.size.z;
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        // MeshFilter
        MeshFilter meshFilter = gridObject.GetComponent<MeshFilter>();
        terrainMesh = meshFilter.mesh;

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(y, 0f, x);
                float terrainHeights = terrain.SampleHeight(vertices[i]);
                vertices[i].y = terrainHeights;
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                /*
                triangles[ti] = vi;
                triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
                */
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
        MeshRenderer meshRenderer = gridObject.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.material = gridMaterial;
        gridMaterial.SetFloat("GridSize", gridWidth);

        // Collider
        MeshCollider meshCollider = gridObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = terrainMesh;
        debugParent = new GameObject();
        debugParent.transform.position = Vector3.zero;
        debugParent.name = "DebugParent";
        Utils.MeshUtils.ShowSurfaceNormals(terrainMesh, debugParent.transform);
        debugParent.SetActive(debug);

        int materialGridSize = (int)gridMaterial.GetFloat("GridSize");
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        InitGridLayerBuildable();
        gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[1];

        //StartCoroutine(SetColor());
    }

    IEnumerator GenerateMesh() {
        gridObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gridObject.transform.SetParent(transform);
        int xSize = gridWidth = (int)terrain.terrainData.size.x;
        int ySize = gridHeight = (int)terrain.terrainData.size.z;
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        // MeshFilter
        MeshFilter meshFilter = gridObject.GetComponent<MeshFilter>();
        terrainMesh = meshFilter.mesh;

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(y, 0f, x);
                float terrainHeights = terrain.SampleHeight(vertices[i]);
                vertices[i].y = terrainHeights;
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);

                GameObject vert = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                vert.transform.position = vertices[i];
                vert.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                /*
                triangles[ti] = vi;
                triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
                */
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
        MeshRenderer meshRenderer = gridObject.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.material = gridMaterial;
        gridMaterial.SetFloat("GridSize", gridWidth);

        // Collider
        MeshCollider meshCollider = gridObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = terrainMesh;
        debugParent = new GameObject();
        debugParent.transform.position = Vector3.zero;
        debugParent.name = "DebugParent";
        Utils.MeshUtils.ShowSurfaceNormals(terrainMesh, debugParent.transform);
        debugParent.SetActive(debug);

        int materialGridSize = (int)gridMaterial.GetFloat("GridSize");
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        InitGridLayerBuildable();
        gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[0];

        //StartCoroutine(SetColor());
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
