// Copyright (C) 2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

using System;
using UnityEngine;
using UnityEditor;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(MercunaObstacle)), CanEditMultipleObjects]
    public class MercunaObstacleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MercunaObstacle obstacle = (MercunaObstacle) target;
            bool automaticRadius = EditorGUILayout.Toggle(new GUIContent("Use automatic radius", "Whether the obstacle radius should automatically be calculated from the collider"), obstacle.automaticRadius);
            EditorGUI.BeginDisabledGroup(obstacle.automaticRadius);
            float radius = EditorGUILayout.FloatField(new GUIContent("Obstacle Radius", "The obstacle radius for this GameObject"), obstacle.radius);
            EditorGUI.EndDisabledGroup();

            radius = Mathf.Max(radius, 0.01f);

            if (automaticRadius != obstacle.automaticRadius || Mathf.Abs(radius - obstacle.radius) > 1e-6f)
            {
                obstacle.automaticRadius = automaticRadius;
                if (!obstacle.automaticRadius)
                {
                    obstacle.radius = radius;
                }
                else
                {
                    obstacle.CalculateAutomaticRadius();
                }
                EditorUtility.SetDirty(target);

                for (int i = 1; i < targets.Length; i++)
                {
                    MercunaObstacle o = (MercunaObstacle) targets[i];
                    o.automaticRadius = obstacle.automaticRadius;
                    o.radius = obstacle.radius;
                    EditorUtility.SetDirty(targets[i]);
                }
            }
        }
    }
}
