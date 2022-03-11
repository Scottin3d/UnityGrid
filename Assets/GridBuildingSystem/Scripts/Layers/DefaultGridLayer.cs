using UnityEngine;

namespace UnityGrid
{
    public class DefaultGridLayer : Layer
    {
        private readonly float opacity = 1f;
        public DefaultGridLayer(int _gridSize, int _numberOfCells, FilterMode _filterMode = FilterMode.Bilinear) : base(_gridSize, _numberOfCells, _filterMode) { }
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


}