// Copyright (C) 2018-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mercuna
{
    [DefaultExecutionOrder(-101)]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna Obstacle")]
    public class MercunaObstacle : MonoBehaviour
    {
        private IntPtr m_pObstacle = IntPtr.Zero;

        [Tooltip("Should the obstacle radius be automatically calculated from the collider")]
        public bool automaticRadius = true;

        [Tooltip("The obstacle radius for this GameObject"), Min(0f)]
        public float radius;

        void Start()
        {
            if (automaticRadius)
            {
                CalculateAutomaticRadius();
            }
        }

        public void CalculateAutomaticRadius()
        {
            Collider collider = GetComponent<Collider>();
            if (collider)
            {
                MerUtilities.GetMercunaEntityCenterAndRadiusFromCollider(collider, out _, out radius);
            }
            else
            {
                radius = 0.0f;
            }
        }

        private void OnEnable()
        {
            Mercuna.instance.EnsureInitialized();

            IntPtr entity = Mercuna.instance.GCCreateRef(gameObject);
            m_pObstacle = CreateMercunaObstacle(entity);
            Mercuna.instance.GCReleaseRef(entity);
        }

        private void OnDisable()
        {
            DestroyMercunaObstacle(m_pObstacle);
        }

        // Calls to native
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern IntPtr CreateMercunaObstacle(IntPtr entity);
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern void DestroyMercunaObstacle(IntPtr pNavigation);
    }
}