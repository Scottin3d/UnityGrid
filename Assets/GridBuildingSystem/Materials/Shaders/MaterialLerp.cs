using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class MaterialLerp : MonoBehaviour
{
    private List<Grid.Layer> gridLayers;
    public int layerIndex = 0;
    public Material material;

    [Header("Grid Options")]
    [SerializeField] private float numberOfCells;

    public bool isGridOn;
    private float gridOpacity = 0;
    // Start is called before the first frame update
    void Start()
    {
        isGridOn = true;
        numberOfCells = (numberOfCells < 1) ? 1 : numberOfCells;
        material.SetFloat("CellCount", numberOfCells);

        gridLayers = new List<Grid.Layer>();

        // basic grid
        Grid.TerrainGrid terrainGrid = new Grid.TerrainGrid((int)numberOfCells);
        gridLayers.Add(terrainGrid);
        terrainGrid.InitLayer();


        Grid.BuildLayer buildLayer = new Grid.BuildLayer(20f, 100, (int)numberOfCells, FilterMode.Point);
        gridLayers.Add(buildLayer);
        buildLayer.InitLayer();
        gridLayers[layerIndex].ApplyLayer(ref material);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isGridOn = !isGridOn;
            if (isGridOn)
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
            if (isGridOn)
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
