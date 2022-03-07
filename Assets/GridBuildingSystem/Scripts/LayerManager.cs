using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using UnityGrid;
using System;

public class LayerManager : MonoBehaviour, IDebugger
{
    private List<UnityGrid.Layer> gridLayers;
    public int layerIndex = 0;
    public GameObject terrain;
    private Material material;
    [Header("Grid Options")]
    [SerializeField] private int numberOfCells;
    [SerializeField] private int terrainSize;
    public bool isLayerOn;

    public static LayerData[,] layerData;
    protected GameObject debugParent;

    // Start is called before the first frame update
    void Start()
    {
        isLayerOn = true;
        numberOfCells = (numberOfCells < 1) ? 1 : numberOfCells;
        terrainSize = Mathf.FloorToInt(terrain.transform.localScale.x);
        material = terrain.GetComponent<MeshRenderer>().material;
        material.SetFloat("CellCount", numberOfCells);
        layerData = new LayerData[numberOfCells, numberOfCells];
        gridLayers = new List<UnityGrid.Layer>();
        debugParent = new GameObject("BuildLayerDebugParent");
        FindTerrainSlops();
        // basic grid
        UnityGrid.TerrainLayer terrainGrid = new UnityGrid.TerrainLayer(terrainSize, numberOfCells);
        gridLayers.Add(terrainGrid);
        terrainGrid.InitLayer();
        UnityGrid.Debugger.OnDebugToggle += HandleDebugToggle;

        UnityGrid.BuildLayer buildLayer = new UnityGrid.BuildLayer(5f, terrainSize, numberOfCells, FilterMode.Point);
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
        for (int x = 0; x < numberOfCells; x++)
        {
            for (int z = 0; z < numberOfCells; z++)
            {
                float gridCellSize = terrainSize / numberOfCells;
                Vector3 pos = (new Vector3(x, 0f, z) * gridCellSize) + new Vector3(gridCellSize / 2, 100f, gridCellSize / 2);

                if (Physics.Raycast(pos, -Vector3.up, out RaycastHit hit, 999f))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);
                    float elevation = hit.point.y;
                    TextMesh text = CodeMonkey.Utils.UtilsClass.CreateWorldText(String.Format("{0:0.##}", angle), debugParent.transform, hit.point, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);

                    LayerData data = new LayerData(angle, elevation, text);
                    layerData[z, x] = data;
                    Debug.DrawLine(hit.point, pos, Color.blue, 1000f);
                }
                i++;
            }
        }
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

    public struct LayerData
    {
        public LayerData(float _a, float _h, TextMesh _m)
        {
            angle = _a;
            elevation = _h;
            text = _m;
        }
        public float angle { get; }
        public float elevation { get; }
        public TextMesh text { get; }
    };
}
