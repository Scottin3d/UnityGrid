using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class LayerManager : MonoBehaviour
{
    private List<Layers.Layer> gridLayers;
    public int layerIndex = 0;
    public GameObject terrain;
    private Material material;
    [Header("Grid Options")]
    [SerializeField] private float numberOfCells;
    [SerializeField] private int terrainSize;
    public bool isLayerOn;

    // Start is called before the first frame update
    void Start()
    {
        isLayerOn = true;
        numberOfCells = (numberOfCells < 1) ? 1 : numberOfCells;
        terrainSize = Mathf.FloorToInt(terrain.transform.localScale.x);
        material = terrain.GetComponent<MeshRenderer>().material;
        material.SetFloat("CellCount", numberOfCells);

        gridLayers = new List<Layers.Layer>();

        // basic grid
        Layers.TerrainLayer terrainGrid = new Layers.TerrainLayer((int)numberOfCells);
        gridLayers.Add(terrainGrid);
        terrainGrid.InitLayer();


        Layers.BuildLayer buildLayer = new Layers.BuildLayer(2f, terrainSize, (int)numberOfCells, FilterMode.Point);
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

        /*
        if (Input.GetMouseButtonDown(0)) {
            // get mouse WS position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f)) {
                Vector2Int gridPos = Grid.Utils.GetXZPosition(hit.point, 100, (int)numberOfCells);

                Debug.Log("ColorMap (" + Mathf.FloorToInt(gridPos.x) + "," + Mathf.FloorToInt(gridPos.y));
                colorMap.SetPixel(Mathf.FloorToInt(gridPos.x), Mathf.FloorToInt(gridPos.y), Color.green);
                colorMap.Apply();
                material.SetTexture("ColorMap", colorMap);
                layerMask.SetPixel(Mathf.FloorToInt(gridPos.x), Mathf.FloorToInt(gridPos.y), Color.white);
                layerMask.Apply();
                material.SetTexture("LayerMask", layerMask);

            }
        }
        */
    }
}
