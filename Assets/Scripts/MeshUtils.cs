/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public static class MeshUtils {

        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);


        private static Quaternion[] cachedQuaternionEulerArr;
        private static void CacheQuaternionEuler() {
            if (cachedQuaternionEulerArr != null) return;
            cachedQuaternionEulerArr = new Quaternion[360];
            for (int i = 0; i < 360; i++) {
                cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
            }
        }
        private static Quaternion GetQuaternionEuler(float rotFloat) {
            int rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;
            //if (rot >= 360) rot -= 360;
            if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
            return cachedQuaternionEulerArr[rot];
        }


        private static Quaternion[] cachedQuaternionEulerXZArr;
        private static void CacheQuaternionEulerXZ() {
            if (cachedQuaternionEulerXZArr != null) return;
            cachedQuaternionEulerXZArr = new Quaternion[360];
            for (int i = 0; i < 360; i++) {
                cachedQuaternionEulerXZArr[i] = Quaternion.Euler(0, i, 0);
            }
        }
        private static Quaternion GetQuaternionEulerXZ(float rotFloat) {
            int rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;

            if (cachedQuaternionEulerXZArr == null) CacheQuaternionEulerXZ();
            return cachedQuaternionEulerXZArr[rot];
        }



        public static Mesh CreateEmptyMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[0];
            mesh.uv = new Vector2[0];
            mesh.triangles = new int[0];
            return mesh;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles) {
            vertices = new Vector3[4 * quadCount];
            uvs = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        public static Mesh CreateMesh(Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
            return AddToMesh(null, pos, rot, baseSize, uv00, uv11);
        }

        public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
            if (mesh == null) {
                mesh = CreateEmptyMesh();
            }
            Vector3[] vertices = new Vector3[4 + mesh.vertices.Length];
            Vector2[] uvs = new Vector2[4 + mesh.uv.Length];
            int[] triangles = new int[6 + mesh.triangles.Length];

            mesh.vertices.CopyTo(vertices, 0);
            mesh.uv.CopyTo(uvs, 0);
            mesh.triangles.CopyTo(triangles, 0);

            int index = vertices.Length / 4 - 1;
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.y;
            if (skewed) {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            } else {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            //mesh.bounds = bounds;

            return mesh;
        }

        /// <summary>
        /// Create a single quad from a set of vertices.
        /// </summary>
        /// <param name="position">The position of the quad.</param>
        /// <param name="rot">The rotation of the quad.</param>
        /// <param name="baseSize"></param>
        /// <param name="uv00"></param>
        /// <param name="uv11"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Mesh CreateQuad(Vector3[] vertices, Vector2 uv00, Vector2 uv11) {
            Mesh mesh = CreateEmptyMesh();

            Vector2[] uvs = new Vector2[4 + mesh.uv.Length];
            int[] triangles = new int[6 + mesh.triangles.Length];

            int index = vertices.Length / 4 - 1;
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex1;
            triangles[tIndex + 2] = vIndex2;

            triangles[tIndex + 3] = vIndex0;
            triangles[tIndex + 4] = vIndex2;
            triangles[tIndex + 5] = vIndex3;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            //mesh.bounds = bounds;

            return mesh;
        }

        public static Mesh GenerateTerrainMesh(Vector3[] vertices, Vector2Int mapData, Vector2 uv00, Vector2 uv11) {
            int verticesPerLine = mapData.x;
            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            Mesh mesh = CreateEmptyMesh();
            int width = mapData.x;
            int height = mapData.y;

            mesh.vertices = vertices;

            int[] triangles = new int[mapData.x * mapData.y * 6];
            for (int ti = 0, vi = 0, y = 0; y < mapData.y; y++, vi++) {
                for (int x = 0; x < mapData.x; x++, ti += 6, vi++) {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + mapData.x + 1;
                    triangles[ti + 5] = vi + mapData.x + 2;
                }
            }


            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            Vector2[] uv = new Vector2[vertices.Length];
            for (int i = 0, y = 0; y <= mapData.y; y++) {
                for (int x = 0; x <= mapData.x; x++, i++) {
                    uv[i] = new Vector2(x / mapData.x, y / mapData.y);
                }
            }
            mesh.uv = uv;
            meshData.normals = mesh.normals;
            return mesh;
        }

        public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.y;
            if (skewed) {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
                vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
                vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
                vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
            } else {
                vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }
        public static void AddToMeshArraysXZ(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11) {
            //Relocate vertices
            int vIndex = index * 4;
            int vIndex0 = vIndex;
            int vIndex1 = vIndex + 1;
            int vIndex2 = vIndex + 2;
            int vIndex3 = vIndex + 3;

            baseSize *= .5f;

            bool skewed = baseSize.x != baseSize.z;
            if (skewed) {
                vertices[vIndex0] = pos + GetQuaternionEulerXZ(rot) * new Vector3(-baseSize.x, 0, baseSize.z);
                vertices[vIndex1] = pos + GetQuaternionEulerXZ(rot) * new Vector3(-baseSize.x, 0, -baseSize.z);
                vertices[vIndex2] = pos + GetQuaternionEulerXZ(rot) * new Vector3(baseSize.x, 0, -baseSize.z);
                vertices[vIndex3] = pos + GetQuaternionEulerXZ(rot) * baseSize;
            } else {
                vertices[vIndex0] = pos + GetQuaternionEulerXZ(rot - 270) * baseSize;
                vertices[vIndex1] = pos + GetQuaternionEulerXZ(rot - 180) * baseSize;
                vertices[vIndex2] = pos + GetQuaternionEulerXZ(rot - 90) * baseSize;
                vertices[vIndex3] = pos + GetQuaternionEulerXZ(rot - 0) * baseSize;
            }

            //Relocate UVs
            uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
            uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
            uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
            uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            int tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }

        public static void ShowSurfaceNormals(Mesh mesh, Transform parent = null) {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length / 3; i+=2) {
                Vector3 p0 = vertices[triangles[i * 3 + 0]];
                Vector3 p1 = vertices[triangles[i * 3 + 1]];
                Vector3 p2 = vertices[triangles[i * 3 + 2]];
                Vector3 pNorm1 = (Vector3.Cross(p1 - p0, p2 - p0)).normalized;
                Vector3 pMid1 = (p0 + p1 + p2) / 3;

                Vector3 p3 = vertices[triangles[i * 3 + 3]];
                Vector3 p4 = vertices[triangles[i * 3 + 4]];
                Vector3 p5 = vertices[triangles[i * 3 + 5]];
                Vector3 pNorm2 = (Vector3.Cross(p4 - p3, p5 - p3)).normalized;
                Vector3 pMid2 = (p3 + p4 + p5) / 3;

                Vector3 pNorm = (pNorm1 + pNorm2) / 2;
                Vector3 pMid = (pMid1 + pMid2) / 2;

                float slope = Vector3.Angle(pNorm, Vector3.up);
                TextMesh text = CodeMonkey.Utils.UtilsClass.CreateWorldText(slope.ToString(), null, pMid, 20, Color.white,TextAnchor.MiddleCenter,TextAlignment.Center);
                text.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                text.transform.SetParent(parent);
                Debug.DrawLine(p0, p1, Color.white,1000f);  // These draw the sides
                Debug.DrawLine(p1, p2, Color.white, 1000f);
                Debug.DrawLine(p2, p0, Color.white, 1000f);
                Debug.DrawLine(pMid, pMid + (pNorm * 1f), Color.blue, 1000f);  // This draws the Normal line
                Debug.Log(pNorm);
            }
            
        }

        /// <summary>
        /// Get the slope of the triangle surface compared to <c>Vector3.up</c>.
        /// </summary>
        /// <param name="surfaceNormal">The surface normal (<c>Vector3</c>)</param>
        /// <returns>The angle between the surface normal.</returns>
        public static float SurfaceSlope(Vector3 surfaceNormal) {
            return Vector3.Angle(surfaceNormal, Vector3.up);
        }

        /// <summary>
        /// Get the slope of the triangle surface.
        /// </summary>
        /// <param name="surfaceNormal">The surface normal (<c>Vector3</c>)</param>
        /// <returns>The angle between the surface normal.</returns>
        public static float SurfaceSlope(Vector3 surfaceNormal, Vector3 compareTo ) {
            return Vector3.Angle(surfaceNormal, compareTo);
        }


    }

    /// <summary>
    /// MeshData: Stores all of the object data for the terrain chunk mesh.
    /// </summary>
    public class MeshData {
        public Mesh chunkMesh;
        public int chunkSize;
        public Vector3[] vertices;
        public Vector3[] normals;

        int triangleIndex;
        private List<int> triangles;
        public int[] Triangles => triangles.ToArray();
        public Vector2[] uv;

        /// <summary>
        /// MeshData: Constructor for the object.
        /// </summary>
        /// <param name="meshWidth">The width of the mesh.</param>
        /// <param name="meshHeight">The height of the mesh.</param>
        public MeshData(int meshWidth, int meshHeight) {
            chunkSize = meshWidth;
            int size = meshWidth * meshHeight;
            vertices = new Vector3[size];
            normals = new Vector3[size];
            triangles = new List<int>();
            uv = new Vector2[size];
            triangleIndex = 0;
        }

        /// <summary>
        /// AddTriangle: Adds two triangles to the mesh triangle array.
        /// </summary>
        /// <param name="a">Vertex A.</param>
        /// <param name="b">Vertex B.</param>
        /// <param name="c">Vertex C.</param>
        public void AddTriangle(int a, int b, int c) {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        /// <summary>
        /// CreateMesh: Creates the actual mesh for the object.
        /// </summary>
        /// <returns></returns>
        public Mesh CreateMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = Triangles;
            mesh.uv = uv;
            mesh.normals = normals;
            chunkMesh = mesh;
            return mesh;
        }

        public void RecalulateNormals() {
            chunkMesh.RecalculateNormals();
        }
    }

    
}
