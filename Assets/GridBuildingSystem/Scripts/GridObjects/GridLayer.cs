using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class Resource : Layer
    {
        public Resource(Mesh meshLayer = default, Material gridLayerMaterial = default,
                        int gridWidth = default, int gridHeight = default, float gridCellSize = 1,
                        FilterMode gridFilterMode = FilterMode.Bilinear)
                        : base(meshLayer, gridLayerMaterial, gridWidth, gridHeight, gridCellSize, gridFilterMode)
        {
        }

        public override void InitLayer()
        {
            if (meshLayer == null) { return; }
            if (gridLayerMaterial == null) { return; }

            Vector3[] vertices = meshLayer.vertices;
            Vector3[] normals = meshLayer.normals;
            int[] triangles = meshLayer.triangles;

            float[] slopes = new float[gridWidth * gridHeight];
            Color[] colors = new Color[gridWidth * gridHeight];
            Color[] colorCache = BRGBWStrip.GetPixels();

            // find surface normal slope
            for (int i = 0, j = 0; j < slopes.Length; i += 2, j++)
            {

                Vector3 p0 = vertices[triangles[i * 3 + 0]];
                Vector3 p1 = vertices[triangles[i * 3 + 1]];
                Vector3 p2 = vertices[triangles[i * 3 + 2]];
                Vector3 pNorm1 = (Vector3.Cross(p1 - p0, p2 - p0)).normalized;

                Vector3 p3 = vertices[triangles[i * 3 + 3]];
                Vector3 p4 = vertices[triangles[i * 3 + 4]];
                Vector3 p5 = vertices[triangles[i * 3 + 5]];
                Vector3 pNorm2 = (Vector3.Cross(p4 - p3, p5 - p3)).normalized;

                Vector3 pNorm = (pNorm1 + pNorm2) / 2;
                slopes[j] = Vector3.Angle(pNorm, Vector3.up);
            }

            // evaluate the slope
            // if below a certain threshold, randomly spawn a spot
            for (int i = 1, j = 0; i < colors.Length; i++, j++)
            {
                float slope = (slopes[i - 1] + slopes[i]) / 2f;
                if (2f <= slope && slope <= 10f)
                {
                    if (Random.Range(0f, 100f) < 50f) {
                        colors[j] = colorCache[50];
                        // SetResourceSpot();
                    }
                }
                else { 
                    colors[j] = colorCache[1];
                }
            }
            colorTex.SetPixels(colors, 0);
            colorTex.Apply();
            gridLayerMaterial.SetTexture("ColorMap", colorTex);
        }
    }

    public class BuildableLayer : Layer
    {
        private float buildThreshold;

        public BuildableLayer(float buildThreshold = 10f, Mesh meshLayer = default, Material gridLayerMaterial = default,
                              int gridWidth = default, int gridHeight = default, float gridCellSize = 1,
                              FilterMode gridFilterMode = FilterMode.Bilinear)
                              : base(meshLayer, gridLayerMaterial, gridWidth, gridHeight, gridCellSize, gridFilterMode)
        {

            this.buildThreshold = buildThreshold;
        }

        public override void InitLayer()
        {
            if (meshLayer == null) { return; }
            if (gridLayerMaterial == null) { return; }

            Vector3[] vertices = meshLayer.vertices;
            Vector3[] normals = meshLayer.normals;
            int[] triangles = meshLayer.triangles;

            float[] slopes = new float[gridWidth * gridHeight];
            Color[] colors = new Color[gridWidth * gridHeight];
            Color[] colorCache = BRGBWStrip.GetPixels();

            for (int i = 0, j = 0; j < slopes.Length; i += 2, j++)
            {

                Vector3 p0 = vertices[triangles[i * 3 + 0]];
                Vector3 p1 = vertices[triangles[i * 3 + 1]];
                Vector3 p2 = vertices[triangles[i * 3 + 2]];
                Vector3 pNorm1 = (Vector3.Cross(p1 - p0, p2 - p0)).normalized;

                Vector3 p3 = vertices[triangles[i * 3 + 3]];
                Vector3 p4 = vertices[triangles[i * 3 + 4]];
                Vector3 p5 = vertices[triangles[i * 3 + 5]];
                Vector3 pNorm2 = (Vector3.Cross(p4 - p3, p5 - p3)).normalized;

                Vector3 pNorm = (pNorm1 + pNorm2) / 2;
                slopes[j] = Vector3.Angle(pNorm, Vector3.up);
            }

            int[,] grid = new int[gridWidth * 2, gridHeight * 2];

            List<float> s = Utils.MeshUtils.SurfaceSlope(meshLayer);

            for (int i = 1, j = 0, x = 0, z = 0; i < colors.Length; i++, j++, z++)
            {
                if (z >= gridWidth)
                {
                    x++;
                    z = 0;
                }
                if (x == 0 || z == 0 || z >= gridHeight - 1 || x >= gridWidth - 1)
                {
                    colors[j] = colorCache[126];
                }
                else
                {
                    float slope = (slopes[i - 1] + slopes[i]) / 2f;
                    Color c = (slope >= buildThreshold) ? colorCache[1] : colorCache[50];
                    colors[j] = c;
                }
            }
            colorTex.SetPixels(colors, 0);
            colorTex.Apply();

            gridLayerMaterial.SetTexture("ColorMap", colorTex);
            //gridLayerMaterial.SetFloat("GridSize", gridWidth);
        }
    }
    public class GridLayer : Layer
    {
        public GridLayer(Mesh meshLayer = default, Material gridLayerMaterial = default,
                         int gridWidth = default, int gridHeight = default, float gridCellSize = 1,
                         FilterMode gridFilterMode = FilterMode.Bilinear)
                         : base(meshLayer, gridLayerMaterial, gridWidth, gridHeight, gridCellSize, gridFilterMode)
        {
        }

        public override void InitLayer()
        {
        }
    }
}