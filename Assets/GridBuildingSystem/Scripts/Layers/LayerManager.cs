using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using UnityGrid;
using System;

public class LayerManager : MonoBehaviour, IDebugger
{
    public static LayerManager singleton;
    private List<UnityGrid.Layer> gridLayers;
    public int layerIndex = 0;
    public GameObject terrain;
    private Vector3 terrainOrigin;
    private Material material;
    [Header("Grid Options")]
    [SerializeField] private int numberOfCells;
    public int NumberOfCells => numberOfCells;
    private float cellSize;
    public float CellSize => cellSize;

    [SerializeField] private int width;
    public int Width => width;
    [SerializeField] private int height;
    public int Height => height;
    private int terrainSize;
    public int TerrainSize => terrainSize;
    public bool isLayerOn;

    public static LayerData[,] layerData;
    protected GameObject debugParent;

    public GridXZ<GridObject> grid;

    private void Awake()
    {
        singleton = this;
        isLayerOn = true;

        numberOfCells = (numberOfCells < 1) ? 1 : numberOfCells;
        cellSize = (float)width / (float)numberOfCells;

        Vector3 terrainOffset = new Vector3(width / 2f, 0f, height / 2f); // for plane test
        terrainOrigin = terrain.transform.position - terrainOffset;


        material = terrain.GetComponent<MeshRenderer>().material;
        material.SetFloat("CellCount", numberOfCells);
        layerData = new LayerData[numberOfCells, numberOfCells];
        gridLayers = new List<UnityGrid.Layer>();
        debugParent = new GameObject("BuildLayerDebugParent");
        FindTerrainSlops();

        grid = new GridXZ<GridObject>(Mathf.FloorToInt(width / cellSize), 
                            Mathf.FloorToInt(height / cellSize), CellSize, terrainOrigin, 
                            (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));

    }
    // Start is called before the first frame update
    void Start()
    {
        // basic layer
        UnityGrid.DefaultGridLayer terrainGrid = new UnityGrid.DefaultGridLayer(width, numberOfCells);
        gridLayers.Add(terrainGrid);
        terrainGrid.InitLayer();
        UnityGrid.Debugger.OnDebugToggle += HandleDebugToggle;

        // Buildable layer
        UnityGrid.BuildLayer buildLayer = new UnityGrid.BuildLayer(5f, width, numberOfCells, FilterMode.Point);
        gridLayers.Add(buildLayer);
        buildLayer.InitLayer();
        gridLayers[layerIndex].ApplyLayer(ref material);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isLayerOn = !isLayerOn;
            if (isLayerOn)
            {
                gridLayers[layerIndex].ApplyLayer(ref material);

            }
            else
            {
                gridLayers[layerIndex].DisableLayer(ref material);
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (isLayerOn)
            {
                layerIndex++;
                if (layerIndex >= gridLayers.Count)
                {
                    layerIndex = 0;
                }
                gridLayers[layerIndex].ApplyLayer(ref material);

            }
        }
    }

    private void FindTerrainSlops()
    {
        int i = 0;
        for (int x = numberOfCells - 1; x >= 0; x--)
        {
            for (int z = numberOfCells - 1; z >= 0; z--)
            {
                Vector3 terrainOffset = new Vector3(width / 2f, 0f, height / 2f); // for plane test 
                Vector3 gridOffset = new Vector3(cellSize / 2f, 0f, cellSize / 2f);
                //terrainOffset -= gridOffset;

                Vector3 pos = (new Vector3(z, 0f, x) * cellSize)
                    - terrainOrigin;

                Vector3 v3 = GetWorldPosition(x, z) + gridOffset;
                v3 += new Vector3(0f,100f,0f);
                if (Physics.Raycast(v3, -Vector3.up, out RaycastHit hit, 999f))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    float elevation = hit.point.y;
                    TextMesh text = CodeMonkey.Utils.UtilsClass.CreateWorldText(String.Format("{0:0.##}", angle), debugParent.transform, hit.point, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);

                    LayerData data = new LayerData(angle, elevation, hit.point, text);
                    layerData[x, z] = data;
                    Debug.DrawLine(hit.point, v3, Color.blue, 1000f);
                    
                }
                i++;
            }
        }
        debugParent.SetActive(false);
    }

    public void HandleDebugToggle(bool b)
    {
        if (b) 
        {
            debugParent.SetActive(true);
        } 
        else 
        {
            debugParent.SetActive(false);
        }

    }

    public bool IsDugOn() => debugParent.activeSelf;

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - terrainOrigin).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - terrainOrigin).z / cellSize);
    }
    public Vector3 GetWorldPosition(int x, int z)
    {
        Vector3 terrainOffset = new Vector3(terrainSize / 2f, 0f, terrainSize / 2f) * 10f;
        return new Vector3(x, 0, z) * cellSize + terrainOrigin;
    }
}
