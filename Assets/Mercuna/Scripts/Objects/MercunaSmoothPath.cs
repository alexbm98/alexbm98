// Copyright (C) 2018-2020 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mercuna
{
    public class MercunaSmoothPath : MercunaPath
    {
        // Get the position along the path curve, where 0 <= length <= path length
        public Vector3 GetPosition(float length)
        {
            return (m_pPath != IntPtr.Zero) ? (Vector3)GetSmoothPathPosition(m_pPath, length) : new Vector3();
        }

        // Get the tangent along the path curve, where 0 <= length <= path length
        public Vector3 GetTangent(float length)
        {
            return (m_pPath != IntPtr.Zero) ? (Vector3)GetSmoothPathTangent(m_pPath, length) : new Vector3();
        }

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern MerVector GetSmoothPathPosition(IntPtr pPath, float length);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern MerVector GetSmoothPathTangent(IntPtr pPath, float length);
    }
}
