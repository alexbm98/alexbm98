// Copyright (C) 2020-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

namespace Mercuna
{
    /**
     * Defines a volume in which pathfinding costs can be increased and pathfinding
     * can be limited to specific navigation usage types.
     */
    [ExecuteInEditMode]
    [AddComponentMenu("Mercuna 3D Navigation/Mercuna Nav Modifier Volume")]
    public class MercunaNavModifierVolume : MonoBehaviour
    {
        [Tooltip("Amount by which to increase the cost of paths (multiplied by path distance) going through this volume.")]
        [Range(1.0f, 15.0f)]
        public float costMultiplier = 1.0f;

        [Tooltip("Set of flags that mark which usage types this modifier volume represents.")]
        public MerUsageTypes usageTypes;

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
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
