using UnityEngine;

namespace Lerp2API.Utility.CSG
{
    /// <summary>
    /// Class BooleanMesh.
    /// </summary>
    public class BooleanMesh
    {

        MeshCollider ObjectA;
        MeshCollider ObjectB;

        Triangulation triangulationA;
        Triangulation triangulationB;

        float distance, customDistance;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanMesh"/> class.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        public BooleanMesh(MeshCollider A, MeshCollider B)
        {

            this.ObjectA = A;
            this.ObjectB = B;
            this.triangulationA = new Triangulation(A);
            this.triangulationB = new Triangulation(B);
            this.distance = 100f;

        }

        class intersectionDATA
        {

            /// <summary>
            /// a
            /// </summary>
            public Triangulation A, B;
            /// <summary>
            /// The mesh collider b
            /// </summary>
            public MeshCollider meshColliderB;
            /// <summary>
            /// The triangle a
            /// </summary>
            public int triangleA;
            /// <summary>
            /// The custom distance
            /// </summary>
            public float customDistance;
            /// <summary>
            /// The r1
            /// </summary>
            public Ray r1, r2;
            /// <summary>
            /// The hit
            /// </summary>
            public RaycastHit hit;

            /// <summary>
            /// Initializes a new instance of the <see cref="intersectionDATA"/> class.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <param name="m">The m.</param>
            public intersectionDATA(Triangulation a, Triangulation b, MeshCollider m)
            {

                this.A = a;
                this.B = b;
                this.meshColliderB = m;
                this.r1 = new Ray();
                this.r2 = new Ray();
                this.hit = new RaycastHit();

            }

        }

        void intersectionPoint(intersectionDATA var)
        {

            var.A.AddWorldPointOnTriangle(var.hit.point, var.triangleA);
            var.B.AddWorldPointOnTriangle(var.hit);

        }

        void intersectionRay(int originVertice, int toVertice, intersectionDATA var)
        {

            var.r1.origin = var.A.vertices[var.A.triangles[var.triangleA].indexVertice[originVertice]].pos;
            var.r2.origin = var.A.vertices[var.A.triangles[var.triangleA].indexVertice[toVertice]].pos;
            var.r1.direction = (var.r2.origin - var.r1.origin).normalized;
            var.r2.direction = (var.r1.origin - var.r2.origin).normalized;

            var.customDistance = Vector3.Distance(var.r1.origin, var.r2.origin);

            if (var.A.vertices[var.A.triangles[var.triangleA].indexVertice[originVertice]].type == 0) if (var.meshColliderB.Raycast(var.r1, out var.hit, var.customDistance)) intersectionPoint(var);
            if (var.A.vertices[var.A.triangles[var.triangleA].indexVertice[toVertice]].type == 0) if (var.meshColliderB.Raycast(var.r2, out var.hit, var.customDistance)) intersectionPoint(var);

        }

        void AInToB(intersectionDATA var)
        {

            // Vertices A In MeshCollider B
            for (int i = 0; i < var.A.vertices.Count; i++)
            {

                if (In(var.meshColliderB, var.A.vertices[i].pos)) var.A.vertices[i].type = -1; //In
                else var.A.vertices[i].type = 0; //Out

            }

        }

        void intersectionsAtoB(intersectionDATA var)
        {

            for (int i = 0; i < var.A.triangles.Count; i++)
            {

                var.triangleA = i;
                intersectionRay(0, 1, var);
                intersectionRay(0, 2, var);
                intersectionRay(1, 2, var);

            }

        }

        void clearVertices(Triangulation triangulation, int t)
        {

            int i, w;

            for (i = triangulation.triangles.Count - 1; i > -1; i--)
            {
                for (w = triangulation.triangles[i].indexVertice.Count - 1; w > -1; w--)
                {

                    if (triangulation.vertices[triangulation.triangles[i].indexVertice[w]].type == t) triangulation.triangles[i].indexVertice.RemoveAt(w);

                }

                if (triangulation.triangles[i].indexVertice.Count < 3) triangulation.triangles.RemoveAt(i);

            }

        }

        void recalculateTriangles(Vector3[] vertices, Vector3[] normals, int[] triangles)
        {

            Vector3 a, b, c;
            int v1, v2, v3;

            for (int i = 0; i < triangles.Length; i += 3)
            {

                v1 = triangles[i];
                v2 = triangles[i + 1];
                v3 = triangles[i + 2];

                a = vertices[v1];
                b = vertices[v2];
                c = vertices[v3];

                if (Vector3.Dot(normals[v1] + normals[v2] + normals[v3], Vector3.Cross((b - a), (c - a))) < 0f)
                {

                    triangles[i + 2] = v1;
                    triangles[i] = v3;

                }

            }

        }

        Mesh triangulationMesh()
        {

            this.triangulationA.Calculate();
            this.triangulationB.Calculate();

            int i;
            Mesh mesh = new Mesh();
            mesh.subMeshCount = 2;

            int tA = this.triangulationA.triangles.Count;
            int tB = this.triangulationB.triangles.Count;

            int[] trianglesA = new int[tA * 3];
            int[] trianglesB = new int[tB * 3];

            this.triangulationA.AddTriangles(triangulationB.vertices.ToArray(), triangulationB.triangles.ToArray());
            this.triangulationA.updateLocalPosition(ObjectA.transform);

            Vector3[] vertices = new Vector3[triangulationA.vertices.Count];
            Vector3[] normals = new Vector3[triangulationA.vertices.Count];
            Vector2[] uv = new Vector2[triangulationA.vertices.Count];

            for (i = 0; i < triangulationA.vertices.Count; i++)
            {

                vertices[i] = triangulationA.vertices[i].localPos;
                normals[i] = triangulationA.vertices[i].normal.normalized;
                uv[i] = triangulationA.vertices[i].uv;

            }

            for (i = 0; i < tA; i++)
            {

                trianglesA[i * 3] = triangulationA.triangles[i].indexVertice[0];
                trianglesA[i * 3 + 1] = triangulationA.triangles[i].indexVertice[1];
                trianglesA[i * 3 + 2] = triangulationA.triangles[i].indexVertice[2];

            }

            for (i = 0; i < tB; i++)
            {

                trianglesB[i * 3] = triangulationA.triangles[tA + i].indexVertice[0];
                trianglesB[i * 3 + 1] = triangulationA.triangles[tA + i].indexVertice[1];
                trianglesB[i * 3 + 2] = triangulationA.triangles[tA + i].indexVertice[2];

            }

            recalculateTriangles(vertices, normals, trianglesA);
            recalculateTriangles(vertices, normals, trianglesB);
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.SetTriangles(trianglesA, 0);
            mesh.SetTriangles(trianglesB, 1);

            return mesh;

        }

        /// <summary>
        /// Unions this instance.
        /// </summary>
        /// <returns>Mesh.</returns>
        public Mesh Union()
        {

            intersections();
            clearVertices(this.triangulationA, -1);
            clearVertices(this.triangulationB, -1);
            return triangulationMesh();

        }

        /// <summary>
        /// Intersections this instance.
        /// </summary>
        /// <returns>Mesh.</returns>
        public Mesh Intersection()
        {

            intersections();
            clearVertices(this.triangulationA, 0);
            clearVertices(this.triangulationB, 0);
            return triangulationMesh();

        }

        /// <summary>
        /// Differences this instance.
        /// </summary>
        /// <returns>Mesh.</returns>
        public Mesh Difference()
        {

            intersections();
            clearVertices(this.triangulationA, -1);
            clearVertices(this.triangulationB, 0);
            this.triangulationB.invertNormals();
            return triangulationMesh();

        }

        void intersections()
        {

            //Update world position vertices
            this.triangulationA.updateWorldPosition(ObjectA.transform);
            this.triangulationB.updateWorldPosition(ObjectB.transform);

            //IntersectionDATA
            intersectionDATA varA = new intersectionDATA(this.triangulationA, this.triangulationB, this.ObjectB);
            intersectionDATA varB = new intersectionDATA(this.triangulationB, this.triangulationA, this.ObjectA);

            //In/Out Points
            AInToB(varA);
            AInToB(varB);

            //Intersections
            intersectionsAtoB(varA);
            intersectionsAtoB(varB);

        }

        /// <summary>
        /// CSGs the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>Mesh.</returns>
        public Mesh CSG(CSGType t)
        {
            switch (t)
            {
                case CSGType.Intersection:
                    return this.Intersection();
                case CSGType.Union:
                    return this.Union();
                case CSGType.Difference:
                    return this.Difference();
                default:
                    return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////

        bool r, l, u, d, f, b;

        RaycastHit rightHit = new RaycastHit();
        RaycastHit leftHit = new RaycastHit();
        RaycastHit upHit = new RaycastHit();
        RaycastHit downHit = new RaycastHit();
        RaycastHit forwardHit = new RaycastHit();
        RaycastHit backHit = new RaycastHit();
        RaycastHit tempHit = new RaycastHit();

        Ray right = new Ray(Vector3.zero, -Vector3.right);
        Ray left = new Ray(Vector3.zero, -Vector3.left);
        Ray up = new Ray(Vector3.zero, -Vector3.up);
        Ray down = new Ray(Vector3.zero, -Vector3.down);
        Ray forward = new Ray(Vector3.zero, -Vector3.forward);
        Ray back = new Ray(Vector3.zero, -Vector3.back);
        Ray tempRay = new Ray();

        bool ConcaveHull(MeshCollider meshCollider, Vector3 position, Ray ray, RaycastHit hit)
        {


            tempRay.origin = position;
            tempRay.direction = -ray.direction;
            customDistance = distance - hit.distance;

            while (meshCollider.Raycast(tempRay, out tempHit, customDistance))
            {

                if (tempHit.triangleIndex == hit.triangleIndex) break;
                ray.origin = -ray.direction * customDistance + position;

                if (!meshCollider.Raycast(ray, out hit, customDistance)) return true;

                if (tempHit.triangleIndex == hit.triangleIndex) break;
                customDistance -= hit.distance;

            }

            return false;

        }

        bool In(MeshCollider meshCollider, Vector3 position)
        {

            right.origin = -right.direction * distance + position;
            left.origin = -left.direction * distance + position;
            up.origin = -up.direction * distance + position;
            down.origin = -down.direction * distance + position;
            forward.origin = -forward.direction * distance + position;
            back.origin = -back.direction * distance + position;

            r = meshCollider.Raycast(right, out rightHit, distance);
            l = meshCollider.Raycast(left, out leftHit, distance);
            u = meshCollider.Raycast(up, out upHit, distance);
            d = meshCollider.Raycast(down, out downHit, distance);
            f = meshCollider.Raycast(forward, out forwardHit, distance);
            b = meshCollider.Raycast(back, out backHit, distance);

            if (r && l && u && d && f && b)
            {

                if (!ConcaveHull(meshCollider, position, right, rightHit))
                    if (!ConcaveHull(meshCollider, position, left, leftHit))
                        if (!ConcaveHull(meshCollider, position, up, upHit))
                            if (!ConcaveHull(meshCollider, position, down, downHit))
                                if (!ConcaveHull(meshCollider, position, forward, forwardHit))
                                    if (!ConcaveHull(meshCollider, position, back, backHit)) return true;

            }

            return false;

        }

        ///////////////////////////////////////////////////////////////////////////////


    }
}