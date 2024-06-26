using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Plawius.NonConvexCollider
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Parameters
    {
        /// Maximum number of convex hulls. Default = 32. Range = [1 .. 128]
        [Tooltip("Maximum number of convex hulls. Default = 32. Range = [1 .. 128]")]
        [UnityEngine.Range(1, 128)]
        public int maxConvexHulls;

        /// Maximum number of voxels generated during the voxelization stage. Default = 50000. Range = [10000 .. 6400000]
        [Tooltip("Maximum number of voxels generated during the voxelization stage. Default = 50000. Range = [10000 .. 6400000]")]
        [UnityEngine.Range(10000, 6400000)]
        public int resolution; 
        
        /// Maximum concavity. Default = 0.0025. Range = [0.0 .. 1.0]
        [Tooltip("Maximum concavity. Default = 0.0025. Range = [0.0 .. 1.0]")]
        [UnityEngine.Range(0.0f, 1.0f)]
        public double concavity;
        
        /// Controls the granularity of the search for the "best" clipping plane. Default = 4. Range = [1 .. 16]
        [Tooltip("Controls the granularity of the search for the \"best\" clipping plane. Default = 4. Range = [1 .. 16]")]
        [UnityEngine.Range(1, 16)]
        public int planeDownsampling;
        
        /// Controls the precision of the convex - hull generation process during the clipping plane selection stage. Default = 4. Range = [1 .. 16]
        [Tooltip("Controls the precision of the convex - hull generation process during the clipping plane selection stage. Default = 4. Range = [1 .. 16]")]
        [UnityEngine.Range(1, 16)]
        public int convexhullApproximation;
        
        /// Controls the bias toward clipping along symmetry planes. Default = 0.05. Range = [0.0 .. 1.0]
        [Tooltip("Controls the bias toward clipping along symmetry planes. Default = 0.05. Range = [0.0 .. 1.0]")]
        [UnityEngine.Range(0.0f, 1.0f)]
        public double alpha;
        
        /// Controls the bias toward clipping along revolution axes. Default = 0.05. Range = [0.0 .. 1.0]
        [Tooltip("Controls the bias toward clipping along revolution axes. Default = 0.05. Range = [0.0 .. 1.0]")]
        [UnityEngine.Range(0.0f, 1.0f)]
        public double beta;
        
        /// Enable / disable normalizing the mesh before applying the convex decomposition. Default = 1. Range = [0 .. 1]
        [Tooltip("Enable / disable normalizing the mesh before applying the convex decomposition. Default = 1. Range = [0 .. 1]")]
        [UnityEngine.Range(0, 1)]
        public int pca;
        
        /// 0: voxel - based approximate convex decomposition, 1 : tetrahedron - based approximate convex decomposition. Default = 0. Range = [0 .. 1]
        [Tooltip("0: voxel - based approximate convex decomposition, 1 : tetrahedron - based approximate convex decomposition. Default = 0. Range = [0 .. 1]")]
        [UnityEngine.Range(0, 1)]
        public int mode;
        
        /// Controls the maximum number of triangles per convex - hull. Default = 64. Range = [4 .. 1024]
        [Tooltip("Controls the maximum number of triangles per convex - hull. Default = 64. Range = [4 .. 1024]")]
        [UnityEngine.Range(4, 1024)]
        public int maxNumVerticesPerCH;
        
        /// Controls the adaptive sampling of the generated convex - hulls. Default = 0.0001. Range = [0.0 .. 0.01]
        [Tooltip("Controls the adaptive sampling of the generated convex - hulls. Default = 0.0001. Range = [0.0 .. 0.01]")]
        [UnityEngine.Range(0.0f, 0.01f)]
        public double minVolumePerCH;

        public delegate void NativeCallbackDelegate(double overallProgress, double stageProgress, double operationProgress, 
                                                    IntPtr stage, IntPtr operation);

        [HideInInspector]
        public NativeCallbackDelegate callback;

        public static Parameters LowResolution()
        {
            return new Parameters
            {
                resolution = 10000,
                maxConvexHulls = 8,
                concavity = 0.0025,
                planeDownsampling = 4,
                convexhullApproximation = 4,
                alpha = 0.05,
                beta = 0.05,
                pca = 1,
                mode = 0,
                maxNumVerticesPerCH = 64,
                minVolumePerCH = 0.0001,
                callback = null
            };
        }
        
        public static Parameters Default()
        {
            return new Parameters
            {
                resolution = 50000,
                maxConvexHulls = 32,
                concavity = 0.0025,
                planeDownsampling = 4,
                convexhullApproximation = 4,
                alpha = 0.05,
                beta = 0.05,
                pca = 1,
                mode = 0,
                maxNumVerticesPerCH = 64,
                minVolumePerCH = 0.0001,
                callback = null
            };
        }

        public static Parameters HighResolution()
        {
            return new Parameters
            {
                resolution = 4000000,
                maxConvexHulls = 32,
                concavity = 0.0025,
                planeDownsampling = 4,
                convexhullApproximation = 4,
                alpha = 0.05,
                beta = 0.05,
                pca = 1,
                mode = 0,
                maxNumVerticesPerCH = 64,
                minVolumePerCH = 0.0001,
                callback = null
            };
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + maxConvexHulls.GetHashCode();
                hash = hash * 31 + resolution.GetHashCode();
                hash = hash * 31 + concavity.GetHashCode();
                hash = hash * 31 + planeDownsampling.GetHashCode();
                hash = hash * 31 + convexhullApproximation.GetHashCode();
                hash = hash * 31 + alpha.GetHashCode();
                hash = hash * 31 + beta.GetHashCode();
                hash = hash * 31 + pca.GetHashCode();
                hash = hash * 31 + mode.GetHashCode();
                hash = hash * 31 + maxNumVerticesPerCH.GetHashCode();
                hash = hash * 31 + minVolumePerCH.GetHashCode();
                return hash;
            }
        }
    }
    
    /// <summary>
    /// API to use from scripts. Editor only!
    /// </summary>
    public static class API
    {
        public static Mesh[] GenerateConvexMeshes(Mesh nonconvexMesh)
        {
            return GenerateConvexMeshes(nonconvexMesh, Parameters.Default(), null);
        }

        private static Action<float, string> _currentProgressCallback = null;
        
        [MonoPInvokeCallback(typeof(Parameters.NativeCallbackDelegate))]
        private static void NativeCallback(double overallProgress, double stageProgress, double operationProgress,
                                       IntPtr stagePtr, IntPtr operationPtr)
        {
            var stage = Marshal.PtrToStringAnsi(stagePtr);
            var operation = Marshal.PtrToStringAnsi(operationPtr);
            
            var progress01 = (float) (overallProgress * 0.01);
            
            var title = (stage != operation) ? string.Format("{0}. {1}", stage, operation) : stage;
            if (_currentProgressCallback != null)
                _currentProgressCallback(progress01, title);
        }

        public static Mesh[] GenerateConvexMeshes(Mesh nonconvexMesh, Parameters parameters, Action<float, string> progressCallback = null)
        {
            if (nonconvexMesh == null)
                throw new Exception("GenerateConvexMeshes called with nonconvexMesh == null");
            
            var progress = 0.0f;
            if (progressCallback != null)
                progressCallback(progress, "Initialization...");


            var ptriangles = nonconvexMesh.triangles;
            var ptrianglesLength = ptriangles.Length;
            var pvertices = nonconvexMesh.vertices;

            var outPoints = IntPtr.Zero;
            var outTriangles = IntPtr.Zero;
            var indexes = IntPtr.Zero;
            var indexesCnt = 0;
            
            progress = 0.1f;
            if (progressCallback != null)
                progressCallback(progress, "Generation is in progress, please wait...");

            if (parameters.maxConvexHulls == 0 || parameters.resolution == 0)
                parameters = Parameters.Default();
            parameters.callback = NativeCallback;

            _currentProgressCallback = progressCallback;

            var meshCount = -1;
            var gcVertices = GCHandle.Alloc(pvertices, GCHandleType.Pinned);
            var gcTriangles = GCHandle.Alloc(ptriangles, GCHandleType.Pinned);
            try
            {
                meshCount = Interop.GetMeshEx(gcVertices.AddrOfPinnedObject(), nonconvexMesh.vertexCount * 3,
                    gcTriangles.AddrOfPinnedObject(), ptrianglesLength,
                    out outPoints, out outTriangles, out indexes, out indexesCnt, parameters);
            }
            finally
            {
                if (gcVertices.IsAllocated) gcVertices.Free();
                if (gcTriangles.IsAllocated) gcTriangles.Free();
            }
            
            _currentProgressCallback = null;

            if (meshCount <= 0)
                throw new Exception("GenerateConvexMeshes failed, nothing is returned, please check your Mesh and/or your Parameters");

            if (meshCount > parameters.maxConvexHulls)
                throw new Exception("GenerateConvexMeshes failed, returned " + meshCount + " meshes returned, but maxConvexHulls == " + parameters.maxConvexHulls);

            var sanityCheck = (indexesCnt >= 0 && indexesCnt % 2 == 0) && (meshCount == indexesCnt / 2);
            if (sanityCheck == false)
                throw new Exception("GenerateConvexMeshes failed, data is corrupted");

            progress = 0.4f;
            if (progressCallback != null)
                progressCallback(progress, "Generation is done. Getting results...");
            var progressStep = (1.0f - progress);
            if (meshCount != 0)
                progressStep /= meshCount;

            try
            {
                var result = new Mesh[meshCount];
            
                var indxs = new int[indexesCnt];
                Marshal.Copy(indexes, indxs, 0, indexesCnt);

                var pointsStartIndex = 0;
                var trianglesStartIndex = 0;

                for (var meshIndex = 0; meshIndex < meshCount; meshIndex++)
                {
                    if (progressCallback != null)
                        progressCallback(progress, "Generation is done. Getting result " + (meshIndex + 1) + "/" + meshCount + "...");
                    progress += progressStep;

                    var pointCnt = indxs[meshIndex * 2];
                    var trianglesCnt = indxs[meshIndex * 2 + 1];

                    sanityCheck = (pointCnt >= 0) && (trianglesCnt >= 0) && (pointCnt % 3 == 0);
                    if (sanityCheck == false)
                        throw new Exception("GenerateConvexMeshes failed, one of the mesh data is corrupted");
            
                    var newPoints = new double[pointCnt];
                    var newTriangles = new int[trianglesCnt];

                    var pPoints = new IntPtr(outPoints.ToInt64() + sizeof(double) * pointsStartIndex);
                    Marshal.Copy(pPoints, newPoints, 0, pointCnt);
                    pointsStartIndex += pointCnt;
                
                    var pTriangles = new IntPtr(outTriangles.ToInt64() + sizeof(int) * trianglesStartIndex);
                    Marshal.Copy(pTriangles, newTriangles, 0, trianglesCnt);
                    trianglesStartIndex += trianglesCnt;

                    var tmp = new Vector3[pointCnt / 3];
                    for (var p = 0; p < (pointCnt / 3); p++)
                    {
                        tmp[p] = new Vector3((float)newPoints[p * 3], (float)newPoints[1 + p * 3], (float)newPoints[2 + p * 3]);
                    }

                    result[meshIndex] = new Mesh
                    {
                        vertices = tmp,
                        triangles = newTriangles,
                        name = "Generated convex submesh " + (meshIndex + 1)
                    };
                }
                
                if (progressCallback != null)
                    progressCallback(1.0f, "Generation is done. Cleaning up...");
                
                return result;
            }
            finally
            {
                Interop.ReleaseMemory(indexes);
                Interop.ReleaseMemory(outPoints);
                Interop.ReleaseMemory(outTriangles);
            }
        }
    }
}