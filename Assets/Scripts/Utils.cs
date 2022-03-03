using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public static class Colors {
        public static Color CreateColorFrom255RGB(float r = 0f, float g = 0f, float b = 0f, float a = 1f) {
            return new Color(r / 255f, g / 255f, b / 255f, Mathf.Clamp01(a));
        }
    }

    public static class Utils {

        // raycast - returns intersection point on a plane
        public static bool ScottCast(out Vector3 intersection, Vector3 linePoint, Vector3 lineNormal,
                                         Vector3 planeNormal, Vector3 planePoint) {
            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineNormal, planeNormal);

            if (dotDenominator != 0.0f) {
                length = dotNumerator / dotDenominator;
                vector = lineNormal.normalized * length;
                intersection = linePoint + vector;
                return true;
            } else
                return false;
        }

        // draws a normal tangent line
        public static void DrawNormal(GameObject obj) {
            Vector3 pt = obj.transform.localPosition;
            pt.y += 2f;
            Debug.DrawLine(obj.transform.localPosition, pt, Color.red);
        }

        public static float Distance(Vector3 lhs, Vector3 rhs) {
            return (lhs - rhs).magnitude;
        }

        public static void AdjustLine(GameObject line, Vector3 p1, Vector3 p2, float lineWidth = 0.1f) {
            Vector3 V = p2 - p1;
            float length = V.magnitude;
            line.transform.localPosition = p1 + 0.5f * V;
            line.transform.localScale = new Vector3(lineWidth, length * 0.5f, lineWidth);
            line.transform.localRotation = Quaternion.FromToRotation(Vector3.up, V);
        }

        /// Summary
        /// https://github.com/GlitchEnzo/UnityProceduralPrimitives/blob/master/Assets/Procedural%20Primitives/Scripts/Primitive.cs
        /// Creates a Mesh filled with vertices forming a sphere.
        /// The values are as follows:
        /// Vertex Count   = slices * (stacks - 1) + 2
        /// Triangle Count = slices * (stacks - 1) * 2
        /// 
        /// Default sphere mesh in Unity has a radius of 0.5 with 20 slices and 20 stacks.
        /// <param> radius. Radius of the sphere. This value should be greater than or equal to 0.0f.
        /// <param> slices. Number of slices around the Y axis.
        /// <param> stacks. Number of stacks along the Y axis. Should be 2 or greater. (stack of 1 is just a cylinder)
        /// <returns> Returns the mesh filled with vertices.
        public static Mesh CreateSphereMesh(float radius, int slices = 20, int stacks = 20) {
            Mesh mesh = new Mesh();
            mesh.name = "SphereMesh";

            float sliceStep = (float)Mathf.PI * 2.0f / slices;
            float stackStep = (float)Mathf.PI / stacks;
            int vertexCount = slices * (stacks - 1) + 2;
            int triangleCount = slices * (stacks - 1) * 2;
            int indexCount = triangleCount * 3;

            Vector3[] sphereVertices = new Vector3[vertexCount];
            Vector3[] sphereNormals = new Vector3[vertexCount];
            Vector2[] sphereUVs = new Vector2[vertexCount];

            int currentVertex = 0;
            sphereVertices[currentVertex] = new Vector3(0, -radius, 0);
            sphereNormals[currentVertex] = Vector3.down;
            currentVertex++;
            float stackAngle = (float)Mathf.PI - stackStep;
            for (int i = 0; i < stacks - 1; i++) {
                float sliceAngle = 0;
                for (int j = 0; j < slices; j++) {
                    //NOTE: y and z were switched from normal spherical coordinates because the sphere is "oriented" along the Y axis as opposed to the Z axis
                    float x = (float)(radius * Mathf.Sin(stackAngle) * Mathf.Cos(sliceAngle));
                    float y = (float)(radius * Mathf.Cos(stackAngle));
                    float z = (float)(radius * Mathf.Sin(stackAngle) * Mathf.Sin(sliceAngle));

                    Vector3 position = new Vector3(x, y, z);
                    sphereVertices[currentVertex] = position;
                    sphereNormals[currentVertex] = Vector3.Normalize(position);
                    sphereUVs[currentVertex] =
                        new Vector2((float)(Mathf.Sin(sphereNormals[currentVertex].x) / Mathf.PI + 0.5f),
                            (float)(Mathf.Sin(sphereNormals[currentVertex].y) / Mathf.PI + 0.5f));

                    currentVertex++;

                    sliceAngle += sliceStep;
                }
                stackAngle -= stackStep;
            }
            sphereVertices[currentVertex] = new Vector3(0, radius, 0);
            sphereNormals[currentVertex] = Vector3.up;

            mesh.vertices = sphereVertices;
            mesh.normals = sphereNormals;
            mesh.uv = sphereUVs;
            mesh.triangles = CreateIndexBuffer(vertexCount, indexCount, slices);

            return mesh;
        }

        /// Summary
        /// Creates an index buffer for spherical shapes like Spheres, Cylinders, and Cones.
        /// <param> vertexCount. The total number of vertices making up the shape.
        /// <param> indexCount. The total number of indices making up the shape.
        /// <param> slices. The number of slices about the Y axis.
        /// <returns> The index buffer containing the index data for the shape.
        private static int[] CreateIndexBuffer(int vertexCount, int indexCount, int slices) {
            int[] indices = new int[indexCount];
            int currentIndex = 0;

            // Bottom circle/cone of shape
            for (int i = 1; i <= slices; i++) {
                indices[currentIndex++] = i;
                indices[currentIndex++] = 0;
                if (i - 1 == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
            }

            // Middle sides of shape
            for (int i = 1; i < vertexCount - slices - 1; i++) {
                indices[currentIndex++] = i + slices;
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;

                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices + slices - 1;
                else
                    indices[currentIndex++] = i + slices - 1;
            }

            // Top circle/cone of shape
            for (int i = vertexCount - slices - 1; i < vertexCount - 1; i++) {
                indices[currentIndex++] = i;
                if ((i - 1) % slices == 0)
                    indices[currentIndex++] = i + slices - 1;
                else
                    indices[currentIndex++] = i - 1;
                indices[currentIndex++] = vertexCount - 1;
            }

            return indices;
        }

        /// Summary
        /// Creates a Mesh filled with vertices forming a torus.
        /// <param> radius. The radius of the torus.
        /// <param> tubRadius.The radius of the torus tube.
        /// <param> segments. The number of segments that make up the torus.
        /// <param> tubs. The number of segments that make up the tube.
        /// <returns> Returns a torus mesh.
        public static Mesh CreateTorus(float radius, float tubeRadius = 0.1f, int segments = 36, int tubes = 6) {
            Mesh mesh = new Mesh();

            float sr = (radius > 0) ? radius : -radius;
            float tr = tubeRadius;
            int _segments = segments;
            int t = tubes;

            
            // Total vertices
            int totalVerts = _segments * t;

            // Total triangles
            int totalTriangles = totalVerts * 2;

            // Total indices
            int totalVertices = totalTriangles * 3;

            // Init vertexList and indexList
            ArrayList verticesList = new ArrayList();
            ArrayList indicesList = new ArrayList();

            // Save these locally as floats
            float numSegments = _segments;
            float numTubes = t;

            // Calculate size of segment and tube
            float segmentSize = 2 * Mathf.PI / numSegments;
            float tubeSize = 2 * Mathf.PI / numTubes;

            // Create floats for our xyz coordinates
            float x = 0;
            float y = 0;
            float z = 0;

            // Init temp lists with tubes and segments
            ArrayList segmentList = new ArrayList();
            ArrayList tubeList = new ArrayList();

            // Loop through number of tubes
            for (int i = 0; i < numSegments; i++) {
                tubeList = new ArrayList();

                for (int j = 0; j < numTubes; j++) {
                    // Calculate X, Y, Z coordinates.
                    x = (sr + tr * Mathf.Cos(j * tubeSize)) * Mathf.Cos(i * segmentSize);
                    y = (sr + tr * Mathf.Cos(j * tubeSize)) * Mathf.Sin(i * segmentSize);
                    z = tr * Mathf.Sin(j * tubeSize);

                    // Add the vertex to the tubeList
                    tubeList.Add(new Vector3(x, z, y));

                    // Add the vertex to global vertex list
                    verticesList.Add(new Vector3(x, z, y));
                }

                // Add the filled tubeList to the segmentList
                segmentList.Add(tubeList);
            }

            // Loop through the segments
            for (int i = 0; i < segmentList.Count; i++) {
                // Find next (or first) segment offset
                int n = (i + 1) % segmentList.Count;

                // Find current and next segments
                ArrayList currentTube = (ArrayList)segmentList[i];
                ArrayList nextTube = (ArrayList)segmentList[n];

                // Loop through the vertices in the tube
                for (int j = 0; j < currentTube.Count; j++) {
                    // Find next (or first) vertex offset
                    int m = (j + 1) % currentTube.Count;

                    // Find the 4 vertices that make up a quad
                    Vector3 v1 = (Vector3)currentTube[j];
                    Vector3 v2 = (Vector3)currentTube[m];
                    Vector3 v3 = (Vector3)nextTube[m];
                    Vector3 v4 = (Vector3)nextTube[j];

                    // Draw the first triangle
                    indicesList.Add((int)verticesList.IndexOf(v1));
                    indicesList.Add((int)verticesList.IndexOf(v2));
                    indicesList.Add((int)verticesList.IndexOf(v3));

                    // Finish the quad
                    indicesList.Add((int)verticesList.IndexOf(v3));
                    indicesList.Add((int)verticesList.IndexOf(v4));
                    indicesList.Add((int)verticesList.IndexOf(v1));
                }
            }

            Vector3[] vertices = new Vector3[totalVerts];
            verticesList.CopyTo(vertices);
            int[] triangles = new int[totalVertices];
            indicesList.CopyTo(triangles);
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();
            return mesh;
        }

        public static Vector3 Matrix4x4ToWorldPostion(ref Matrix4x4 m) {
            Vector3 pos = new Vector3(m.m03, m.m13, m.m23);
            return pos;
        }
    }

}