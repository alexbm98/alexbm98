// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

namespace Mercuna
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(-100)]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna Nav Testing")]
    public class MercunaNavTesting : MonoBehaviour
    {
        public GameObject end;
        public float radius = 1.0f;
        private MercunaSmoothPath m_path = null;
        private Vector3 m_otherPosition;
        private Vector3 m_position;

        private void OnEnable()
        {
            Mercuna.instance.EnsureInitialized();
        }

        private void OnDisable()
        {
            if (m_path != null)
            {
                m_path.Destroy();
                m_path = null;
            }
        }

        void Update()
        {
            if (end == null && m_path != null) 
            {
                m_path.Destroy();
                m_path = null;
            }

            if (end != null && (m_path == null || m_position != transform.position || m_otherPosition != end.transform.position))
            {
                UpdatePath();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public void UpdatePath()
        {
            if (m_path != null)
            {
                m_path.Destroy();
            }

            m_position = transform.position;

            if (end)
            {
                m_otherPosition = end.transform.position;

                MercunaNavOctree octree = MercunaNavOctree.GetInstance();
                if (octree)
                {
                    m_path = octree.FindSmoothPathToLocationDebug(transform.position, end.transform.position, 1, 1, radius, true);
                    m_path.SetAsTest();
                } 
            }
        }

        public void GetPathInfo(out bool bPathExists, out bool bPathPartial, out float pathLength, out int pathSections,
                                out float pathFindTime, out int pathNodesUsed, out bool bPathOutOfNodes)
        {
            if (m_path != null && m_path.IsValid())
            {
                bPathExists = true;
                bPathPartial = m_path.IsPartial();
                pathLength = m_path.GetLength();
                pathSections = m_path.GetNumPoints() - 1;
                m_path.GetDebugInfo(out pathNodesUsed, out bPathOutOfNodes, out pathFindTime);
            }
            else
            {
                bPathExists = false;
                bPathPartial = false;
                pathLength = 0.0f;
                pathSections = 0;
                pathFindTime = 0.0f;
                pathNodesUsed = 0;
                bPathOutOfNodes = false;
            }
        }
    }
}