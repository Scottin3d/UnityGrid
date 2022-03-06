using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class MaterialLerp : MonoBehaviour
{
    public Texture2D colorMap;
    private Texture2D layerMask;
    public Material material;

    [Header("Grid Options")]
    [SerializeField] private float numberOfCells;

    private bool isGridOn = false;
    private float gridOpacity = 0;
    // Start is called before the first frame update
    void Start()
    {
        //material = GetComponent<MeshRenderer>().material;
        numberOfCells = (numberOfCells < 1) ? 1 : numberOfCells;
        material.SetFloat("CellCount", numberOfCells);
        colorMap = new Texture2D(Mathf.RoundToInt(numberOfCells), Mathf.RoundToInt(numberOfCells));
        material.SetTexture("ColorMap", colorMap);
        layerMask = new Texture2D(Mathf.RoundToInt(numberOfCells), Mathf.RoundToInt(numberOfCells));
        layerMask.filterMode = FilterMode.Point;
        InitLayerMask();
        material.SetTexture("LayerMask", layerMask);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            isGridOn = !isGridOn;
            gridOpacity = (isGridOn) ? 1f : 0f;
            material.SetFloat("GridOpacity", gridOpacity);
        }

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
    }

    private void InitLayerMask() {
        Color[] c = new Color[Mathf.RoundToInt(numberOfCells) * Mathf.RoundToInt(numberOfCells)];
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = Color.black;
        }
        layerMask.SetPixels(c);
        layerMask.Apply();
    }
}

public class BuildLayer : Grid.IGridLayer
{
    public void InitLayer()
    {
        throw new System.NotImplementedException();
    }
}
