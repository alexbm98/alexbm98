// Copyright (C) 2018-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

namespace Mercuna
{
    [ExecuteInEditMode]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna Nav Volume")]
    public class MercunaNavVolume : MonoBehaviour
    {
        public enum LODEnum { Full, Half, Quarter, Eighth };

        [Tooltip("Level of detail to store for this nav volume")]
        public LODEnum LOD = LODEnum.Full;

        public Bounds GetBounds()
        {
            return new Bounds(transform.position, transform.localScale);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        void Start()
        {
            if (!MercunaNavOctree.GetInstance())
            {
                // No octree so create a new one
                MercunaNavOctree.CreateOctreeInstance();
            }
        }

        public static Bounds CalculateSceneBounds()
        {
            // Reasonable approximation for nav volume bounds should be bounding box encapsulating all colliders.
            Bounds allBounds = new Bounds();
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                Collider collider = obj.GetComponent<Collider>();
                if (collider)
                {
                    Bounds bounds = collider.bounds;
                    bounds.extents.Scale(obj.transform.localScale);  // TODO: Is this correct?!
                    allBounds.Encapsulate(bounds);
                }
            }
            return allBounds;
        }
    }
}
