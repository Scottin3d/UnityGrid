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

    public Material gridMaterial;
    private TerrainGridTile[,] terrainGridTiles;
    public Terrain terrain;

    public Texture2D colorTex;
    // Start is called before the first frame update
    void Start() {
        //GenerateTerrainGrid();
        Generate();
        int materialGridSize = (int)gridMaterial.GetFloat("GridSize");
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = FilterMode.Point;
        InitColorMap();
        //CreateTerrainMesh();
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            ColorMap();
        }

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
        GameObject terrainMesh = GameObject.CreatePrimitive(PrimitiveType.Plane);
        terrainMesh.transform.SetParent(transform);
        int xSize = gridWidth = (int)terrain.terrainData.size.x;
        int ySize = gridHeight = (int)terrain.terrainData.size.z;
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        // MeshFilter
        MeshFilter meshFilter = terrainMesh.GetComponent<MeshFilter>();

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(x, 0f, y);
                float terrainHeights = terrain.SampleHeight(vertices[i]);
                vertices[i].y = terrainHeights;
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        meshFilter.mesh.triangles = triangles;
        meshFilter.mesh.RecalculateNormals();
        // MeshRenderer
        MeshRenderer meshRenderer = terrainMesh.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.material = gridMaterial;
        gridMaterial.SetFloat("GridSize", gridWidth);

        // Collider
        MeshCollider meshCollider = terrainMesh.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;
        Utils.MeshUtils.ShowSurfaceNormals(meshFilter.mesh);
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
