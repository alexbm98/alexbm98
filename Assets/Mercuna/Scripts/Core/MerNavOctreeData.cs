// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mercuna
{
    [ExecuteInEditMode]
    public class MerNavOctreeData : ScriptableObject, ISerializationCallbackReceiver
    {
        public IntPtr m_pOctree = IntPtr.Zero;

        // Private as want this to be serialized on assembly reload but not to disk
        private long m_pSerializedPtr = 0;

        // Buffer used for saving octree when it is serialized
        public byte[] serializedData = new byte[0];

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fpWriteOctreeDataCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] inData, int dataSize);

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr CreateOctree();
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void DestroyOctree(IntPtr pOctree);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void SerializeOctree(IntPtr pOctree, [MarshalAs(UnmanagedType.FunctionPtr)]fpWriteOctreeDataCallback WriteSerializedData);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern void DeserializeOctree(IntPtr pOctree, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] inData, int dataSize);

        /////////////////////////// Creation and destruction ///////////////////////////

        private void Awake()
        {
            // Only null when a new octree is first added to level, otherwise will have been
            // created when loaded
            if (m_pOctree == IntPtr.Zero)
            {
                m_pOctree = CreateOctree();
            }
        }

        private void OnDestroy()
        {
            // Clean up octree
            DestroyOctree(m_pOctree);
        }

        /////////////////////////// Loading and saving ///////////////////////////

        // Callback used by C++ octree to write data into this object
        private void WriteSerializedData([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] inData, int dataSize)
        {
            serializedData = new byte[dataSize];
            if (dataSize > 0)
            {
                Array.Copy(inData, serializedData, dataSize);
            }
        }

        public void OnBeforeSerialize()
        {
            // Save pointer to C++ octree so that it can be restored on script reload
            m_pSerializedPtr = m_pOctree.ToInt64();

            // Convert C++ into byte array so that it can be saved
            SerializeOctree(m_pOctree, WriteSerializedData);
        }

        public void OnAfterDeserialize()
        {
            // If the saved pointer zero, then this is a load from disk
            if (m_pSerializedPtr == 0)
            {
                m_pOctree = CreateOctree();

                if (serializedData.Length > 0)
                {
                    DeserializeOctree(m_pOctree, serializedData, serializedData.Length);
                    serializedData = new byte[0];
                }
            }
            else
            {
                // Saved pointer not zero so existing C++ octree already exists, due to this being a script reload
                m_pOctree = new IntPtr(m_pSerializedPtr);
                m_pSerializedPtr = 0;
                serializedData = new byte[0];
            } 
        }
    }
}
