using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public Material material;
    public MeshRenderer meshRenderer;
    public Texture2D text;
    public Texture2D blank;

    Texture2D RGB;
    Color[] colorCache;

    public int width;
    int size;
    // Start is called before the first frame update
    void Start()
    {
        size = width * width;
        blank = new Texture2D(width, width);
        //text = material.mainTexture as Texture2D;
        colorCache = Resources.Load<Texture2D>("Texture/BRGBWStrip").GetPixels();
        

        StartCoroutine(Generate());
    }

    IEnumerator Generate() {
        Color[] p =  text.GetPixels();
        Color[] c = blank.GetPixels();
        /*
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = Color.white;
        }
        blank.SetPixels(c);
        blank.Apply();
        material.mainTexture = blank;
        */
        //Vector3[] verts = GetComponent<MeshFilter>().mesh.vertices;
        /*

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                GameObject v = GameObject.CreatePrimitive(PrimitiveType.Plane);
                v.transform.position = new Vector3(x, 0f, z) + new Vector3(0.5f, 0f, 0.5f);
                v.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                v.name = "P: " + x + "," + z;

                yield return new WaitForSeconds(.1f);
            }
        }
        
        */
        int j = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                c[j] = p[j];
                blank.SetPixels(c);
                blank.Apply();
                material.mainTexture = blank;
                //yield return new WaitForSeconds(.1f);
                yield return new WaitForEndOfFrame();
                j++;
            }
        }

        int i = 0;
        Vector3 origin = new Vector3(5,0,5);
        for (int x = width; x > 0; x--)
        {
            for (int z = width; z > 0; z--)
            {
                GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float xPos = (x / (float)width * 10f);
                float zPos = (z / (float)width * 10f);
                v.transform.position = new Vector3(zPos, 0, xPos) - origin;
                v.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                v.name = "V: " + x + "," + z;

                yield return new WaitForEndOfFrame();
                i++;
            }
        }

        /*
        int s = size - 1;
        int r = 0;
        for (int k = 0; k < p.Length; k++)
        {
            if (r >= width) {
                r = 0;
                s--;
            }
            int index = s - (r * width);
            //p[index] = c[k];
            c[index] = p[k];

            r++;

            blank.SetPixels(c);
            blank.Apply();
            material.mainTexture = blank;
            //yield return new WaitForSeconds(.1f);
            yield return new WaitForEndOfFrame();

        }
        */
    }
}
