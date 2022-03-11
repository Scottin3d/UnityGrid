using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGrid;

namespace UnityGrid
{

    public class BuildLayer : Layer
    {
        private readonly float opacity = 0.65f;
        private float buildThreshold;
        private Color[] BRGBW;

        public BuildLayer(float _buildThreshold, int _gridSize, int _numberOfCells, FilterMode _filterMode = FilterMode.Bilinear)
            : base(_gridSize, _numberOfCells, _filterMode)
        {
            buildThreshold = _buildThreshold;
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
            for (int x = 0; x < LayerManager.layerData.GetLength(0); x++)
            {
                for (int z = 0; z < LayerManager.layerData.GetLength(0); z++)
                {
                    if (LayerManager.layerData[x, z].angle > buildThreshold)
                    {
                        map[i] = BRGBW[1];
                        mask[i] = Color.white;

                    }
                    else
                    {
                        map[i] = BRGBW[50];
                        mask[i] = Color.white;
                    }

                    i++;
                }
            }
            layerMap.SetPixels(map);
            layerMap.Apply();
           // UnityGrid.Utils.FlipTexture(layerMap, true);
            //Layers.Utils.FlipTexture(layerMap, false);


            layerMask.SetPixels(mask);
            layerMask.Apply();
            //UnityGrid.Utils.FlipTexture(layerMask, true);
            //Layers.Utils.FlipTexture(layerMask, false);

        }

        
    }


}