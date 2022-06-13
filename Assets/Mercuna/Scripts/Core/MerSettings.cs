// Copyright (C) 2018-2021 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

namespace Mercuna
{
    public class MerSettings
    {
        public static void Init()
        {
            octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Disabled;
            lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Unnavigable;
        }

        //----------------------------- Runtime settings -----------------------------

        public static MercunaNavOctree.EDebugDrawMode octreeDebugDrawMode
        {
            get
            {
#if UNITY_EDITOR
                return (MercunaNavOctree.EDebugDrawMode)UnityEditor.EditorPrefs.GetInt("OctreeDebugDrawMode", 0);
#else
                return MercunaNavOctree.EDebugDrawMode.Disabled;
#endif
            }

            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetInt("OctreeDebugDrawMode", (int)value);
#endif
            }
        }

        public static MercunaNavOctree.EDebugDrawMode lastOctreeDebugDrawMode
        {
            get
            {
#if UNITY_EDITOR
                return (MercunaNavOctree.EDebugDrawMode)UnityEditor.EditorPrefs.GetInt("LastOctreeDebugDrawMode", 0);
#else
                return MercunaNavOctree.EDebugDrawMode.Disabled;
#endif
            }

            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetInt("LastOctreeDebugDrawMode", (int)value);
#endif
            }
        }

        //----------------------------- Persistent settings -----------------------------

        public static bool extraLogging
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorPrefs.GetBool("ExtraLogging", false);
#else
                return false;
#endif
            }

            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetBool("ExtraLogging", value);
#endif
            }
        }

        public static bool flushLog
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorPrefs.GetBool("FlushLogFileOnWrite", false);
#else
                return false;
#endif
            }

            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetBool("FlushLogFileOnWrite", value);
#endif
            }
        }

        public static bool alwaysShowErrors
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorPrefs.GetBool("AlwaysShowErrors", true);
#else
                return false;
#endif
            }

            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorPrefs.SetBool("AlwaysShowErrors", value);
#endif
            }
        }
    }
}