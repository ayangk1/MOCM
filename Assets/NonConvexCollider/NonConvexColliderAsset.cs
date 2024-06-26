using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Plawius.NonConvexCollider
{

    public class NonConvexColliderAsset : ScriptableObject
    {
        public Mesh[] ConvexMeshes = new Mesh[0];
        public long[] HashOfSourceMeshes; // hash of vertices + triangles + vhacd params

        public static NonConvexColliderAsset CreateAsset(Mesh[] meshes)
        {
            var obj = ScriptableObject.CreateInstance<NonConvexColliderAsset>();
            obj.ConvexMeshes = meshes;
            obj.HashOfSourceMeshes = new long[0];
            return obj;
        }

#if UNITY_EDITOR
        public static NonConvexColliderAsset CreateAsset(string path, Mesh[] meshes, long[] hashes)
        {
            var obj = ScriptableObject.CreateInstance<NonConvexColliderAsset>();
            AssetDatabase.CreateAsset(obj, path);
            foreach (var mesh in meshes)
                AssetDatabase.AddObjectToAsset(mesh, obj);
            obj.ConvexMeshes = meshes;
            obj.HashOfSourceMeshes = hashes;
            return obj;
        }

        public bool SameHash(long[] meshHashes)
        {
            if (HashOfSourceMeshes == null)
                HashOfSourceMeshes = new long[0];

            if (meshHashes.Length != HashOfSourceMeshes.Length)
                return false;
            foreach (var h in meshHashes)
            {
                if (HashOfSourceMeshes.Contains(h) == false)
                    return false;
            }

            return true;
        }
#endif
    }
}