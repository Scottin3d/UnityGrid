using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class TerrainGrid : Layer
    {
        private readonly float opacity = 1f;
        public TerrainGrid(int _numberOfCells, FilterMode _filterMode = FilterMode.Bilinear) : base(_numberOfCells, _filterMode) {}
        public override void InitLayer()
        {
            Color[] c = new Color[Mathf.RoundToInt(numberOfCells) * Mathf.RoundToInt(numberOfCells)];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = Color.black;
            }
            layerMask.SetPixels(c);
            layerMask.Apply();
        }

        public override void ApplyLayer(ref Material material)
        {
            material.SetFloat("GridOpacity", opacity);
            material.SetFloat("LayerOpacity", 0);
            material.SetTexture("LayerMap", layerMap);
            material.SetTexture("LayerMask", layerMask);
        }

        public override void DisableLayer(ref Material material)
        {
            material.SetFloat("LayerOpacity", 0);
            material.SetFloat("GridOpacity", 0);
        }
    }

    public class BuildLayer : Layer {
        private readonly float opacity = 0.65f;

        private float buildThreshold;
        private float[,] buildAngles;
        private int gridSize;
        private Color[] BRGBW;
        public BuildLayer(float _buildThreshold, int _gridSize, int _numberOfCells, FilterMode _filterMode = FilterMode.Bilinear) 
            : base(_numberOfCells, _filterMode) 
        {
            buildThreshold = _buildThreshold;
            buildAngles = new float[numberOfCells, numberOfCells];
            gridSize = _gridSize;
            BRGBW = Resources.Load<Texture2D>("Texture/BRGBWStrip").GetPixels();
        }

        public override void ApplyLayer(ref Material material)
        {
            material.SetFloat("LayerOpacity", opacity);
            material.SetFloat("GridOpacity", 1);
            material.SetTexture("LayerMap", layerMap);
            material.SetTexture("LayerMask", layerMask);
        }

        public override void DisableLayer(ref Material material)
        {
            material.SetFloat("LayerOpacity", 0);
            material.SetFloat("GridOpacity", 0);

        }

        public override void InitLayer()
        {
            Color[] map = new Color[Mathf.RoundToInt(numberOfCells) * Mathf.RoundToInt(numberOfCells)];
            Color[] mask = new Color[Mathf.RoundToInt(numberOfCells) * Mathf.RoundToInt(numberOfCells)];


            int i = 0;
            for (int x = 0; x < numberOfCells; x++)
            {
                for (int z = 0; z < numberOfCells; z++)
                {
                    float gridCellSize = gridSize / numberOfCells;
                    Vector3 pos = (new Vector3(x, 0f, z) * gridCellSize) + new Vector3(gridCellSize / 2, 100f, gridCellSize / 2);

                    if (Physics.Raycast(pos, -Vector3.up, out RaycastHit hit, 999f))
                    {
                        Debug.DrawLine(pos, hit.point, Color.blue, 1000f);
                        float angle = Vector3.Angle(hit.normal, Vector3.up);
                        buildAngles[x, z] = angle;

                        if (angle > buildThreshold)
                        {
                            map[i] = BRGBW[1];
                        }
                        else {
                            map[i] = BRGBW[50];
                        }
                        mask[i] = Color.white;
                    }
                    i++;
                }
            }
            layerMap.SetPixels(map);
            layerMap.Apply();
            layerMask.SetPixels(mask);
            layerMask.Apply();

        }
    }
}