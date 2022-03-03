using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayer
{
    protected Mesh meshLayer;
    public Material GridLayerMaterial => gridLayerMaterial;
    protected Material gridLayerMaterial;
    protected Texture2D colorTex;
    public Texture2D BRGBWStrip;
    protected FilterMode gridFilterMode;
    protected int gridWidth;
    protected int gridHeight;

    public GridLayer(Mesh meshLayer = default, Material gridLayerMaterial = default, int gridWidth = default, int gridHeight = default, FilterMode gridFilterMode = FilterMode.Bilinear) {
        this.meshLayer = meshLayer;
        this.gridLayerMaterial = gridLayerMaterial;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.gridFilterMode = gridFilterMode;
        colorTex = new Texture2D(gridWidth, gridHeight);
        colorTex.filterMode = this.gridFilterMode;

        BRGBWStrip = Resources.Load("Texture/BRGBWStrip", typeof(Texture2D)) as Texture2D;
    }
    public virtual void InitLayer() {
        Color[] colors = new Color[gridWidth * gridHeight * 2];
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = Color.black;
        }
        colorTex.SetPixels(colors, 0);
        colorTex.Apply();
        gridLayerMaterial.SetTexture("ColorMap", colorTex);
    }
}

public class GridLayerBuildable : GridLayer 
{
    private float buildThreshold;
    public GridLayerBuildable(float buildThreshold = 10f, Mesh meshLayer = default, Material gridLayerMaterial = default, 
                              int gridWidth = default, int gridHeight = default, 
                              FilterMode gridFilterMode = FilterMode.Bilinear) 
        : base(meshLayer, gridLayerMaterial, gridWidth, gridHeight, gridFilterMode) {

        this.buildThreshold = buildThreshold;
    }
    public override void InitLayer() {
        if (meshLayer == null) { return; }
        if (gridLayerMaterial == null) { return; }

        Vector3[] vertices = meshLayer.vertices;
        Vector3[] normals = meshLayer.normals;
        int[] triangles = meshLayer.triangles;

        float[] slopes = new float[triangles.Length / 3];
        Color[] colors = new Color[gridWidth * gridHeight * 2];

        for (int i = 0, j = 0; i < triangles.Length / 3; i += 2, j++) {
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

        Color[] colorCache = BRGBWStrip.GetPixels();

        for (int i = 1, j = 0; i < slopes.Length; i++, j++) {
            float slope = (slopes[i - 1] + slopes[i]) / 2f;
            Color c = (slope >= buildThreshold) ? colorCache[1] : colorCache[50];
            colors[j] = c;
        }

        colorTex.SetPixels(colors, 0);
        colorTex.Apply();

        gridLayerMaterial.SetTexture("ColorMap", colorTex);
        gridLayerMaterial.SetFloat("GridSize", gridWidth);
    }
}
