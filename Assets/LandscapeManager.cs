using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGrid;
using UnityEngine.Pool;

public class LandscapeManager : MonoBehaviour
{
    [SerializeField] private SceneObjectObjectPool treePool;


    private LayerManager gameLayerManager;
    private LayerData[,] layerData;
    public Texture2D treeMap;

    public Texture2D heightMap;
    private float[,] heightValues;
    public Texture2D noiseMap;
    private float[,] noiseValues;
    private float[,] treeFloats;

    // Start is called before the first frame update
    void Start()
    {
        heightMap = Instantiate(heightMap);
        noiseMap = Instantiate(noiseMap);
        gameLayerManager = LayerManager.singleton;
        layerData = LayerManager.layerData;
        GenerateTreeMap();

        
        
    }

    private float[,] Tex2FloatGrid(Texture2D _tex) {
        int width = _tex.width;
        int height = _tex.height;
        float[,] grid = new float[width, height];
        Color[] c = _tex.GetPixels();

        int i = 0;
        for (int x = 0; x < 500; x++)
        {
            for (int z = 0; z < 500; z++)
            {
                grid[x, z] = c[i].grayscale;
                i++;
            }
        }

        return grid;
    }

    private void GenerateTreeMap() 
    {
        int width = layerData.GetLength(0);
        int height = layerData.GetLength(1);

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (layerData[x,z].elevation >= 5f) {
                    if (Random.Range(0, 100) > 50) { 
                        SceneObject obj = treePool.GetPrefabInstance();
                        obj.transform.localPosition = layerData[x, z].origin;
                    }
                
                }
                i++;
            }
        }

    }
}
