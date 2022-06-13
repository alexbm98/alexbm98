// Copyright (C) 2020-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Runtime.InteropServices;

namespace Mercuna
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-104)]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna Nav Octree")]
    public class MercunaNavOctree : MonoBehaviour
    {
        // These must match the enum in the native Mercuna library
        public enum EDebugDrawMode { Disabled, Unnavigable, Navigable, Both, Reachable, Pathfind };

        [Tooltip("Determines the side length of the cubes that make up the lowest level of the octree. Cells are considered unnavigable if there is any level geometry within them, so the larger the cell size the greater the error margin in the representation of the geometry."), Range(0.01f, 100.0f)]
        public float cellSize = 1.0f;

        [Tooltip("The minimum agent radius, as multiples of the cell size, to determine what navigation data is stored in the octree. Paths will never go closer to geometry than the minimum agent radius."), Range(1, 6)]
        public int minAgentRadius = 1;

        [Tooltip("The maximum agent radius, as multiples of the cell size, determine what navigation data is stored in the octree. The octree doesn't store data to allow paths to be found with more clearance from geometry than the maximum radius."), Range(1, 29)]
        public int maxAgentRadius = 2;

        [Header ("Geometry inclusion")]

        [Tooltip("Which layers to collect geometry from when building the octree")]
        public LayerMask layers = ~0;

        [Tooltip("Exclude objects with a rigidbody component")]
        public bool staticCollidersOnly = true;

        // Force the debug draw to be recalculated
        private bool forceDebugDrawDirty = false;

        private static MercunaNavOctree m_instance;
        public static MercunaNavOctree GetInstance()
        {
            return m_instance;//FindObjectOfType<MercunaNavOctree>();
        }

        public static MercunaNavOctree CreateOctreeInstance()
        {
            GameObject octreeObject = new GameObject();
            octreeObject.name = typeof(MercunaNavOctree).ToString();
            return octreeObject.AddComponent<MercunaNavOctree>();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BuildConfig
        {
            public float CellSize;
            public int MinPawnRadius;
            public int MaxPawnRadius;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NavVolume
        {
	        public IntPtr VolumePtr;
            public MerAABB Bounds;
            public uint LOD;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ModifierVolume
        {
	        public IntPtr VolumePtr;
            public MerAABB Bounds;
            public float CostMultiplier;
            public uint UsageTypes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GeometrySource
        {
            public IntPtr SourcePtr;
            public MerAABB Bounds;

            public GeometrySource(IntPtr source, Bounds bounds)
            {
                SourcePtr = source;
                Bounds = new MerAABB(bounds);
            }
        }

        public void Build()
        {
            // Find the nav volumes
            Bounds totalBounds = new Bounds();
            var navVolumes = FindObjectsOfType<MercunaNavVolume>();
            NavVolume[] navVolumesArray = new NavVolume[navVolumes.Length];

            for (int i = 0, iEnd = navVolumes.Length; i != iEnd; ++i)
            {
                Bounds bounds = navVolumes[i].GetBounds();
                navVolumesArray[i] = new NavVolume();
                navVolumesArray[i].VolumePtr = Mercuna.instance.GCCreateRef(navVolumes[i].gameObject);
                navVolumesArray[i].Bounds = new MerAABB(bounds);
                navVolumesArray[i].LOD = (uint)navVolumes[i].LOD;

                totalBounds.Encapsulate(bounds);
            }

            if (navVolumesArray.Length == 0)
            {
                Mercuna.Log(ELogLevel.Error, "Can't build Mercuna Octree: No nav volumes found");
                return;
            }

            // Find navseeds
            var seeds = FindObjectsOfType<MercunaNavSeed>();
            MerVector[] seedsArray = new MerVector[seeds.Length];

            for (int i = 0, iEnd = seeds.Length; i != iEnd; ++i)
            {
                seedsArray[i] = new MerVector(seeds[i].transform.position);
            }

            if (seedsArray.Length == 0)
            {
                Mercuna.Log(ELogLevel.Error, "Can't build Mercuna Octree: No nav seeds found");
                return;
            }

            // Find the modifier volumes
            var modifierVolumes = FindObjectsOfType<MercunaNavModifierVolume>();
            ModifierVolume[] modifierVolumeArray = new ModifierVolume[modifierVolumes.Length];
            for (int i = 0, iEnd = modifierVolumes.Length; i != iEnd; ++i)
            {
                modifierVolumeArray[i] = new ModifierVolume();
                modifierVolumeArray[i].VolumePtr = Mercuna.instance.GCCreateRef(modifierVolumes[i].gameObject);
                Bounds bounds = modifierVolumes[i].GetBounds();
                modifierVolumeArray[i].Bounds = new MerAABB(bounds);
                modifierVolumeArray[i].CostMultiplier = modifierVolumes[i].costMultiplier;
                modifierVolumeArray[i].UsageTypes = modifierVolumes[i].usageTypes.GetPacked();
            }

            // Find all sources within these bounds
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(totalBounds, layers, NavMeshCollectGeometry.PhysicsColliders, 0, markups, sources);

            GeometrySource[] sourceArray = new GeometrySource[sources.Count];
            int numSources = 0;
            for (int i = 0, iEnd = sources.Count; i < iEnd; i++)
            {
                Component sourceComponent = sources[i].component;

                if (sourceComponent.gameObject.GetComponent<Mercuna3DNavigation>() == null && 
                    sourceComponent.gameObject.GetComponent<MercunaObstacle>() == null)
                {
                    // If the staticCollidersOnly flag is set, then ignore any sources with a rigidbody component.
                    if (!(staticCollidersOnly && sourceComponent.GetComponent<Rigidbody>()))
                    {
                        IntPtr h = Mercuna.instance.GCCreateRef(sources[i]);
                        Matrix4x4 sourceTransform = sources[i].transform;
                        Vector3 position = new Vector3(sourceTransform[0, 3], sourceTransform[1, 3], sourceTransform[2, 3]);
                        Vector3 size = sources[i].size;
                        if (sources[i].shape == NavMeshBuildSourceShape.Terrain)
                        {
                            size = ((TerrainData) sources[i].sourceObject).size;
                            position += size * 0.5f;
                        }
                        else if (sources[i].shape == NavMeshBuildSourceShape.Mesh)
                        {
                            size = ((Mesh) sources[i].sourceObject).bounds.size;
                            position += sourceTransform.MultiplyVector(((Mesh) sources[i].sourceObject).bounds.center);
                        }

                        size = sourceTransform.MultiplyVector(size);
                        for (int j = 0; j < 3; ++j)
                        {
                            size[j] = Mathf.Abs(size[j]);
                        }

                        sourceArray[numSources++] = new GeometrySource(h, new Bounds(position, size));
                    }
                }
            }

            // Build octree
            BuildConfig config = new BuildConfig();
            config.CellSize = cellSize;
            config.MinPawnRadius = minAgentRadius;
            config.MaxPawnRadius = maxAgentRadius;

            m_fpBuildComplete = BuildCompleteCallback;
            BuildOctree(m_data.m_pOctree, config, navVolumesArray, navVolumesArray.Length, seedsArray, seedsArray.Length, modifierVolumeArray, modifierVolumeArray.Length, 
                        sourceArray, numSources, m_fpBuildComplete);

            for (int i = 0, iEnd = numSources; i < iEnd; i++)
            {
                Mercuna.instance.GCReleaseRef(sourceArray[i].SourcePtr);
            }
            
            for (int i = 0, iEnd = navVolumesArray.Length; i < iEnd; i++)
            {
                Mercuna.instance.GCReleaseRef(navVolumesArray[i].VolumePtr);
            }

            for (int i = 0, iEnd = modifierVolumeArray.Length; i < iEnd; i++)
            {
	            Mercuna.instance.GCReleaseRef(modifierVolumeArray[i].VolumePtr);
            }
        }

        public void CancelBuild()
        {
            CancelOctreeBuild(m_data.m_pOctree);
        }

        void BuildCompleteCallback(bool bSuccess)
        {
#if UNITY_EDITOR
            if (bSuccess && !Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
#endif
        }

        public bool IsBuilding()
        {
            return m_data.m_pOctree != IntPtr.Zero ? GetOctreeIsBuilding(m_data.m_pOctree) : false;
        }

        public float GetBuildProgress()
        {
            return m_data.m_pOctree != IntPtr.Zero ? GetOctreeBuildProgress(m_data.m_pOctree) : 1.0f;
        }

        private void Awake()
        {
            m_instance = this;

            // Create debug draw material
            m_material = new Material(Shader.Find("Mercuna/NavOctreeDebug"));
            m_materialTime = new Material(Shader.Find("Mercuna/NavOctreeTimeVaryingDebug"));

            if (m_data == null)
            {
                m_data = ScriptableObject.CreateInstance<MerNavOctreeData>();
            }
        }

        private void Start()
        {
            int mode = (int)MerSettings.octreeDebugDrawMode;
            if (mode != GetOctreeDebugDrawMode(m_data.m_pOctree))
            {
                SetOctreeDebugDrawMode(m_data.m_pOctree, mode);
            }
        }

        private void OnEnable()
        {
            Mercuna.instance.EnsureInitialized();
            Mercuna.Log(ELogLevel.Info, "Octree '" + gameObject.name + "' - Cell Size: " + cellSize.ToString("F2") + ", Min Agent Radius: " + minAgentRadius + ", Max agent radius: " + maxAgentRadius);
        }

        // Calls to native
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern bool BuildOctree(IntPtr pOctree, BuildConfig config,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] NavVolume[] navVolumes, int numNavVolumes,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] [In] MerVector[] seeds, int numSeeds,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 7)] [In] ModifierVolume[] modifierVolumes, int numModifierVolumes,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 9)] [In] GeometrySource[] sources, int numSources,
                                       fpBuildCompleteCallback BuildCompleteCallback);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void CancelOctreeBuild(IntPtr pOctree);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool GetOctreeIsBuilding(IntPtr pOctree);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern float GetOctreeBuildProgress(IntPtr pOctree);

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void SetOctreeDebugDrawMode(IntPtr pOctree, int mode);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern int GetOctreeDebugDrawMode(IntPtr pOctree);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool IsOctreeDebugDrawMeshDirty(IntPtr pOctree);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void GetOctreeDebugDrawMesh(IntPtr pOctree, [MarshalAs(UnmanagedType.FunctionPtr)]fpWriteDebugMeshDataCallback WriteDebugMeshData);

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool OctreeIsNavigable(IntPtr pOctree, MerVector pos, float navigationRadius);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool OctreeClampToNavigable(IntPtr pOctree, MerVector pos, float navigationRadius, out MerVector clampedPos);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool OctreeRaycast(IntPtr pOctree, MerVector from, MerVector to, float navigationRadius, out MerVector hitPos);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern bool OctreeIsReachable(IntPtr pOctree, MerVector from, MerVector to, float navigationRadius);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void OctreeIsReachableMulti(IntPtr pOctree, MerVector from, 
                                                          int numQueries, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [In] MerVector[] to,
                                                          int numResults, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] [Out] bool[] results,
                                                          float navigationRadius);

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr OctreeFindPathToLocation(IntPtr pOctree, MerVector start, MerVector end, float navigationRadius, [MarshalAs(UnmanagedType.I1)] bool bAllowPartial, [MarshalAs(UnmanagedType.I1)] bool bDebugDrawOn, MercunaPath.fpOnPathUpdate PathEventCallback);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr OctreeFindPathToObject(IntPtr pOctree, MerVector start, IntPtr goalObject, float navigationRadius, [MarshalAs(UnmanagedType.I1)] bool bAllowPartial, [MarshalAs(UnmanagedType.I1)] bool bDebugDrawOn, MercunaPath.fpOnPathUpdate PathEventCallback);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr OctreeFindSplineToLocation(IntPtr pOctree, MerVector start, MerVector end, float maxSpeed, float maxAcceleration, float navigationRadius, [MarshalAs(UnmanagedType.I1)] bool bAllowPartial, [MarshalAs(UnmanagedType.I1)] bool bDebugDrawOn, MercunaSmoothPath.fpOnPathUpdate PathEventCallback);

        // Callback from native to managed
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fpBuildCompleteCallback([MarshalAs(UnmanagedType.I1)] bool bSuccess);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fpWriteDebugMeshDataCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices,
                                                          [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] float[] texCoords, int numTexCoords,
                                                          [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] [In] uint[] triIndices, int numTriIndices);

        public IntPtr octreePtr
        {
            get
            {
                return m_data.m_pOctree;
            }
        }

        [HideInInspector]
        public MerNavOctreeData m_data;
        private fpBuildCompleteCallback m_fpBuildComplete;

        /////////////////////////// Debug draw ///////////////////////////

        private Material m_material;
        private Material m_materialTime;
        private List<Mesh> m_meshes = new List<Mesh>();

        public void WriteDebugMeshData([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] float[] texCoords, int numTexCoords,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] [In] uint[] triIndices, int numTriIndices)
        {
            Mesh mesh = new Mesh();

            Vector3[] meshVertices = new Vector3[numVertices / 3];
            Vector2[] meshUV = new Vector2[numTexCoords / 2];
            int[] meshTriangles = new int[numTriIndices];

            for (int i = 0, iEnd = numVertices / 3; i < iEnd; i++)
            {
                meshVertices[i].Set(vertices[3 * i], vertices[3 * i + 1], vertices[3 * i + 2]);
                meshUV[i].Set(texCoords[2 * i], texCoords[2 * i + 1]);
            }

            for (int i = 0; i < numTriIndices; i++)
            {
                meshTriangles[i] = (int)triIndices[i];
            }

            mesh.vertices = meshVertices;
            mesh.uv = meshUV;
            mesh.triangles = meshTriangles;

            m_meshes.Add(mesh);
        }

        public void ForceDebugDrawDirty()
        {
            forceDebugDrawDirty = true;
        }

        void RenderDebugDraw()
        {
            int mode = (int)MerSettings.octreeDebugDrawMode;

            if (m_data.m_pOctree == IntPtr.Zero)
            {
                return;
            }

            if (mode != GetOctreeDebugDrawMode(m_data.m_pOctree) || forceDebugDrawDirty)
            {
                forceDebugDrawDirty = false;
                SetOctreeDebugDrawMode(m_data.m_pOctree, mode);
            }

            if (IsOctreeDebugDrawMeshDirty(m_data.m_pOctree))
            {
                m_meshes = new List<Mesh>();
                GetOctreeDebugDrawMesh(m_data.m_pOctree, WriteDebugMeshData);
            }

            if (m_meshes.Count > 0)
            {
                if (mode < 4)
                {
                    m_material.SetPass(0);
                }
                else
                {
                    m_materialTime.SetPass(0);
                }

                // will make the mesh appear in the scene at origin position
                for (int i = 0; i < m_meshes.Count; ++i)
                {
                    Graphics.DrawMeshNow(m_meshes[i], Vector3.zero, Quaternion.identity);
                }
            }
        }

        void OnDrawGizmos()
        {
            RenderDebugDraw();
        }

        /////////////////////////// Navigability/Reachability ///////////////////////////

        public bool IsNavigable(Vector3 pos, float navigationRadius)
        {
            return OctreeIsNavigable(m_data.m_pOctree, pos, navigationRadius);
        }

        public bool ClampToNavigable(Vector3 pos, float navigationRadius, out Vector3 clampedPos)
        {
            MerVector merClampedPos;
            bool rv = OctreeClampToNavigable(m_data.m_pOctree, pos, navigationRadius, out merClampedPos);
            clampedPos = merClampedPos;
            return rv;
        }

        public bool Raycast(Vector3 from, Vector3 to, float navigationRadius, out Vector3 hitPos)
        {
            MerVector merHitPos;
            bool rv = OctreeRaycast(m_data.m_pOctree, from, to, navigationRadius, out merHitPos);
            hitPos = merHitPos;
            return rv;
        }

        public bool Raycast(Vector3 from, Vector3 to, float navigationRadius)
        {
            MerVector merHitPos;
            bool rv = OctreeRaycast(m_data.m_pOctree, from, to, navigationRadius, out merHitPos);
            return rv;
        }

        public bool IsReachable(Vector3 from, Vector3 to, float navigationRadius)
        {
            return OctreeIsReachable(m_data.m_pOctree, from, to, navigationRadius);
        }

        public void IsReachable(Vector3 from, Vector3[] to, float navigationRadius, bool[] result)
        {
            MerVector[] merTo = new MerVector[to.Length];
            for (int i = 0; i < to.Length; ++i)
            {
                merTo[i] = to[i];
            }
            OctreeIsReachableMulti(m_data.m_pOctree, from, merTo.Length, merTo, result.Length, result, navigationRadius);
        }

        public List<Vector3> IsReachable(Vector3 from, IList<Vector3> to, float navigationRadius)
        {
            MerVector[] merTo = new MerVector[to.Count];
            for (int i = 0; i < to.Count; ++i)
            {
                merTo[i] = to[i];
            }
            bool[] result = new bool[to.Count];
            OctreeIsReachableMulti(m_data.m_pOctree, from, merTo.Length, merTo, result.Length, result, navigationRadius);

            List<Vector3> reachablePoints = new List<Vector3>();
            for (int i = 0; i < to.Count; i++)
            {
                if (result[i])
                {
                    reachablePoints.Add(to[i]);
                }
            }
            return reachablePoints;
        }

        /////////////////////////// Path Finding ///////////////////////////

        public MercunaPath FindPathToLocation(Vector3 start, Vector3 end, float navigationRadius, bool bAllowPartial)
        {
            MercunaPath path = new MercunaPath();
            path.m_pPath = OctreeFindPathToLocation(m_data.m_pOctree, start, end, navigationRadius, bAllowPartial, false, path.m_fpOnPathUpdate);
            return path;
        }

        public MercunaPath FindPathToObject(Vector3 start, GameObject goalObject, float navigationRadius, bool bAllowPartial)
        {
            IntPtr h = Mercuna.instance.GCCreateRef(goalObject);
            MercunaPath path = new MercunaPath();
            path.m_pPath = OctreeFindPathToObject(m_data.m_pOctree, start, h, navigationRadius, bAllowPartial, false, path.m_fpOnPathUpdate);
            Mercuna.instance.GCReleaseRef(h);
            return path;
        }

        public MercunaSmoothPath FindSmoothPathToLocation(Vector3 start, Vector3 end, float maxSpeed, float maxAcceleration, float navigationRadius, bool bAllowPartial)
        {
            MercunaSmoothPath smoothPath = new MercunaSmoothPath();
            smoothPath.m_pPath = OctreeFindSplineToLocation(m_data.m_pOctree, start, end, maxSpeed, maxAcceleration, navigationRadius, bAllowPartial, false, smoothPath.m_fpOnPathUpdate);
            return smoothPath;
        }

        internal MercunaSmoothPath FindSmoothPathToLocationDebug(Vector3 start, Vector3 end, float maxSpeed, float maxAcceleration, float navigationRadius, bool bAllowPartial)
        {
            MercunaSmoothPath smoothPath = new MercunaSmoothPath();
            smoothPath.m_pPath = OctreeFindSplineToLocation(m_data.m_pOctree, start, end, maxSpeed, maxAcceleration, navigationRadius, bAllowPartial, true, smoothPath.m_fpOnPathUpdate);
            return smoothPath;
        }
    }
}
