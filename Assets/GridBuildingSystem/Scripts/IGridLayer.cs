using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    /// <summary>
    /// Grid Layer interface.
    /// </summary>
    public interface IGridLayer
    {
        void InitLayer();
    }

    /// <summary>
    /// Grid Layer abstract class.
    /// </summary>
    public abstract class Layer : IGridLayer
    {
        protected Mesh meshLayer;
        public Material LayerMaterial => gridLayerMaterial;
        protected Material gridLayerMaterial;
        protected Texture2D colorTex;
        protected Texture2D BRGBWStrip;
        protected FilterMode gridFilterMode;
        protected int gridWidth;
        protected int gridHeight;
        protected float gridCellSize;

        public Layer(Mesh meshLayer = default, Material gridLayerMaterial = default,
                      int gridWidth = default, int gridHeight = default, float gridCellSize = 1,
                      FilterMode gridFilterMode = FilterMode.Bilinear)
        {
            this.meshLayer = meshLayer;
            this.gridLayerMaterial = gridLayerMaterial;
            colorTex = new Texture2D(gridWidth, gridHeight);
            this.gridFilterMode = gridFilterMode;
            colorTex.filterMode = this.gridFilterMode;
            BRGBWStrip = Resources.Load("Texture/BRGBWStrip", typeof(Texture2D)) as Texture2D;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.gridCellSize = gridCellSize;
        }
        public abstract void InitLayer();
    }
}
