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
    static class UnityExtensionsMenus
    {
        // ----------------
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all disabled Colliders", true, 1)]
        static bool DeleteAllDisabledCollidersValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var collider = (Collider)menuCommand.context;
            if (collider == null) return false;
            var gameObject = collider.gameObject;
            if (gameObject == null) return false;

            return gameObject.GetComponents<Collider>().Any(c => c.enabled == false);
        }
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all disabled Colliders", false, 1)]
        static void DeleteAllDisabledColliders(MenuCommand menuCommand)
        {
            var collider = (Collider)menuCommand.context;
            var gameObject = collider.gameObject;

            UnityExtensions.DeleteAllDisabledColliders(gameObject);
        }

        // ----------------
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all Colliders", true, 1)]
        static bool DeleteAllCollidersValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var collider = (Collider)menuCommand.context;
            if (collider == null) return false;
            var gameObject = collider.gameObject;
            if (gameObject == null) return false;

            return gameObject.GetComponents<Collider>().Length > 0;
        }
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all Colliders", false, 1)]
        static void DeleteAllColliders(MenuCommand menuCommand)
        {
            var collider = (Collider)menuCommand.context;
            var gameObject = collider.gameObject;

            UnityExtensions.DeleteAllColliders(gameObject);
        }

        // ----------------
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all Colliders but This", true, 2)]
        static bool DeleteAllCollidersButThisValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var collider = (Collider)menuCommand.context;
            if (collider == null) return false;
            var gameObject = collider.gameObject;
            if (gameObject == null) return false;

            return gameObject.GetComponents<Collider>().Any(c => collider != c);
        }
        [MenuItem("CONTEXT/Collider/ - NonConvex Collider - Delete all Colliders but This", false, 2)]
        static void DeleteAllCollidersButThis(MenuCommand menuCommand)
        {
            var collider = (Collider)menuCommand.context;
            var gameObject = collider.gameObject;

            using (var _ = new UnityExtensions.UndoGroup("Delete all Colliders but This"))
            {
                foreach (var c in gameObject.GetComponents<Collider>())
                {
                    if (collider == c) continue;

                    Undo.DestroyObjectImmediate(c);
                }
            }
        }

        // ----------------
        [MenuItem("CONTEXT/MeshRenderer/ - NonConvex Collider - Generate NonConvex Collider using Mesh for Rendering", true, 0)]
        static bool CreateProperCollidersMeshRendererValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var renderer = (MeshRenderer)menuCommand.context;
            if (renderer == null) return false;
            var gameObject = renderer.gameObject;
            if (gameObject == null) return false;
            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null) return false;
            return filter.sharedMesh != null;
        }
        [MenuItem("CONTEXT/MeshRenderer/ - NonConvex Collider - Generate NonConvex Collider using Mesh for Rendering", false, 0)]
        static void CreateProperCollidersMeshRenderer(MenuCommand menuCommand)
        {
            var renderer = (MeshRenderer)menuCommand.context;
            var gameObject = renderer.gameObject;
            var filter = gameObject.GetComponent<MeshFilter>();

            UnityExtensions.GenerateConvexMeshes(filter.sharedMesh, gameObject);
        }
                
        // ----------------
        [MenuItem("CONTEXT/MeshFilter/ - NonConvex Collider - Generate NonConvex Collider using Mesh for Rendering", true, 0)]
        static bool CreateProperCollidersMeshFilterValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var filter = (MeshFilter)menuCommand.context;
            if (filter == null) return false;
            if (filter.gameObject == null) return false;
            return filter.sharedMesh != null;
        }
        [MenuItem("CONTEXT/MeshFilter/ - NonConvex Collider - Generate NonConvex Collider using Mesh for Rendering", false, 0)]
        static void CreateProperCollidersMeshFilter(MenuCommand menuCommand)
        {
            var filter = (MeshFilter)menuCommand.context;
            var gameObject = filter.gameObject;

            UnityExtensions.GenerateConvexMeshes(filter.sharedMesh, gameObject);
        }
        
        // ----------------
        [MenuItem("CONTEXT/MeshCollider/ - NonConvex Collider - Generate NonConvex Collider using this collision mesh", true, 0)]
        static bool CreateProperCollidersMeshColliderValidation(MenuCommand menuCommand)
        {
            if (menuCommand == null) return false;
            var collider = (MeshCollider)menuCommand.context;
            if (collider == null) return false;
            var gameObject = collider.gameObject;
            if (gameObject == null) return false;
            return collider.sharedMesh != null;
        }
        [MenuItem("CONTEXT/MeshCollider/ - NonConvex Collider - Generate NonConvex Collider using this collision mesh", false, 0)]
        static void CreateProperCollidersMeshCollider(MenuCommand menuCommand)
        {
            var collider = (MeshCollider)menuCommand.context;
            var gameObject = collider.gameObject;

            UnityExtensions.GenerateConvexMeshes(collider.sharedMesh, gameObject, collider);
        }
    }
}