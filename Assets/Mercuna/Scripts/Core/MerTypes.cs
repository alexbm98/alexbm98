// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;
using System.Runtime.InteropServices;

namespace Mercuna
{
    /////////////////////////// Simple interfacing types ///////////////////////////

    [StructLayout(LayoutKind.Sequential)]
    public struct MerVector
    {
        public float x;
        public float y;
        public float z;

        public MerVector(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static implicit operator MerVector(Vector3 v)
        {
            return new MerVector(v);
        }

        public static implicit operator Vector3(MerVector v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MerQuat
    {
        public float w;
        public float x;
        public float y;
        public float z;

        public MerQuat(Quaternion quat)
        {
            w = quat.w;
            x = quat.x;
            y = quat.y;
            z = quat.z;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MerAABB
    {
        public float minx;
        public float miny;
        public float minz;
        public float maxx;
        public float maxy;
        public float maxz;

        public MerAABB(Bounds bounds)
        {
            minx = bounds.min.x; miny = bounds.min.y; minz = bounds.min.z;
            maxx = bounds.max.x; maxy = bounds.max.y; maxz = bounds.max.z;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MerColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public MerColor(float _r, float _g, float _b, float _a = 1.0f)
        {
            r = _r;
            g = _g;
            b = _b;
            a = _a;
        }

        public static implicit operator Color(MerColor c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MerGeometryInfo
    {
        public UnityEngine.AI.NavMeshBuildSourceShape shape;
        public Vector3 size;
        public Matrix4x4 transform;

        // The following are only set for terrains
        public int width;
        public int height;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct MerNativeArray
    {
        public unsafe void* ptr;
        public int len;
    }

    [System.Serializable]
    public class MerUsageTypes
    {
        public bool UsageType0 = false;
        public bool UsageType1 = false;
        public bool UsageType2 = false;
        public bool UsageType3 = false;
        public bool UsageType4 = false;

        public uint GetPacked()
        {
            uint res = 0;
            res |= (UsageType0 ? 1U : 0U) << 0;
            res |= (UsageType1 ? 1U : 0U) << 1;
            res |= (UsageType2 ? 1U : 0U) << 2;
            res |= (UsageType3 ? 1U : 0U) << 3;
            res |= (UsageType4 ? 1U : 0U) << 4;
            return res;
        }
    }

    [System.Serializable]
    public class MerAgentUsageFlags
    {
        public MerUsageTypes requiredUsageFlags;
        public MerUsageTypes allowedUsageFlags;
    } 
}
