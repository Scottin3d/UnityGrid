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
        protected Texture2D layerMap;
        public Texture2D LayerMap => layerMap;
        protected Texture2D layerMask;
        public Texture2D LayerMask => layerMask;
        protected int numberOfCells;
        protected FilterMode filterMode;
        protected float gridOpacity = 0f;
        protected float layerOpacity = 0f;
        protected Color gridColor = Color.green;
        public Layer(int _numberOfCells, FilterMode _filterMode = FilterMode.Bilinear)
        {
            numberOfCells = (_numberOfCells < 1) ? 1 : _numberOfCells;
            filterMode = _filterMode;

            layerMap = new Texture2D(Mathf.RoundToInt(numberOfCells), Mathf.RoundToInt(numberOfCells));
            layerMask = new Texture2D(Mathf.RoundToInt(numberOfCells), Mathf.RoundToInt(numberOfCells));
            layerMask.filterMode = filterMode;
            layerMap.filterMode = filterMode;

        }
        public abstract void InitLayer();

        public abstract void ApplyLayer(ref Material material);
        public abstract void DisableLayer(ref Material material);


    }

    public static class Utils {
        /// <summary>
        /// Get the (X, Z) coordinate from a point in world space.
        /// </summary>
        /// <param name="position">The world space position.</param>
        /// <param name="gridSize">The size of the grid.</param>
        /// <param name="numberOfCells">The number of cells in the grid.</param>
        /// <returns>A <c>Vector2Int</c> of the grid coordinates.</returns>
        public static Vector2Int GetXZPosition(Vector3 position, int gridSize, int numberOfCells)
        {
            int xPos = Mathf.FloorToInt(position.x / (gridSize / numberOfCells));
            int zPos = Mathf.FloorToInt(position.z / (gridSize / numberOfCells));
            return new Vector2Int(xPos, zPos);
        }

        public static Vector3 GetWorldPositon(Vector2Int gridCoord, int gridSize, int numberOfCells) {
            return new Vector3(gridCoord.x, 0f, gridCoord.y) * (gridSize / numberOfCells);
        }


    }
    
}
