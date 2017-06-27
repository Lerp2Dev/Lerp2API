using UnityEngine;
using System;
using System.Collections.Generic;

namespace Lerp2API.Utility.CSG
{
    /// <summary>
    /// Class Triangulation.
    /// </summary>
    public class Triangulation
    {

        /// <summary>
        /// Class Vertex.
        /// </summary>
        public class Vertex
        {

            /// <summary>
            /// The local position
            /// </summary>
            public Vector3 localPos; //Mesh-Local Position
                                     /// <summary>
                                     /// The position
                                     /// </summary>
            public Vector3 pos;      //World Position
                                     /// <summary>
                                     /// The normal
                                     /// </summary>
            public Vector3 normal;
            /// <summary>
            /// The uv
            /// </summary>
            public Vector2 uv;
            /// <summary>
            /// The type
            /// </summary>
            public int type;  // -1: In, 0: Out/Local, 1: Intersection

            /// <summary>
            /// Initializes a new instance of the <see cref="Vertex"/> class.
            /// </summary>
            /// <param name="p">The p.</param>
            /// <param name="t">The t.</param>
            /// <param name="n">The n.</param>
            /// <param name="u">The u.</param>
            public Vertex(Vector3 p, int t, Vector3 n, Vector2 u)
            {

                this.type = t;
                if (t == 0) { this.localPos = new Vector3(p.x, p.y, p.z); this.pos = new Vector3(); }//Local Position
                else { this.pos = new Vector3(p.x, p.y, p.z); this.localPos = new Vector3(); }//World Position
                this.normal = new Vector3(n.x, n.y, n.z);
                this.uv = new Vector2(u.x, u.y);

            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Vertex"/> class.
            /// </summary>
            /// <param name="lp">The lp.</param>
            /// <param name="p">The p.</param>
            /// <param name="t">The t.</param>
            /// <param name="n">The n.</param>
            /// <param name="u">The u.</param>
            public Vertex(Vector3 lp, Vector3 p, int t, Vector3 n, Vector2 u)
            {

                this.localPos = new Vector3(lp.x, lp.y, lp.z);
                this.pos = new Vector3(p.x, p.y, p.z);
                this.normal = new Vector3(n.x, n.y, n.z);
                this.uv = new Vector2(u.x, u.y);
                this.type = t;

            }

            /// <summary>
            /// Clones this instance.
            /// </summary>
            /// <returns>Vertex.</returns>
            public Vertex Clone()
            {

                return new Vertex(this.localPos, this.pos, this.type, this.normal, this.uv);

            }

        }

        /// <summary>
        /// Class Polygon.
        /// </summary>
        public class Polygon
        {

            /// <summary>
            /// The index vertice
            /// </summary>
            public List<int> indexVertice;

            /// <summary>
            /// Initializes a new instance of the <see cref="Polygon"/> class.
            /// </summary>
            /// <param name="indexVertices">The index vertices.</param>
            public Polygon(int[] indexVertices)
            {

                this.indexVertice = new List<int>();
                this.indexVertice.AddRange(indexVertices);

            }

            /// <summary>
            /// Clones this instance.
            /// </summary>
            /// <returns>Polygon.</returns>
            public Polygon Clone()
            {

                int[] index = new int[this.indexVertice.Count];
                for (int i = 0; i < this.indexVertice.Count; i++) index[i] = this.indexVertice[i];
                Polygon polygon = new Polygon(index);
                return polygon;

            }


        }

        /// <summary>
        /// The vertices
        /// </summary>
        public List<Vertex> vertices;
        /// <summary>
        /// The triangles
        /// </summary>
        public List<Polygon> triangles;
        /// <summary>
        /// The lower angle
        /// </summary>
        public float lowerAngle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangulation"/> class.
        /// </summary>
        public Triangulation()
        {

            this.vertices = new List<Vertex>();
            this.triangles = new List<Polygon>();
            this.lowerAngle = 1f;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangulation"/> class.
        /// </summary>
        /// <param name="meshC">The mesh c.</param>
        public Triangulation(MeshCollider meshC)
        {

            int i;
            this.lowerAngle = 1f;
            this.vertices = new List<Vertex>();
            for (i = 0; i < meshC.sharedMesh.vertices.Length; i++) this.vertices.Add(new Vertex(meshC.sharedMesh.vertices[i], 0, meshC.sharedMesh.normals[i], meshC.sharedMesh.uv[i]));
            this.triangles = new List<Polygon>();
            for (i = 0; i < meshC.sharedMesh.triangles.Length; i += 3) this.triangles.Add(new Polygon(new int[3] { meshC.sharedMesh.triangles[i], meshC.sharedMesh.triangles[i + 1], meshC.sharedMesh.triangles[i + 2] }));

        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Triangulation.</returns>
        public Triangulation Clone()
        {

            Triangulation triangulation = new Triangulation();
            int i;
            for (i = 0; i < this.vertices.Count; i++) triangulation.vertices.Add(this.vertices[i].Clone());
            for (i = 0; i < this.triangles.Count; i++) triangulation.triangles.Add(this.triangles[i].Clone());

            return triangulation;

        }

        bool existOnTriangle(Vector3 worldPosition, int onTriangle)
        {

            for (int i = 0; i < this.triangles[onTriangle].indexVertice.Count; i++)
            {

                if (this.vertices[this.triangles[onTriangle].indexVertice[i]].pos == worldPosition) return true;

            }

            return false;

        }

        /// <summary>
        /// Adds the world point on triangle.
        /// </summary>
        /// <param name="hit">The hit.</param>
        /// <param name="onTriangle">The on triangle.</param>
        public void AddWorldPointOnTriangle(RaycastHit hit, int onTriangle) { AddWorldPointOnTriangle(hit.point, onTriangle); }
        /// <summary>
        /// Adds the world point on triangle.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="onTriangle">The on triangle.</param>
        public void AddWorldPointOnTriangle(Vector3 pos, int onTriangle)
        {

            if (onTriangle < 0 || onTriangle >= this.triangles.Count) return;

            if (!existOnTriangle(pos, onTriangle))
            {

                this.vertices.Add(new Vertex(pos, 1, normalCoords(onTriangle), uvCoords(pos, onTriangle)));
                this.triangles[onTriangle].indexVertice.Add(this.vertices.Count - 1);

            }

        }
        /// <summary>
        /// Adds the world point on triangle.
        /// </summary>
        /// <param name="hit">The hit.</param>
        public void AddWorldPointOnTriangle(RaycastHit hit)
        {

            if (!existOnTriangle(hit.point, hit.triangleIndex))
            {

                this.vertices.Add(new Vertex(hit.point, 1, hit.normal, hit.textureCoord));
                this.triangles[hit.triangleIndex].indexVertice.Add(this.vertices.Count - 1);

            }

        }

        /// <summary>
        /// Updates the local position.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void updateLocalPosition(Transform matrix) { for (int i = 0; i < this.vertices.Count; i++) this.vertices[i].localPos = matrix.worldToLocalMatrix.MultiplyPoint3x4(this.vertices[i].pos); }
        /// <summary>
        /// Updates the world position.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void updateWorldPosition(Transform matrix) { for (int i = 0; i < this.vertices.Count; i++) this.vertices[i].pos = matrix.localToWorldMatrix.MultiplyPoint3x4(this.vertices[i].localPos); }

        /// <summary>
        /// Adds the triangles.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="polygons">The polygons.</param>
        public void AddTriangles(Vertex[] vertices, Polygon[] polygons)
        {

            int head = this.vertices.Count;
            int i, w;
            this.vertices.AddRange(vertices);
            for (i = 0; i < polygons.Length; i++)
            {

                for (w = 0; w < polygons[i].indexVertice.Count; w++)
                {

                    polygons[i].indexVertice[w] += head;

                }

            }

            this.triangles.AddRange(polygons);

        }

        Vector3 normalCoords(int onTriangle)
        {

            Vector3 a, b, c;

            a = this.vertices[this.triangles[onTriangle].indexVertice[0]].localPos;
            b = this.vertices[this.triangles[onTriangle].indexVertice[1]].localPos;
            c = this.vertices[this.triangles[onTriangle].indexVertice[2]].localPos;

            b = b - a;
            c = c - a;

            return Vector3.Cross(b, c).normalized;

        }

        /// <summary>
        /// Inverts the normals.
        /// </summary>
        public void invertNormals() { for (int i = 0; i < this.vertices.Count; i++) this.vertices[i].normal *= -1f; }

        Vector2 uvCoords(Vector3 point, int onTriangle)
        {

            // http://answers.unity3d.com/questions/383804/calculate-uv-coordinates-of-3d-point-on-plane-of-m.html
            // ... interpolate (extrapolate?) points outside the triangle, a more general approach must be used: the "sign" of each
            // area must be taken into account, which produces correct results for points inside or outside the triangle. In order 
            // to calculate the area "signs", we can use (guess what?) dot products - like this:


            // triangle points
            Vector3 p1 = this.vertices[this.triangles[onTriangle].indexVertice[0]].pos;
            Vector3 p2 = this.vertices[this.triangles[onTriangle].indexVertice[1]].pos;
            Vector3 p3 = this.vertices[this.triangles[onTriangle].indexVertice[2]].pos;
            // calculate vectors from point f to vertices p1, p2 and p3:
            Vector3 f1 = p1 - point; //p1-f;
            Vector3 f2 = p2 - point; //p2-f;
            Vector3 f3 = p3 - point; //p3-f;
                                     // calculate the areas (parameters order is essential in this case):
            Vector3 va = Vector3.Cross(p1 - p2, p1 - p3); // main triangle cross product
            Vector3 va1 = Vector3.Cross(f2, f3); // p1's triangle cross product
            Vector3 va2 = Vector3.Cross(f3, f1); // p2's triangle cross product
            Vector3 va3 = Vector3.Cross(f1, f2); // p3's triangle cross product
            float a = va.magnitude; // main triangle area
                                    // calculate barycentric coordinates with sign:
            float a1 = va1.magnitude / a * Mathf.Sign(Vector3.Dot(va, va1));
            float a2 = va2.magnitude / a * Mathf.Sign(Vector3.Dot(va, va2));
            float a3 = va3.magnitude / a * Mathf.Sign(Vector3.Dot(va, va3));
            // find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
            Vector2 uv1 = this.vertices[this.triangles[onTriangle].indexVertice[0]].uv;
            Vector2 uv2 = this.vertices[this.triangles[onTriangle].indexVertice[1]].uv;
            Vector2 uv3 = this.vertices[this.triangles[onTriangle].indexVertice[2]].uv;

            return uv1 * a1 + uv2 * a2 + uv3 * a3;

        }


        /// <summary>
        /// Calculates this instance.
        /// </summary>
        public void Calculate()
        {

            if (this.vertices.Count == 0) return;

            int i, w, x, q;
            float circumsphereRadius;
            Vector3 a, b, c, ac, ab, abXac, toCircumsphereCenter, ccs;
            bool allIntersections;
            List<int[]> combination = new List<int[]>();

            for (q = this.triangles.Count - 1; q > -1; q--)
            {

                if (this.triangles[q].indexVertice.Count > 3)
                {

                    allIntersections = true;

                    // Delete Duplicate
                    for (i = this.triangles[q].indexVertice.Count - 1; i > 0; i--)
                    {

                        for (w = 0; w < i; w++)
                        {

                            if (this.vertices[this.triangles[q].indexVertice[i]].type < 1) allIntersections = false;
                            if (this.vertices[this.triangles[q].indexVertice[i]].pos == this.vertices[this.triangles[q].indexVertice[w]].pos) { this.triangles[q].indexVertice.RemoveAt(i); break; }

                        }

                    }

                    if (this.triangles[q].indexVertice.Count > 3)
                    {

                        //All Combinations without repetition, some vertice of different type
                        for (i = 0; i < this.triangles[q].indexVertice.Count - 2; i++)
                        {
                            for (w = i + 1; w < this.triangles[q].indexVertice.Count - 1; w++)
                            {
                                for (x = w + 1; x < this.triangles[q].indexVertice.Count; x++)
                                {

                                    if (!allIntersections) if (this.vertices[this.triangles[q].indexVertice[i]].type == this.vertices[this.triangles[q].indexVertice[w]].type && this.vertices[this.triangles[q].indexVertice[i]].type == this.vertices[this.triangles[q].indexVertice[x]].type) continue; // Same type
                                                                                                                                                                                                                                                                                                           //if(Vector3.Angle(this.vertices[this.triangles[q].indexVertice[w]].pos-this.vertices[this.triangles[q].indexVertice[i]].pos,this.vertices[this.triangles[q].indexVertice[x]].pos-this.vertices[this.triangles[q].indexVertice[i]].pos) < this.lowerAngle) continue; // Remove triangles with angle near to 180º
                                    combination.Add(new int[3] { this.triangles[q].indexVertice[i], this.triangles[q].indexVertice[w], this.triangles[q].indexVertice[x] });

                                }
                            }
                        }

                        //Delaunay Condition
                        for (i = combination.Count - 1; i > -1; i--)
                        {

                            //Points
                            a = this.vertices[combination[i][0]].pos;
                            b = this.vertices[combination[i][1]].pos;
                            c = this.vertices[combination[i][2]].pos;

                            //Circumcenter 3Dpoints
                            //http://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
                            ac = c - a;
                            ab = b - a;
                            abXac = Vector3.Cross(ab, ac);
                            // this is the vector from a TO the circumsphere center
                            toCircumsphereCenter = (Vector3.Cross(abXac, ab) * ac.sqrMagnitude + Vector3.Cross(ac, abXac) * ab.sqrMagnitude) / (2f * abXac.sqrMagnitude);
                            // The 3 space coords of the circumsphere center then:
                            ccs = a + toCircumsphereCenter; // now this is the actual 3space location
                                                            // The three vertices A, B, C of the triangle ABC are the same distance from the circumcenter ccs.
                            circumsphereRadius = toCircumsphereCenter.magnitude;
                            // As defined by the Delaunay condition, circumcircle is empty if it contains no other vertices besides the three that define.
                            for (w = 0; w < this.triangles[q].indexVertice.Count; w++)
                            {

                                if (this.triangles[q].indexVertice[w] != combination[i][0] && this.triangles[q].indexVertice[w] != combination[i][1] && this.triangles[q].indexVertice[w] != combination[i][2])
                                {

                                    // If it's not empty, remove.
                                    if (Vector3.Distance(this.vertices[this.triangles[q].indexVertice[w]].pos, ccs) <= circumsphereRadius)
                                    {

                                        combination.RemoveAt(i);
                                        break;

                                    }

                                }

                            }

                        }

                        if (combination.Count > 0)
                        {

                            this.triangles.RemoveAt(q);
                            for (i = 0; i < combination.Count; i++)
                            {

                                /*
                                this.vertices.Add(this.vertices[combination[i][0]].Clone());
                                combination[i][0] = this.vertices.Count-1;
                                this.vertices.Add(this.vertices[combination[i][1]].Clone());
                                combination[i][1] = this.vertices.Count-1;
                                this.vertices.Add(this.vertices[combination[i][2]].Clone());
                                combination[i][2] = this.vertices.Count-1;
                                */

                                this.triangles.Add(new Polygon(combination[i]));

                            }

                        }

                        combination.Clear();

                    }

                }

            }

        }

    }
}