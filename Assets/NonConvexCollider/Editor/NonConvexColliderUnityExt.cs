using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace Plawius.NonConvexCollider.Editor
{
    public static class UnityExtensions
    {
        public class UndoGroup : IDisposable
        {
            readonly int undoGroupId;

            public UndoGroup(string name)
            {
                undoGroupId = Undo.GetCurrentGroup();
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName(name);
            }
            
            public void Dispose()
            {
                Undo.CollapseUndoOperations(undoGroupId);
            }
        }
        
        public class DisableGroup : IDisposable
        {
            public DisableGroup(bool disabled)
            {
                EditorGUI.BeginDisabledGroup(disabled);
            }
            
            public void Dispose()
            {
                EditorGUI.EndDisabledGroup();
            }
        }


        public static void DeleteAllColliders(GameObject gameObject)
        {
            using (var _ = new UndoGroup("Delete all Colliders"))
            {
                foreach (var c in gameObject.GetComponents<Collider>())
                {
                    Undo.DestroyObjectImmediate(c);
                }
            }
        }
        
        public static void DeleteAllDisabledColliders(GameObject gameObject)
        {
            using (var _ = new UndoGroup("Delete all disabled Colliders"))
            {
                foreach (var c in gameObject.GetComponents<Collider>())
                {
                    if (c.enabled == false)
                        Undo.DestroyObjectImmediate(c);
                }
            }
        }

        static void ShowProgressBar(float progress, string title)
        {
            EditorUtility.DisplayProgressBar("Generating...", title, progress);
        }

        static void HideProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }


        public static NonConvexColliderComponent GenerateConvexMeshes(Mesh mesh, GameObject gameObjectInScene, MeshCollider originalMeshCollider = null)
        {
            var meshHashes = new long[] { HashMesh(mesh) };
            return GenerateConvexMeshes(mesh, meshHashes, gameObjectInScene, originalMeshCollider, Parameters.Default());
        }

        public static NonConvexColliderComponent GenerateConvexMeshes(Mesh mesh, long[] meshHashes, GameObject gameObjectInScene, MeshCollider originalMeshCollider = null)
        {
            return GenerateConvexMeshes(mesh, meshHashes, gameObjectInScene, originalMeshCollider, Parameters.Default());
        }

        public static NonConvexColliderComponent GenerateConvexMeshes(Mesh mesh, GameObject gameObjectInScene, MeshCollider originalMeshCollider, Parameters parameters)
        {
            var meshHashes = new long[] { HashMesh(mesh) };
            return GenerateConvexMeshes(mesh, meshHashes, gameObjectInScene, originalMeshCollider, parameters);
        }

        public static NonConvexColliderComponent GenerateConvexMeshes(Mesh mesh, long[] meshHashes, GameObject gameObjectInScene, MeshCollider originalMeshCollider, Parameters parameters)
        {
            if (gameObjectInScene == null)
            {
                throw new Exception("gameObjectInScene is null");
            }

            if (originalMeshCollider != null)
                UnityEditorInternal.ComponentUtility.CopyComponent(originalMeshCollider);

            NonConvexColliderComponent coll = null;

            try
            {
                meshHashes = AddHash(meshHashes, parameters.GetHashCode());

                var assetObject = LocateColliderAsset(meshHashes);

                using (var _ = new UndoGroup("Generate NonConvex Collider"))
                {
                    if (originalMeshCollider != null)
                        originalMeshCollider.enabled = false;

                    coll = gameObjectInScene.GetComponent<NonConvexColliderComponent>();
                    if (coll == null)
                    {
                        coll = gameObjectInScene.AddComponent<NonConvexColliderComponent>();
                        if (originalMeshCollider != null)
                        {
                            coll.IsTrigger = originalMeshCollider.isTrigger;
                            coll.Material = originalMeshCollider.sharedMaterial;
                        }
                    }

                    if (assetObject == null)
                    {
                        var meshes = API.GenerateConvexMeshes(mesh, parameters, ShowProgressBar);

                        // asset creation
                        var existingPath = AssetDatabase.GetAssetPath(mesh);
                        if (string.IsNullOrEmpty(existingPath) || existingPath.StartsWith("Assets/") == false)
                        {
                            existingPath = "Assets/GeneratedConvexColliders/" + gameObjectInScene.name + ".asset";
                            if (AssetDatabase.IsValidFolder("Assets/GeneratedConvexColliders") == false)
                                AssetDatabase.CreateFolder("Assets", "GeneratedConvexColliders");
                        }

                        var newPath = Path.ChangeExtension(existingPath, null) + "_Convex.asset";
                        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                        assetObject = NonConvexColliderAsset.CreateAsset(newPath, meshes, meshHashes);

                        coll.SetPhysicsCollider(assetObject);

                        AssetDatabase.SaveAssets();

                        EditorUtility.FocusProjectWindow();
                        EditorGUIUtility.PingObject(assetObject);

                        Debug.LogWarning("New asset with generated collider was added to your project -> " + newPath, assetObject);
                    }
                    else
                    {
                        coll.SetPhysicsCollider(assetObject);
                        Debug.LogWarning("Asset with generated collider was found and reused for this mesh + parameters combination", assetObject);
                    }
                }
            }
            finally
            {
                HideProgressBar();
            }

            return coll;
        }

        private static NonConvexColliderAsset LocateColliderAsset(long[] meshHashes)
        {
            Assert.IsNotNull(meshHashes);
            var guids = AssetDatabase.FindAssets("t:NonConvexColliderAsset");
            foreach (var gid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath<NonConvexColliderAsset>(AssetDatabase.GUIDToAssetPath(gid));
                if (asset != null && asset.SameHash(meshHashes))
                    return asset;
            }
            return null;
        }

        public static void GenerateConvexMeshesCombine(MeshFilter[] meshFilters, GameObject gameObjectInScene, Parameters parameters)
        {
            long[] hashes;
            var combined = TryToCombine(meshFilters, gameObjectInScene.transform, out hashes);
            GenerateConvexMeshes(combined, hashes, gameObjectInScene, null, parameters);
        }
        
        public static void GenerateConvexMeshesCombine(MeshCollider[] meshColliders, GameObject gameObjectInScene, Parameters parameters)
        {
            long[] hashes;
            var combined = TryToCombine(meshColliders, gameObjectInScene.transform, out hashes);
            GenerateConvexMeshes(combined, hashes, gameObjectInScene, null, parameters);
        }

        private static long[] AddHash(long[] meshHashes, long paramsHash)
        {
            var l = meshHashes.ToList();
            l.Add(paramsHash);
            return l.ToArray();
        }

        private static long HashMesh(Mesh mesh)
        {
            if (mesh == null)
                return 0;

            unchecked
            {
                long hash = 17;

                hash = hash * 23 + (mesh.vertexCount);
                hash = hash * 23 + (mesh.triangles.Length);

                for (int i = 0; i < Math.Min(32, mesh.vertexCount); ++i)
                    hash = hash * 23 + (mesh.vertices[i].GetHashCode());
                for (int i = 0; i < Math.Min(32, mesh.triangles.Length); ++i)
                    hash = hash * 23 + mesh.triangles[i];

                //Debug.LogFormat("[HASH] Mesh {0} has no guid. Calc hash is {1}", mesh.name, hash);

                return hash;
            }
        }

        private static Mesh TryToCombine(Mesh[] meshes, Transform[] transforms, Transform parent, out long[] outHashes)
        {
            if (meshes == null) throw new ArgumentNullException("meshes");
            if (transforms == null) throw new ArgumentNullException("transforms");
            if (parent == null) throw new ArgumentNullException("parent");
            if (meshes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", "meshes");
            if (meshes.Length == 1)
            {
                outHashes = new long[] { HashMesh(meshes[0]) };
                return meshes[0];
            }

#if UNITY_2017_3_OR_NEWER
            const uint maxVertexCount = uint.MaxValue;
#else
            const uint maxVertexCount = 65536;
#endif
            
            uint vertexCount = 0;
            var combine = new List<CombineInstance>(meshes.Length);
            var hashes = new List<long>(meshes.Length);
            for (var i = 0; i < meshes.Length; ++i)
            {
                var newVertexCount = vertexCount + (uint)meshes[i].vertexCount;    
                if (newVertexCount < maxVertexCount)
                {
                    combine.Add(new CombineInstance {mesh = meshes[i], 
                                                     transform = parent.worldToLocalMatrix * transforms[i].localToWorldMatrix });
                    vertexCount = newVertexCount;
                    hashes.Add(HashMesh(meshes[i]));
                }
            }

            if (vertexCount == 0)
            {
                Debug.LogError("Cannot combine meshes, too much vertices (more than " + maxVertexCount + ")");
                outHashes = new long[] { HashMesh(meshes[0]) };
                return meshes[0];
            }

            var result = new Mesh();
            
#if UNITY_2017_3_OR_NEWER
            result.indexFormat = IndexFormat.UInt32;
#endif
            result.CombineMeshes(combine.ToArray(), false, true, false);
            outHashes = hashes.ToArray();
            return result;
        }
        
        private static Mesh TryToCombine(MeshFilter[] meshFilters, Transform parent, out long[] outHashes)
        {
            return TryToCombine(meshFilters.Select(mf => mf.sharedMesh).ToArray(), 
                                meshFilters.Select(mf => mf.transform).ToArray(), 
                                parent, out outHashes);
        }
        private static Mesh TryToCombine(MeshCollider[] meshColliders, Transform parent, out long[] outHashes)
        {
            return TryToCombine(meshColliders.Select(mf => mf.sharedMesh).ToArray(), 
                                meshColliders.Select(mf => mf.transform).ToArray(), 
                                parent, out outHashes);
        }

        public static void GenerateCollidersFromMeshColliders(NonConvexColliderComponent coll)
        {
            var meshCollidersColl = coll.gameObject.GetComponentsInChildren<MeshCollider>()
                                              .Where(mc => mc != null && mc.sharedMesh != null && mc.enabled)
                                              .OrderBy(mc => mc.name).ThenBy(mc => mc.sharedMesh.vertexCount)
                                              .ToArray();

            if (meshCollidersColl.Length > 0)
                UnityExtensions.GenerateConvexMeshesCombine(meshCollidersColl, coll.gameObject, coll.Params);
        }

        public static void GenerateCollidersFromMeshCollidersThis(NonConvexColliderComponent coll)
        {
            var meshCollidersColl = coll.gameObject.GetComponent<MeshCollider>();
            if (meshCollidersColl != null && meshCollidersColl.sharedMesh != null && meshCollidersColl.enabled)
            {
                var mesh = meshCollidersColl.sharedMesh;
                var meshHashes = new long[] { HashMesh(mesh) };
                UnityExtensions.GenerateConvexMeshes(mesh, meshHashes, coll.gameObject, meshCollidersColl, coll.Params);
            }
        }
        
        public static void GenerateCollidersFromRenderingMesh(NonConvexColliderComponent coll)
        {
            var meshFiltersColl = coll.gameObject.GetComponentsInChildren<MeshFilter>()
                                            .Where(mf => mf != null && mf.sharedMesh != null)
                                            .OrderBy(mf => mf.name).ThenBy(mf => mf.sharedMesh.vertexCount)
                                            .ToArray();

            if (meshFiltersColl.Length > 0)
                UnityExtensions.GenerateConvexMeshesCombine(meshFiltersColl, coll.gameObject, coll.Params);
        }

        public static void GenerateCollidersFromRenderingMeshThis(NonConvexColliderComponent coll)
        {
            var meshFiltersColl = coll.gameObject.GetComponent<MeshFilter>();
            if (meshFiltersColl != null && meshFiltersColl.sharedMesh != null)
            {
                var mesh = meshFiltersColl.sharedMesh;
                var meshHashes = new long[] { HashMesh(mesh) };
                UnityExtensions.GenerateConvexMeshes(mesh, meshHashes, coll.gameObject, null, coll.Params);
            }
        }
    }
}