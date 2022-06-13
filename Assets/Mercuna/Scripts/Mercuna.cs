// Copyright (C) 2018-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Mercuna
{
    internal class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-105)]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna")]
    public class Mercuna : MerSingleton<Mercuna>
    {

#if (UNITY_EDITOR && !UNITY_EDITOR_OSX)
        public const CallingConvention MERCUNA_CALLING_CONVENTION = CallingConvention.Winapi;
        public const string MERCUNA_DLL_NAME = "MercunaUnityEditor";
#elif UNITY_SWITCH
        public const CallingConvention MERCUNA_CALLING_CONVENTION = CallingConvention.Cdecl;
        public const string MERCUNA_DLL_NAME = "__Internal";
#elif UNITY_ANDROID
        public const CallingConvention MERCUNA_CALLING_CONVENTION = CallingConvention.Cdecl;
        public const string MERCUNA_DLL_NAME = "MercunaUnity";
#else
        public const CallingConvention MERCUNA_CALLING_CONVENTION = CallingConvention.Winapi;
        public const string MERCUNA_DLL_NAME = "MercunaUnity";
#endif

        protected Mercuna() // guarantee this will be always a singleton only - can't use the constructor!
        {
            m_doJobCallback = new WaitCallback(DoJobInternal);
        } 

        [NonSerialized]
        private bool m_bInitialized = false;

        private MerLogger m_logger = new MerLogger();
        private MerDebugDraw m_debugDraw = new MerDebugDraw();
        private MerGeometryCollector m_geometryCollector = new MerGeometryCollector();

        private MerRefTable m_refTable = new MerRefTable();

        private bool m_bPaused = false;

        // Hold a static reference to the Mercuna singleton so that static methods (need for delegates)
        // can access it
        internal static Mercuna mercunaSingleton = null;

        /////////////////////////// Internal methods ///////////////////////////

        internal void EnsureInitialized()
        {
            if (!m_bInitialized)
            {
                Initialize();
            }
        }

        internal static void Log(ELogLevel level, string msg)
        {
            Log((int)level, msg);
        }

        /////////////////////////// Public methods ///////////////////////////

        public void SetDebugObject(GameObject obj)
        {
            if (obj != null)
            {
                IntPtr h = GCCreateRef(obj);
                SetDebugObject(h);
                GCReleaseRef(h);

                SetExtraLogging(true);
            }
            else
            {
                SetDebugObject(IntPtr.Zero);
            }
        }

        public GameObject GetDebugObject()
        {
            IntPtr namePtr = GetDebugObjectName();
            string name = Marshal.PtrToStringAnsi(namePtr);
            return GameObject.Find(name);
        }

        public static string GetVersion()
        {
            IntPtr versionPtr = GetMercuna3DNavigationVersion();
            return Marshal.PtrToStringAnsi(versionPtr);
        }

        /////////////////////////// Standard Unity events ///////////////////////////

        private void OnEnable()
        {
            EnsureInitialized();
        }

        private void OnDisable()
        {
            Deinitialize();
        }

        private void FixedUpdate()
        {
            // In edit mode is ticked via UnityEditor.EditorApplication.update
            if (Application.isPlaying)
            {
                Tick();
            }
        }

        private void OnDrawGizmos()
        {
            DebugDraw();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            //bPaused = !hasFocus;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            m_bPaused = pauseStatus;
        }

        /////////////////////////// Initialization code ///////////////////////////

        private void Initialize()
        {
            // Profiling always enabled on main thread.
            profilingEnabledForThread = true;

            bool bFirstInitialisation = IsFirstInitialization();

            if (bFirstInitialisation)
            {
                m_logger.ResetLogFile();
                MerSettings.Init();

                // Set the maximum number of jobs that Mercuna is allowed to run at once
                int workerThreads, completionPortThreads;
                ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
                SetMaxScheduledJobs(Math.Max(2, Math.Min(SystemInfo.processorCount, workerThreads) - 2));

                SetExtraLogging(MerSettings.extraLogging);
                SetFlushLogFile(MerSettings.flushLog);
                SetAlwaysShowErrors(MerSettings.alwaysShowErrors);
            }

            SetupInterfaces();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.update -= Tick;
                UnityEditor.EditorApplication.update += Tick;
                UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += OnSceneOpened;
            }
#endif

            // Reset the debug object
            SetDebugObject(GetDebugObject());

            m_bInitialized = true;

            if (bFirstInitialisation)
            {
                Log(ELogLevel.Always, "Initialized v" + GetVersion());
                Log(ELogLevel.Always, "Unity version: " + Application.unityVersion);
            }
            else
            {
                Log(ELogLevel.Info, "Reinitialized v" + GetVersion());
            }
        }

        private void Deinitialize()
        {
            Log(ELogLevel.Info, "Deinitialized");
            m_bInitialized = false;

            ClearCallbacks();
            m_logger.CloseLogFile();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Tick;
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened -= OnSceneOpened;
#endif
        }

        /////////////////////////// Interfaces to and from native ///////////////////////////
        // Calls to native from managed
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void SetCallbacks(ref ManagedCalls cb);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void ClearCallbacks();
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void SetMaxScheduledJobs(int maxScheduledJobs);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        public static extern void SetExtraLogging([MarshalAs(UnmanagedType.I1)] bool bEnabled);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        public static extern void SetFlushLogFile([MarshalAs(UnmanagedType.I1)] bool bEnabled);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        public static extern void SetAlwaysShowErrors([MarshalAs(UnmanagedType.I1)] bool bEnabled);

        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void Log(int level, string msg);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        [return: MarshalAs(UnmanagedType.I1)]
        static extern bool IsFirstInitialization();
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void Tick(float deltaTime, float time, [MarshalAs(UnmanagedType.I1)] bool bIsPaused);

        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        public static extern bool IsDebugDrawEnabled(string name);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        public static extern void ToggleDebugDraw(string name, [MarshalAs(UnmanagedType.I1)] bool bEntityScope);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void DebugDraw();
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void SetDebugObject(IntPtr debugObject);
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr GetDebugObjectName();
        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr GetMercuna3DNavigationVersion();

        [DllImport(MERCUNA_DLL_NAME, CallingConvention = MERCUNA_CALLING_CONVENTION)]
        static extern void DoJob(IntPtr pJob);

        // Calls from native to managed
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpLogToEngine(int level, string msg);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpLogToFile(int level, string msg, [MarshalAs(UnmanagedType.I1)] bool bFlush);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpFlushToLog();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int fpEntityGetID(IntPtr pEntity);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate int fpObjectGetHash(IntPtr pObject);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool fpObjectIsEqual(IntPtr pObject1, IntPtr pObject2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate string fpEntityGetName(IntPtr pEntity);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEntityGetVelocity(IntPtr pEntity, out MerVector vel);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEntityGetOrientation(IntPtr pEntity, out MerQuat orient);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEntityGetBounds(IntPtr pEntity, out MerVector center, out float radius);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEntityGetInfo(IntPtr pEntity, out MerVector position, out MerVector vel, out MerQuat orient, out float radius, out float obstacleRadius);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEntityGetUsageFlags(IntPtr pEntity, out uint requiredUsageFlags, out uint allowedUsageFlags);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawLine(MerVector start, MerVector end, MerColor color, float thickness);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawPolyline([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices, 
                                          MerColor color, float thickness);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawSphere(MerVector pos, float radius, MerColor color);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawCone(MerVector pos, float height, float radius, MerVector dir, MerColor color);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawMesh([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices,
                                      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] uint[] triIndices, int numTriIndices,
                                      MerColor color);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDrawText(string text, MerVector pos, float offset, MerColor color);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDraw2DText(string text, float x, float y, MerColor color);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDebugDrawGetViewInfo(out MerVector pos, out MerVector forward, out MerVector up);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpGetGeometryInfo(int numSources, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] pSources, MerAABB bounds,
                                        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] MerGeometryInfo[] infos);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpCollectTerrain(IntPtr pSource, MerAABB bounds,
                                       [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [Out] float[] pVertices, int maxVertexFloats);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate IntPtr fpCollectMesh(IntPtr pGeometryCollector, IntPtr pSource);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool fpGCIsPtrValid(IntPtr pEntity);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpGCAddRef(IntPtr pEntity, [MarshalAs(UnmanagedType.I1)] bool bWeak);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpGCReleaseRef(IntPtr pEntity, [MarshalAs(UnmanagedType.I1)] bool bWeak);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpRegisterProfileName(string sampleName, int sampleId);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate bool fpBeginProfile(int sampleId);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpEndProfile(int sampleId);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpNotifyMoveComplete(IntPtr moveCompleteCallbackId, [MarshalAs(UnmanagedType.I1)] bool bResult);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void fpDoWork(IntPtr pJob);

        [StructLayout(LayoutKind.Sequential)]
        struct ManagedCalls
        {
            public fpLogToEngine m_fpLogToEngine;
            public fpLogToFile m_fpLogToFile;
            public fpFlushToLog m_fpFlushToLog;

            public fpEntityGetID m_fpEntityGetID;
            public fpObjectGetHash m_fpObjectGetHash;
            public fpObjectIsEqual m_fpObjectIsEqual;

            public fpEntityGetName m_fpEntityGetName;
            public fpEntityGetVelocity m_fpEntityGetVelocity;
            public fpEntityGetOrientation m_fpEntityGetOrientation;
            public fpEntityGetBounds m_fpEntityGetBounds;
            public fpEntityGetInfo m_fpEntityGetInfo;
            public fpEntityGetUsageFlags m_fpEntityGetUsageFlags;

            public fpDebugDrawLine m_fpDebugDrawLine;
            public fpDebugDrawPolyline m_fpDebugDrawPolyline;
            public fpDebugDrawSphere m_fpDebugDrawSphere;
            public fpDebugDrawCone m_fpDebugDrawCone;
            public fpDebugDrawMesh m_fpDebugDrawMesh;
            public fpDrawText m_fpDrawText;
            public fpDraw2DText m_fpDraw2DText;
            public fpDebugDrawGetViewInfo m_fpDebugDrawGetViewInfo;

            public fpGetGeometryInfo m_fpGetGeometryInfo;
            public fpCollectTerrain m_fpCollectTerrain;
            public fpCollectMesh m_fpCollectMesh;

            public fpGCIsPtrValid m_fpGCIsPtrValid;
            public fpGCAddRef m_fpGCAddRef;
            public fpGCReleaseRef m_fpGCReleaseRef;

            public fpRegisterProfileName m_fpRegisterProfileName;
            public fpBeginProfile m_fpBeginProfile;
            public fpEndProfile m_fpEndProfile;

            public fpNotifyMoveComplete m_fpNotifyMoveComplete;

            public fpDoWork m_fpDoWork;
        }
        ManagedCalls managedCalls;


        /////////////////////////// Called by native interface ///////////////////////////

        int EntityGetID(IntPtr pEntity)
        {
            object o = m_refTable.Get(pEntity);
            if (o == null) return 0;
            GameObject go = (GameObject)o;
            return go.GetInstanceID();
        }

        int ObjectGetHash(IntPtr pObject)
        {
            object o = m_refTable.Get(pObject);
            if (o == null) return 0;
            return o.GetHashCode();
        }

        bool ObjectIsEqual(IntPtr pObject1, IntPtr pObject2)
        {
            object o1 = m_refTable.Get(pObject1);
            object o2 = m_refTable.Get(pObject2);
            return o1.Equals(o2);
        }

        string EntityGetName(IntPtr pEntity)
        {
            GameObject go = (GameObject)m_refTable.Get(pEntity);
            return go.name + go.GetInstanceID();
        }

        void EntityGetVelocity(IntPtr pEntity, out MerVector vel)
        {
            MercunaMoveController move = m_refTable.GetMoveController(pEntity);
            if (move)
            {
                vel = move.GetVelocity();
            }
            else
            {
                Rigidbody rigidbody = m_refTable.GetRigidbody(pEntity);
                if (rigidbody)
                {
                    vel = rigidbody.velocity;
                }
                else
                {
                    vel = Vector3.zero;
                }
            }
        }

        void EntityGetOrientation(IntPtr pEntity, out MerQuat orient)
        {
            GameObject go = (GameObject)m_refTable.Get(pEntity);
            Quaternion objOrient = go.transform.rotation;
            orient.w = objOrient.w;
            orient.x = objOrient.x;
            orient.y = objOrient.y;
            orient.z = objOrient.z;
        }

        void EntityGetBounds(IntPtr pEntity, out MerVector center, out float radius)
        {
            GameObject go = (GameObject)m_refTable.Get(pEntity);
            Collider collider = m_refTable.GetCollider(pEntity);
            Mercuna3DNavigation navigation = m_refTable.GetNavigation(pEntity);
            Vector3 uCenter;
            GetCenterAndRadius(go, collider, navigation, out uCenter, out radius);
            center = uCenter;
        }

        void EntityGetInfo(IntPtr pEntity, out MerVector position, out MerVector vel, out MerQuat orient, out float radius, out float obstacleRadius)
        {
            MerReference r = m_refTable.GetRef(pEntity);
            GameObject go = (GameObject)r.Get();


            GetCenterAndRadius(go, r.collider, r.navigation ,out var uCenter, out radius);
            position = uCenter;

            Quaternion objOrient = go.transform.rotation;
            orient.w = objOrient.w;
            orient.x = objOrient.x;
            orient.y = objOrient.y;
            orient.z = objOrient.z;

            Rigidbody rb = r.rigidbody;
            vel = rb ? rb.velocity : Vector3.zero;

            MercunaObstacle obstacle = r.obstacle;
            Debug.Assert(obstacle);
            obstacleRadius = obstacle.radius;
        }

        void EntityGetUsageFlags(IntPtr pEntity, out uint requiredUsageFlags, out uint allowedUsageFlags)
        {
            MerReference r = m_refTable.GetRef(pEntity);
            Mercuna3DNavigation navigationComponent = r.navigation;
            if (navigationComponent)
            {
                requiredUsageFlags = navigationComponent.usageFlags.requiredUsageFlags.GetPacked();
                allowedUsageFlags = navigationComponent.usageFlags.allowedUsageFlags.GetPacked();
            }
            else
            {
                requiredUsageFlags = 0;
                allowedUsageFlags = 0;
            }
        }

        internal static void GetCenterAndRadius(GameObject go, Collider collider, Mercuna3DNavigation navigation, out Vector3 center, out float radius)
        {

            if (collider)
            {
                MerUtilities.GetMercunaEntityCenterAndRadiusFromCollider(collider, out center, out radius);
            }
            else
            {
                center = go.transform.position;
                radius = 0.0f;
            }

            if (navigation)
            {
                radius = navigation.radius;
            }
        }

        public void GetGeometryInfo(int numSources, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] pSources, MerAABB bounds,
                                    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] MerGeometryInfo[] infos)
        {
            List<NavMeshBuildSource> buildSources = new List<NavMeshBuildSource>();
            foreach (var pSource in pSources)
            {
                buildSources.Add((NavMeshBuildSource)m_refTable.Get(pSource));
            }

            m_geometryCollector.GetSourceInfo(numSources, buildSources, bounds, infos);
        }

        public void CollectTerrain(IntPtr pSource, MerAABB bounds,
                                   [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [Out] float[] pVertices, int maxVertexFloats)
        {
            NavMeshBuildSource source = (NavMeshBuildSource)m_refTable.Get(pSource);
            m_geometryCollector.CollectTerrain(source, bounds, pVertices);
        }

        public IntPtr CollectMesh(IntPtr pGeometryCollector, IntPtr pSource)
        {
            NavMeshBuildSource source = (NavMeshBuildSource)m_refTable.Get(pSource);
            return m_geometryCollector.CollectMesh(pGeometryCollector, source);
        }

        bool GCIsPtrValid(IntPtr ptr)
        {
            return m_refTable.IsValid(ptr);
        }

        void GCAddRef(IntPtr ptr, bool bWeak)
        {
            m_refTable.AddRef(ptr, bWeak);
        }

        internal void GCReleaseRef(IntPtr ptr, bool bWeak = false)
        {
            m_refTable.ReleaseRef(ptr, bWeak);
        }

        internal IntPtr GCCreateRef(object o, bool bWeak = false)
        {
            return m_refTable.GetRefId(o, bWeak);
        }

        private Dictionary<int, CustomSampler> m_samplerLookup;
        private Dictionary<int, string> m_samplerReqs = new Dictionary<int, string>();
        private object m_samplerMutex = new object();

        [ThreadStaticAttribute]
        static bool profilingEnabledForThread;

        void RegisterProfileName(string sampleName, int sampleId)
        {
            lock(m_samplerMutex)
            {
                if (!m_samplerReqs.ContainsKey(sampleId))
                {
                    m_samplerReqs[sampleId] = sampleName;
                }
            }
        }

        bool BeginProfile(int sampleId)
        {
            Dictionary<int, CustomSampler> samplerLookup = m_samplerLookup;
            if (samplerLookup != null && samplerLookup.ContainsKey(sampleId))
            {
                if (!profilingEnabledForThread)
                {
                    Profiler.BeginThreadProfiling("Mercuna Workers", "Worker Thread");
                    profilingEnabledForThread = true;
                }
                samplerLookup[sampleId].Begin();
                return true;
            }
            else
            {
                return false;
            }
        }

        void EndProfile(int sampleId)
        {
            Dictionary<int, CustomSampler> samplerLookup = m_samplerLookup;
            if (samplerLookup != null && samplerLookup.ContainsKey(sampleId))
            {
                samplerLookup[sampleId].End();
            }
        }

        [System.Diagnostics.ConditionalAttribute("ENABLE_PROFILER")]
        void ProfilingStartJobThread()
        {
            profilingEnabledForThread = false;
        }

        [System.Diagnostics.ConditionalAttribute("ENABLE_PROFILER")]
        void ProfilingEndJobThread()
        {
            if (profilingEnabledForThread)
            {
                Profiler.EndThreadProfiling();
            }
        }

        void DoJobInternal(object pJob)
        {
            ProfilingStartJobThread();
            DoJob((IntPtr)pJob);
            ProfilingEndJobThread();
        }

        private readonly WaitCallback m_doJobCallback;

        void DoWork(IntPtr pJob)
        {
            object o = (object)pJob;
            ThreadPool.QueueUserWorkItem(m_doJobCallback, o);
        }

        //////////////////////////////////////////////////////////////////////////////////
        [MonoPInvokeCallback]
        private static void SLogToConsole(int level, string msg)
        {
            mercunaSingleton.m_logger.LogToConsole(level, msg);
        }
        [MonoPInvokeCallback]
        private static void SLogToFile(int level, string msg, bool bFlush)
        {
            mercunaSingleton.m_logger.LogToFile(level, msg, bFlush);
        }
        [MonoPInvokeCallback]
        private static void SFlushLogFile()
        {
            mercunaSingleton.m_logger.FlushLogFile();
        }
        [MonoPInvokeCallback]
        private static int SEntityGetID(IntPtr pEntity)
        {
            return mercunaSingleton.EntityGetID(pEntity);
        }
        [MonoPInvokeCallback]
        private static int SObjectGetHash(IntPtr pObject)
        {
            return mercunaSingleton.ObjectGetHash(pObject);
        }
        [MonoPInvokeCallback]
        private static bool SObjectIsEqual(IntPtr pObject1, IntPtr pObject2)
        {
            return mercunaSingleton.ObjectIsEqual(pObject1, pObject2);
        }
        [MonoPInvokeCallback]
        private static string SEntityGetName(IntPtr pEntity)
        {
            return mercunaSingleton.EntityGetName(pEntity);
        }
        [MonoPInvokeCallback]
        private static void SEntityGetVelocity(IntPtr pEntity, out MerVector vel)
        {
            mercunaSingleton.EntityGetVelocity(pEntity, out vel);
        }

        [MonoPInvokeCallback]
        private static void SEntityGetOrientation(IntPtr pEntity, out MerQuat orient)
        {
            mercunaSingleton.EntityGetOrientation(pEntity, out orient);
        }

        [MonoPInvokeCallback]
        private static void SEntityGetBounds(IntPtr pEntity, out MerVector center, out float radius)
        {
            mercunaSingleton.EntityGetBounds(pEntity, out center, out radius);
        }

        [MonoPInvokeCallback]
        private static void SEntityGetInfo(IntPtr pEntity, out MerVector position, out MerVector vel, out MerQuat orient, out float radius, out float obstacleRadius)
        {
            mercunaSingleton.EntityGetInfo(pEntity, out position, out vel, out orient, out radius, out obstacleRadius);
        }

        [MonoPInvokeCallback]
        private static void SEntityGetUsageFlags(IntPtr pEntity, out uint requiredUsageFlags, out uint allowedUsageFlags)
        {
            mercunaSingleton.EntityGetUsageFlags(pEntity, out requiredUsageFlags, out allowedUsageFlags);
        }

        [MonoPInvokeCallback]
        private static void SDebugDrawLine(MerVector start, MerVector end, MerColor color, float thickness)
        {
            mercunaSingleton.m_debugDraw.DrawLine(start, end, color, thickness);
        }

        [MonoPInvokeCallback]
        private static void SDebugDrawPolyline([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices, 
                                               MerColor color, float thickness)
        {
            mercunaSingleton.m_debugDraw.DrawPolyline(vertices, numVertices, color, thickness);
        }

        [MonoPInvokeCallback]
        private static void SDebugDrawSphere(MerVector pos, float radius, MerColor color)
        {
            mercunaSingleton.m_debugDraw.DrawSphere(pos, radius, color);
        }
        [MonoPInvokeCallback]
        private static void SDebugDrawCone(MerVector pos, float height, float radius, MerVector dir, MerColor color)
        {
            mercunaSingleton.m_debugDraw.DrawCone(pos, height, radius, dir, color);
        }

        [MonoPInvokeCallback]
        private static void SDebugDrawMesh([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices,
                               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] uint[] triIndices, int numTriIndices,
                               MerColor color)
        {
            mercunaSingleton.m_debugDraw.DrawMesh(vertices, numVertices, triIndices, numTriIndices, color);
        }

        [MonoPInvokeCallback]
        private static void SDebugDrawText(string text, MerVector pos, float offset, MerColor color)
        {
            mercunaSingleton.m_debugDraw.DrawText(text, pos, offset, color);
        }
        [MonoPInvokeCallback]
        private static void SDebugDraw2DText(string text, float x, float y, MerColor color)
        {
            mercunaSingleton.m_debugDraw.Draw2DText(text, x, y, color);
        }
        [MonoPInvokeCallback]
        private static void SDebugGetViewInfo(out MerVector position, out MerVector forward, out MerVector up)
        {
            mercunaSingleton.m_debugDraw.GetViewInfo(out position, out forward, out up);
        }

        [MonoPInvokeCallback]
        private static void SGetGeometryInfo(int numSources, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] pSources, MerAABB bounds,
                                            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] MerGeometryInfo[] infos)
        {
            mercunaSingleton.GetGeometryInfo(numSources, pSources, bounds, infos);
        }
        [MonoPInvokeCallback]
        private static void SCollectTerrain(IntPtr pSource, MerAABB bounds,
                                            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [Out] float[] pVertices, int maxVertexFloats)
        {
            mercunaSingleton.CollectTerrain(pSource, bounds, pVertices, maxVertexFloats);
        }
        [MonoPInvokeCallback]
        private static IntPtr SCollectMesh(IntPtr pGeometryCollector, IntPtr pSource)
        {
            return mercunaSingleton.CollectMesh(pGeometryCollector, pSource);
        }

        [MonoPInvokeCallback]
        private static bool SGCIsPtrValid(IntPtr ptr)
        {
            return mercunaSingleton.GCIsPtrValid(ptr);
        }

        [MonoPInvokeCallback]
        private static void SGCAddRef(IntPtr ptr, bool bWeak)
        {
            mercunaSingleton.GCAddRef(ptr, bWeak);
        }

        [MonoPInvokeCallback]
        private static void SGCReleaseRef(IntPtr ptr, bool bWeak = false)
        {
            mercunaSingleton.GCReleaseRef(ptr, bWeak);
        }
        [MonoPInvokeCallback]
        private static void SRegisterProfileName(string sampleName, int sampleId)
        {
            mercunaSingleton.RegisterProfileName(sampleName, sampleId);
        }
        [MonoPInvokeCallback]
        private static bool SBeginProfile(int sampleId)
        {
            return mercunaSingleton.BeginProfile(sampleId);
        }
        [MonoPInvokeCallback]
        private static void SEndProfile(int sampleId)
        {
            mercunaSingleton.EndProfile(sampleId);
        }
        [MonoPInvokeCallback]
        private static void SDoWork(IntPtr pJob)
        {
            mercunaSingleton.DoWork(pJob);
        }
        [MonoPInvokeCallback]
        private static void SNotifyMoveComplete(IntPtr moveCompleteCallbackId, [MarshalAs(UnmanagedType.I1)] bool bSuccess)
        {
            Mercuna3DNavigation.MoveCompleteDelegate fpMercunaMoveComplete = (Mercuna3DNavigation.MoveCompleteDelegate)mercunaSingleton.m_refTable.Get(moveCompleteCallbackId);
           
            fpMercunaMoveComplete(bSuccess);  // Call the functions
            mercunaSingleton.m_refTable.ReleaseRef(moveCompleteCallbackId, false); // Strong reference otherwise GC'd
        }
        public static IntPtr RegisterMoveCompleteDelegate(Mercuna3DNavigation.MoveCompleteDelegate fpMercunaMoveComplete)
        {
            return mercunaSingleton.m_refTable.GetRefId(fpMercunaMoveComplete, false); // Strong reference otherwise GC'd
        }

        //////////////////////////////////////////////////////////////////////////////////

        private void SetupInterfaces()
        {
            mercunaSingleton = this;

            // Setup managedCall
            managedCalls = new ManagedCalls();
            managedCalls.m_fpLogToEngine = SLogToConsole;
            managedCalls.m_fpLogToFile = SLogToFile;
            managedCalls.m_fpFlushToLog = SFlushLogFile;

            managedCalls.m_fpEntityGetID = SEntityGetID;
            managedCalls.m_fpObjectGetHash = SObjectGetHash;
            managedCalls.m_fpObjectIsEqual = SObjectIsEqual;

            managedCalls.m_fpEntityGetName = SEntityGetName;
            managedCalls.m_fpEntityGetVelocity = SEntityGetVelocity;
            managedCalls.m_fpEntityGetOrientation = SEntityGetOrientation;
            managedCalls.m_fpEntityGetBounds = SEntityGetBounds;
            managedCalls.m_fpEntityGetInfo = SEntityGetInfo;
            managedCalls.m_fpEntityGetUsageFlags = SEntityGetUsageFlags;

            managedCalls.m_fpDebugDrawLine = SDebugDrawLine;
            managedCalls.m_fpDebugDrawPolyline = SDebugDrawPolyline;
            managedCalls.m_fpDebugDrawSphere = SDebugDrawSphere;
            managedCalls.m_fpDebugDrawMesh = SDebugDrawMesh;
            managedCalls.m_fpDebugDrawCone = SDebugDrawCone;
            managedCalls.m_fpDrawText = SDebugDrawText;
            managedCalls.m_fpDraw2DText = SDebugDraw2DText;
            managedCalls.m_fpDebugDrawGetViewInfo = SDebugGetViewInfo;

            managedCalls.m_fpGetGeometryInfo = SGetGeometryInfo;
            managedCalls.m_fpCollectTerrain = SCollectTerrain;
            managedCalls.m_fpCollectMesh = SCollectMesh;

            managedCalls.m_fpGCIsPtrValid = SGCIsPtrValid;
            managedCalls.m_fpGCAddRef = SGCAddRef;
            managedCalls.m_fpGCReleaseRef = SGCReleaseRef;

            managedCalls.m_fpRegisterProfileName = SRegisterProfileName;
            managedCalls.m_fpBeginProfile = SBeginProfile;
            managedCalls.m_fpEndProfile = SEndProfile;

            managedCalls.m_fpNotifyMoveComplete = SNotifyMoveComplete;

            managedCalls.m_fpDoWork = SDoWork;

            // Set callbacks with Mercuna library
            SetCallbacks(ref managedCalls);
        }

        private void Tick()
        {
            lock(m_samplerMutex)
            {
                if (m_samplerReqs.Count > 0)
                {
                    Dictionary<int, CustomSampler> newSamplerLookup;
                    if (m_samplerLookup == null)
                    {
                        newSamplerLookup = new Dictionary<int, CustomSampler>();
                    }
                    else
                    {
                        newSamplerLookup = new Dictionary<int, CustomSampler>(m_samplerLookup);
                    }

                    foreach (var req in m_samplerReqs)
                    {
                        if (!newSamplerLookup.ContainsKey(req.Key))
                        {
                            newSamplerLookup.Add(req.Key, CustomSampler.Create(req.Value));
                        }
                    }
                    m_samplerReqs.Clear();

                    m_samplerLookup = newSamplerLookup;
                }
            }

            Tick(Time.deltaTime, Time.time, m_bPaused);
        }

#if UNITY_EDITOR
        private void OnSceneOpened(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            SetDebugObject(null);
        }
#endif
    }
}
