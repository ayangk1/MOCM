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
    [CustomEditor(typeof(NonConvexColliderComponent))]
    [CanEditMultipleObjects]
    public class NonConvexColliderComponentEditor : UnityEditor.Editor 
    {
        SerializedProperty m_parameters;

        SerializedProperty m_parameters_maxConvexHulls;
        SerializedProperty m_parameters_resolution;
        SerializedProperty m_parameters_concavity;
        SerializedProperty m_parameters_planeDownsampling;
        SerializedProperty m_parameters_convexhullApproximation;
        SerializedProperty m_parameters_alpha;
        SerializedProperty m_parameters_beta;
        SerializedProperty m_parameters_pca;
        SerializedProperty m_parameters_mode;
        SerializedProperty m_parameters_maxNumVerticesPerCH;
        SerializedProperty m_parameters_minVolumePerCH;

        SerializedProperty m_asset;
        SerializedProperty m_isTrigger;
        SerializedProperty m_material;

        SerializedProperty m_colliders;
        SerializedProperty m_showColliders;
            
        [SerializeField] private int genSettingsOptionIndx = 1;
        
        readonly GUIContent[] modeOptions = {
            new GUIContent("Voxel"), 
            new GUIContent("Tetrahedron")
        };
    
        readonly GUIContent[] genSettingsOptions = {
            new GUIContent("Low Resolution"), 
            new GUIContent("Default Resoltion"),
            new GUIContent("High Resoluition"),
            new GUIContent("Custom...")    // always last
        };

        private int lowResolutionValue;
        private int defaultResolutionValue;
        private int highResolutionValue;
        
        void OnEnable()
        {
            lowResolutionValue = Parameters.LowResolution().resolution;
            defaultResolutionValue = Parameters.Default().resolution;
            highResolutionValue = Parameters.HighResolution().resolution;
                
            m_parameters = serializedObject.FindProperty("Params");

            m_parameters_maxConvexHulls = m_parameters.FindPropertyRelative("maxConvexHulls");
            m_parameters_resolution = m_parameters.FindPropertyRelative("resolution");
            m_parameters_concavity = m_parameters.FindPropertyRelative("concavity");
            m_parameters_planeDownsampling = m_parameters.FindPropertyRelative("planeDownsampling");
            m_parameters_convexhullApproximation = m_parameters.FindPropertyRelative("convexhullApproximation");
            m_parameters_alpha = m_parameters.FindPropertyRelative("alpha");
            m_parameters_beta = m_parameters.FindPropertyRelative("beta");
            m_parameters_pca = m_parameters.FindPropertyRelative("pca");
            m_parameters_mode = m_parameters.FindPropertyRelative("mode");
            m_parameters_maxNumVerticesPerCH = m_parameters.FindPropertyRelative("maxNumVerticesPerCH");
            m_parameters_minVolumePerCH = m_parameters.FindPropertyRelative("minVolumePerCH");

            m_asset = serializedObject.FindProperty("m_colliderAsset");
            m_isTrigger = serializedObject.FindProperty("m_isTrigger");
            m_material   = serializedObject.FindProperty("m_material");
            
            m_colliders = serializedObject.FindProperty("m_colliders");
            m_showColliders = serializedObject.FindProperty("m_showColliders");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            try
            {
                var objects = serializedObject.targetObjects;
                
                var nonConvexColliders = objects.Select(o => o as NonConvexColliderComponent)
                                                .Where(c => c != null)
                                                .ToArray();
                
                var gameObjects = nonConvexColliders.Select(c => c.gameObject).ToArray();
                
                var renderingMeshThis = gameObjects.Select(go => go.GetComponent<MeshFilter>()) // can be only one one the gameobject
                                                   .Where(mf => mf != null)
                                                   .Select(mf => mf.sharedMesh)
                                                   .Where(mesh => mesh != null)
                                                   .ToArray();
                
                var meshColliderThis = gameObjects.Select(go => go.GetComponents<MeshCollider>())
                                                  .Where(mf => mf.Length == 1)
                                                  .ToArray();

                var renderingMeshes = gameObjects.SelectMany(go => go.GetComponentsInChildren<MeshFilter>())
                                                 .Where(mf => mf != null)
                                                 .Select(mf => mf.sharedMesh)
                                                 .Where(mesh => mesh != null)
                                                 .ToArray();

                var colliders = gameObjects.SelectMany(go => go.GetComponentsInChildren<Collider>())
                                           .Where(c => c != null)
                                           .ToArray();
                
                var meshColliders = gameObjects.SelectMany(go => go.GetComponentsInChildren<MeshCollider>())
                                               .ToArray();

                var enabledMeshColliders = meshColliders.Where(c => c != null && c.enabled && c.sharedMesh != null)
                                                        .ToArray();
                
                var disabledOrBrokenMeshColliders = meshColliders.Where(c => c != null && (!c.enabled || c.sharedMesh == null))
                                                                 .ToArray();
                var disabledColliders = colliders.Where(c => c != null && !c.enabled)
                                                 .ToArray();

                var disabledOrBrokenCollidersCount = disabledColliders.Length + disabledOrBrokenMeshColliders.Length;
                
                foreach (var coll in nonConvexColliders)
                {
                    if (coll.Params.resolution == 0)
                        coll.Params = Parameters.Default();    
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_asset, true);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var coll in nonConvexColliders)
                    {
                        coll.SetPhysicsCollider(m_asset.objectReferenceValue as NonConvexColliderAsset);
                        EditorUtility.SetDirty(coll.gameObject);
                    }
                    serializedObject.UpdateIfRequiredOrScript();    // m_collider will be invalid
                }
                
                DrawParametersGUI(m_parameters);

                using (var _ = new UnityExtensions.DisableGroup(renderingMeshThis.Length == 0))
                {
                    var title = "Generate using render mesh on this GameObject";
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var coll in nonConvexColliders)
                        {
                            UnityExtensions.GenerateCollidersFromRenderingMeshThis(coll);
                            EditorUtility.SetDirty(coll);
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                }
                
                using (var _ = new UnityExtensions.DisableGroup(renderingMeshes.Length == 0))
                {
                    var title = "Generate using render meshes combined";
                    if (objects.Length == 1)
                    {
                        title += " (" + renderingMeshes.Length + " found)";
                    }
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var coll in nonConvexColliders)
                        {
                            UnityExtensions.GenerateCollidersFromRenderingMesh(coll);
                            EditorUtility.SetDirty(coll);
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                }
                
                using (var _ = new UnityExtensions.DisableGroup(meshColliderThis.Length == 0))
                {
                    var title = "Generate using enabled MeshCollider on this GameObject";
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var coll in nonConvexColliders)
                        {
                            UnityExtensions.GenerateCollidersFromMeshCollidersThis(coll);
                            EditorUtility.SetDirty(coll);
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                }

                using (var _ = new UnityExtensions.DisableGroup(enabledMeshColliders.Length == 0))
                {
                    var title = "Generate using enabled MeshColliders combined";
                    if (objects.Length == 1)
                    {
                        title += " (" + enabledMeshColliders.Length + " found)";
                    }
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var coll in nonConvexColliders)
                        {
                            UnityExtensions.GenerateCollidersFromMeshColliders(coll);
                            EditorUtility.SetDirty(coll);
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                }

                using (var _ = new UnityExtensions.DisableGroup(colliders.Length == 0))
                {
                    var title = "Delete all Colliders (including children)";
                    if (objects.Length == 1)
                    {
                        title += " (" + colliders.Length + " found)";
                    }
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var coll in nonConvexColliders)
                        {
                            coll.SetPhysicsCollider(null);
                            EditorUtility.SetDirty(coll);
                        }
                        foreach (var c in colliders)
                        {
                            if (c == null) continue;
                            var go = c.gameObject;
                            GameObject.DestroyImmediate(c, true);
                            EditorUtility.SetDirty(go);
                        }
                        serializedObject.UpdateIfRequiredOrScript();    // m_collider will be invalid
                        EditorGUIUtility.ExitGUI();
                    }
                }

                using (var _ = new UnityExtensions.DisableGroup(disabledOrBrokenCollidersCount == 0))
                {
                    var title = "Delete all disabled/empty Colliders";
                    if (objects.Length == 1)
                    {
                        title += " (" + disabledOrBrokenCollidersCount + " found)";
                    }
                    if (GUILayout.Button(new GUIContent(title)))
                    {
                        foreach (var c in disabledOrBrokenMeshColliders)
                        {
                            if (c == null) continue;
                            var go = c.gameObject;
                            GameObject.DestroyImmediate(c, true);
                            EditorUtility.SetDirty(go);
                        }
                        foreach (var c in disabledColliders)
                        {
                            if (c == null) continue;
                            var go = c.gameObject;
                            GameObject.DestroyImmediate(c, true);
                            EditorUtility.SetDirty(go);
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(string.Format("Has {0} convex colliders in control", m_colliders.arraySize), MessageType.Info);
                EditorGUI.BeginChangeCheck();
                m_showColliders.boolValue = EditorGUILayout.Toggle("Show Colliders", m_showColliders.boolValue);
                EditorGUILayout.Space();

                m_isTrigger.boolValue = EditorGUILayout.Toggle("Is Trigger", m_isTrigger.boolValue);
                m_material.objectReferenceValue = EditorGUILayout.ObjectField("Material", m_material.objectReferenceValue, typeof(PhysicMaterial), false);
                
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var t in serializedObject.targetObjects)
                    {
                        var nonConvexCollider = t as NonConvexColliderComponent;
                        EditorUtility.SetDirty(nonConvexCollider.gameObject);
                    }

                    if (serializedObject != null)
                        serializedObject.ApplyModifiedProperties();
                    
                    foreach (var t in serializedObject.targetObjects)
                    {
                        var nonConvexCollider = t as NonConvexColliderComponent;
                        nonConvexCollider.SyncState();
                        EditorUtility.SetDirty(nonConvexCollider.gameObject);
                    }
                }
            }
            finally
            {
                if (serializedObject != null)
                    serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawParametersGUI(SerializedProperty m_parameters)
        {
            if (genSettingsOptionIndx != 3)
            {
                if (m_parameters_resolution.intValue == lowResolutionValue)
                    genSettingsOptionIndx = 0;
                else if (m_parameters_resolution.intValue == defaultResolutionValue)
                    genSettingsOptionIndx = 1;
                else if (m_parameters_resolution.intValue == highResolutionValue)
                    genSettingsOptionIndx = 2;
                else
                    genSettingsOptionIndx = 3;    // this is custom, so set it to custom
            }

            EditorGUI.BeginChangeCheck();
            genSettingsOptionIndx = EditorGUILayout.Popup(new GUIContent("Generation settings"), genSettingsOptionIndx, genSettingsOptions);
            if (EditorGUI.EndChangeCheck())
            {
                if (genSettingsOptionIndx == 0)
                    m_parameters_resolution.intValue = lowResolutionValue;
                else if (genSettingsOptionIndx == 1)
                    m_parameters_resolution.intValue = defaultResolutionValue;
                else if (genSettingsOptionIndx == 2)
                    m_parameters_resolution.intValue = highResolutionValue;
            }   
            
            EditorGUILayout.PropertyField(m_parameters_resolution, true);
            EditorGUILayout.PropertyField(m_parameters_maxConvexHulls, true);

            if (genSettingsOptionIndx == 3)
            {
                EditorGUILayout.PropertyField(m_parameters_concavity, true);
                EditorGUILayout.PropertyField(m_parameters_planeDownsampling, true);
                EditorGUILayout.PropertyField(m_parameters_convexhullApproximation, true);
                EditorGUILayout.PropertyField(m_parameters_alpha, true);
                EditorGUILayout.PropertyField(m_parameters_beta, true);
                m_parameters_pca.boolValue = EditorGUILayout.Toggle(new GUIContent("Normalization", "Enable / disable normalizing the mesh before applying the convex decomposition. Default = False"), m_parameters_pca.boolValue);
                m_parameters_mode.intValue = EditorGUILayout.Popup(new GUIContent("Mode", "Voxel mode is default"), m_parameters_mode.intValue, modeOptions);
                EditorGUILayout.PropertyField(m_parameters_maxNumVerticesPerCH, true);
                EditorGUILayout.PropertyField(m_parameters_minVolumePerCH, true);
            }
        }
    }
}