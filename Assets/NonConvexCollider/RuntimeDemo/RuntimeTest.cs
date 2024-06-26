using System;
using System.Diagnostics;
using Plawius.NonConvexCollider;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RuntimeTest : MonoBehaviour
{
	void Start()
	{
	    gameObject.AddComponent<Rigidbody>();
	    
		var filter = gameObject.AddComponent< MeshFilter >();
        var mesh = filter.mesh;
        
        using (var _ = new StopwatchScoped("Generate mesh"))
        {
            GenerateTorusMesh(mesh);
        }

        gameObject.AddComponent<MeshRenderer>();

        Mesh[] meshes;
        using (var _ = new StopwatchScoped("NonConvexCollider generate meshes"))
        {
            meshes = API.GenerateConvexMeshes(mesh, Parameters.Default());
        }

        using (var _ = new StopwatchScoped("NonConvexCollider generate add to gameobject"))
        {
            var colliderAsset = NonConvexColliderAsset.CreateAsset(meshes);

            var nonConvex = gameObject.AddComponent<NonConvexColliderComponent>();
            nonConvex.SetPhysicsCollider(colliderAsset);
        }
	}

    // -------------------------------------------------------------------------------------------------------
    private static void GenerateTorusMesh(Mesh mesh)
    {
        mesh.Clear();
        
        const float radius1 = 1f;
        const float radius2 = .3f;
        const int nbRadSeg = 24;
        const int nbSides = 18;

        #region Vertices		

        var vertices = new Vector3[(nbRadSeg + 1) * (nbSides + 1)];
        const float pi2 = Mathf.PI * 2f;
        for (var seg = 0; seg <= nbRadSeg; seg++)
        {
            var currSeg = seg == nbRadSeg ? 0 : seg;

            var t1 = (float) currSeg / nbRadSeg * pi2;
            var r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

            for (var side = 0; side <= nbSides; side++)
            {
                var currSide = side == nbSides ? 0 : side;

                var t2 = (float) currSide / nbSides * pi2;
                var r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) *
                             new Vector3(Mathf.Sin(t2) * radius2, Mathf.Cos(t2) * radius2);

                vertices[side + seg * (nbSides + 1)] = r1 + r2;
            }
        }

        #endregion

        #region Normales		

        var normales = new Vector3[vertices.Length];
        for (var seg = 0; seg <= nbRadSeg; seg++)
        {
            var currSeg = seg == nbRadSeg ? 0 : seg;

            var t1 = (float) currSeg / nbRadSeg * pi2;
            var r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

            for (var side = 0; side <= nbSides; side++)
            {
                normales[side + seg * (nbSides + 1)] = (vertices[side + seg * (nbSides + 1)] - r1).normalized;
            }
        }

        #endregion

        #region UVs

        var uvs = new Vector2[vertices.Length];
        for (var seg = 0; seg <= nbRadSeg; seg++)
        for (var side = 0; side <= nbSides; side++)
            uvs[side + seg * (nbSides + 1)] = new Vector2((float) seg / nbRadSeg, (float) side / nbSides);

        #endregion

        #region Triangles

        var nbFaces = vertices.Length;
        var nbTriangles = nbFaces * 2;
        var nbIndexes = nbTriangles * 3;
        var triangles = new int[nbIndexes];

        var i = 0;
        for (var seg = 0; seg <= nbRadSeg; seg++)
        {
            for (var side = 0; side <= nbSides - 1; side++)
            {
                var current = side + seg * (nbSides + 1);
                var next = side + (seg < (nbRadSeg) ? (seg + 1) * (nbSides + 1) : 0);

                if (i < triangles.Length - 6)
                {
                    triangles[i++] = current;
                    triangles[i++] = next;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = current + 1;
                }
            }
        }

        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
    }

    // -------------------------------------------------------------------------------------------------------
    private class StopwatchScoped : IDisposable
    {
        private readonly string name;
        private readonly Stopwatch stopwatch;
        public StopwatchScoped(string name)
        {
            this.name = name;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed > 1000)
                Debug.LogFormat("[{0}] took {1} seconds", name, elapsed / 1000.0);
            else
                Debug.LogFormat("[{0}] took {1} msec", name, elapsed);
        }
    }
}
