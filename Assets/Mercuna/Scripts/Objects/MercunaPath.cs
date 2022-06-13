// Copyright (C) 2018-2020 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mercuna
{
    public class MercunaPath
    {
        public enum PathUpdateType
        {
            Ready,
            Updated,
            Invalid
        };

        public delegate void PathUpdated(PathUpdateType type);
        public PathUpdated onPathUpdated;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fpOnPathUpdate(int pathEvent);

        internal IntPtr m_pPath;
        internal fpOnPathUpdate m_fpOnPathUpdate;

        internal MercunaPath()
        {
            m_fpOnPathUpdate = OnPathUpdate;
        }

        ~MercunaPath()
        {
            if (m_pPath != IntPtr.Zero)
            {
                DestroyPath(m_pPath);
            }
        }

        internal void Destroy()
        {
            DestroyPath(m_pPath);
            m_pPath = IntPtr.Zero;
        }

        internal void OnPathUpdate(int pathEvent)
        {
            if (onPathUpdated != null)
            {
                switch (pathEvent)
                {
                    case 0:
                        onPathUpdated.Invoke(PathUpdateType.Ready);
                        break;
                    case 1:
                        onPathUpdated.Invoke(PathUpdateType.Updated);
                        break;
                    case 2:
                        // This can happen very frequently so probably not of interest to pass on
                        break;
                    case 3:
                        onPathUpdated.Invoke(PathUpdateType.Invalid);
                        break;
                    default:
                        Mercuna.Log(ELogLevel.Error, String.Format("Unknown path event received:{0}", pathEvent));
                        break;
                }
            }
        }

        // Has the path finished being generated
        public bool IsReady()
        {
            return (m_pPath != IntPtr.Zero) && GetPathReady(m_pPath);
        }

        // Has a valid path been found - path that are in the process of generating will
        // also be considered invalid until they are Ready
        public bool IsValid()
        {
            return (m_pPath != IntPtr.Zero) && GetPathValid(m_pPath);
        }

        // Does the path reach the destination or only get part of the way there
        public bool IsPartial()
        {
            return (m_pPath != IntPtr.Zero) && GetPathPartial(m_pPath);
        }

        // The total length of the path
        public float GetLength()
        {
            return (m_pPath != IntPtr.Zero) ? GetPathLength(m_pPath) : 0.0f;
        }

        // Get position of point i
        public Vector3 GetPoint(int idx)
        {
            return (m_pPath != IntPtr.Zero) ? (Vector3)GetPathPoint(m_pPath, idx) : new Vector3();
        }

        // The number of points that make up the path
        public int GetNumPoints()
        {
            return (m_pPath != IntPtr.Zero) ? GetPathNumPoints(m_pPath) : 0;
        }

        // Get information about the path search used to create this path
        public void GetDebugInfo(out int nodesUsed, out bool bOutOfNodes, out float findTime)
        {
            GetPathDebugInfo(m_pPath, out nodesUsed, out bOutOfNodes, out findTime);
        }

        internal void SetAsTest()
        {
            SetPathAsTest(m_pPath, true);
        }

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern void DestroyPath(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern void SetPathAsTest(IntPtr pPath, [MarshalAs(UnmanagedType.I1)] bool bTest);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern bool GetPathReady(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern bool GetPathValid(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern bool GetPathPartial(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern float GetPathLength(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern MerVector GetPathPoint(IntPtr pPath, int idx);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern int GetPathNumPoints(IntPtr pPath);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern void GetPathDebugInfo(IntPtr pPath, out int nodesUsed, [MarshalAs(UnmanagedType.I1)] out bool bOutOfNodes, out float queryTime);
    }
}
