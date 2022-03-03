using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class TerrainGrid : MonoBehaviour {
    [Header("Grid Properties")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float gridCellSize;

    [Header("Layer Materials")]
    public Material[] gridLayerMaterials;
    public GridLayer[] gridLayers;

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
        //GenerateTerrainGrid();
        Generate();
        int materialGridSize = (int)gridMaterial.GetFloat("GridSize");
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        InitColorMap();
        //CreateTerrainMesh();

        //InitBuildSlopeColorMap();
        InitGridLayerBuildable();
        gridObject.GetComponent<MeshRenderer>().material = gridLayerMaterials[0];
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

    }

    private void InitGridLayerBuildable() {
        GridLayerBuildable layer = new GridLayerBuildable(15f, terrainMesh, gridLayerMaterials[1], gridWidth, gridHeight, FilterMode.Point);
        layer.InitLayer();
        gridLayerMaterials[1] = layer.GridLayerMaterial;
    }
    private void InitBuildSlopeColorMap() {
        buildableColorTex = new Texture2D(gridWidth, gridHeight);
        buildableColorTex.filterMode = FilterMode.Bilinear;

        Vector3[] vertices = terrainMesh.vertices;
        Vector3[] normals = terrainMesh.normals;
        int[] triangles = terrainMesh.triangles;

        float[] slopes = new float[triangles.Length / 3];
        Color[] colors = new Color[gridWidth * gridHeight * 2];

        for (int i = 0; i < slopes.Length; i++) {
            Vector3 p0 = vertices[triangles[i * 3 + 0]];
            Vector3 p1 = vertices[triangles[i * 3 + 1]];
            Vector3 p2 = vertices[triangles[i * 3 + 2]];
            Vector3 p0N = normals[triangles[i * 3 + 0]];
            Vector3 p1N = normals[triangles[i * 3 + 1]];
            Vector3 p2N = normals[triangles[i * 3 + 2]];
            Vector3 pNorm = (Vector3.Cross(p1 - p0, p2 - p0)).normalized;
            Vector3 pMid = (p0 + p1 + p2) / 3;

            slopes[i] = Utils.MeshUtils.SurfaceSlope(pNorm);
        }

        Color[] colorCache = BRGBWStrip.GetPixels();

        for (int i = 1, j =0; i < slopes.Length; i++, j++) {
            float a = ((slopes[i - 1]) / 90) * 128;
            float b = ((slopes[i]) / 90) * 128;
            //Color colorA = colorCache[Mathf.FloorToInt(a)];

            //Color colorB = colorCache[Mathf.FloorToInt(b)];
            //Utils.ColorsUtils.BlendColors(colorA, colorB);
            colors[j] = colorCache[Mathf.FloorToInt(128 - ((slopes[i - 1] + slopes[i]) / 2))];
        }

        buildableColorTex.SetPixels(colors, 0);
        buildableColorTex.Apply();

        buildableGridLayer.SetTexture("ColorMap", buildableColorTex);
        buildableGridLayer.SetFloat("GridSize", gridWidth);
    }
    private void InitColorMap() {
        for (int x = 0; x < colorTex.width; x++) {
            for (int z = 0; z < colorTex.height; z++) {
                colorTex.SetPixel(x, z, Color.black);
            }
        }
        colorTex.Apply();
        gridMaterial.SetTexture("ColorMap", colorTex);
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
                vertices[i] = new Vector3(x, 0f, y);
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
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
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
        Utils.MeshUtils.ShowSurfaceNormals(terrainMesh);
    }

    private void CreateTerrainMesh() {
        int terrainWidth = (int)terrain.terrainData.size.x;
        int terrainHeight = (int)terrain.terrainData.size.z;
        Vector3[] vertices = new Vector3[terrainWidth * terrainHeight];
        float[] terrainHeights = new float[terrainWidth * terrainHeight];
        Vector2Int mapData = new Vector2Int(terrainWidth, terrainHeight);

        for (int x = 0, i = 0; x < terrainWidth; x++) {
            for (int z = 0; z < terrainHeight; z++, i++) {
                vertices[i] = new Vector3(x, 0f, z);
                vertices[i].y = terrain.SampleHeight(vertices[i]);
                terrainHeights[i] = terrain.SampleHeight(vertices[i]);

                /*
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.position = vertices[i];
                s.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForSeconds(0.01f);
                */
            }
        }



        // GameObject
        Mesh mesh = Utils.MeshUtils.GenerateTerrainMesh(vertices, mapData, Vector2.zero, Vector2.one);
        GameObject terrainMesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
        terrainMesh.transform.SetParent(transform);

        // MeshFilter
        MeshFilter meshFilter = terrainMesh.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;

        // MeshRenderer
        MeshRenderer meshRenderer = terrainMesh.GetComponent<MeshRenderer>();
        meshRenderer.material = gridMaterial;

        // Collider
        MeshCollider meshCollider = terrainMesh.GetComponent<MeshCollider>();
        meshCollider.sharedMesh.Clear();
        meshCollider.sharedMesh = mesh;
    }

    private void GenerateTerrainGrid() {

        float terrainWidth = terrain.terrainData.size.x;
        float terrainHeight = terrain.terrainData.size.z;
        gridCellSize = terrainWidth / gridWidth;
        terrainGridTiles = new TerrainGridTile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++) {
            for (int z = 0; z < gridHeight; z++) {
                Vector3[] vertices = new Vector3[4];

                // Sample tile corners from terrain
                vertices[0] = new Vector3(x * gridCellSize, 0f, z * gridCellSize);
                vertices[0].y = terrain.SampleHeight(vertices[0]);
                //vertices[0].y = terrain.terrainData.GetHeight((int)(vertices[0].x), (int)(vertices[0].z));

                vertices[1] = new Vector3(x * gridCellSize, 0f, (z + 1) * gridCellSize);
                vertices[1].y = terrain.SampleHeight(vertices[1]);
                //vertices[1].y = terrain.terrainData.GetHeight((int)(vertices[1].x), (int)(vertices[1].z));

                vertices[2] = new Vector3((x + 1) * gridCellSize, 0f, (z + 1) * gridCellSize);
                vertices[2].y = terrain.SampleHeight(vertices[2]);
                //vertices[2].y = terrain.terrainData.GetHeight((int)(vertices[2].x), (int)(vertices[2].z));

                vertices[3] = new Vector3((x + 1) * gridCellSize, 0f, z * gridCellSize);
                vertices[3].y = terrain.SampleHeight(vertices[3]);
                //vertices[3].y = terrain.terrainData.GetHeight((int)(vertices[3].x), (int)(vertices[3].z));

                // Create quad mesh
                Mesh mesh = Utils.MeshUtils.CreateQuad(vertices, Vector2.zero, Vector2.one);
                float gridCellHalf = gridCellSize / 2f;
                Vector3 position = new Vector3((x * gridCellSize) + gridCellHalf, 0f, (z * gridCellSize) + gridCellHalf);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
                tile.transform.SetParent(transform);
                tile.name = "Tile " + x + ", " + z;
                // MeshFilter
                MeshFilter meshFilter = tile.GetComponent<MeshFilter>();
                meshFilter.mesh.Clear();
                meshFilter.mesh = mesh;

                // MeshRenderer
                MeshRenderer meshRenderer = tile.GetComponent<MeshRenderer>();
                meshRenderer.material = gridMaterial;

                // Collider
                MeshCollider meshCollider = tile.GetComponent<MeshCollider>();
                meshCollider.sharedMesh.Clear();
                meshCollider.sharedMesh = mesh;

                // Set tile to array
                terrainGridTiles[x, z] = new TerrainGridTile(x, z, tile);
            }
        }
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
