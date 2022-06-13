// Copyright (C) 2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEditor;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(Mercuna3DNavigation)), CanEditMultipleObjects]
    public class Mercuna3DNavigationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Mercuna3DNavigation navigation = (Mercuna3DNavigation) target;
            bool automaticRadius = EditorGUILayout.Toggle(new GUIContent("Use automatic radius", "Whether to automatically determine the navigation radius from the object's collider"), navigation.automaticRadius);
            EditorGUI.BeginDisabledGroup(navigation.automaticRadius);
            float radius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the 3D navigation component"), navigation.radius);
            EditorGUI.EndDisabledGroup();

            radius = Mathf.Max(radius, 0.01f);

            if (automaticRadius != navigation.automaticRadius || Mathf.Abs(radius - navigation.radius) > 1e-6f)
            {
                navigation.automaticRadius = automaticRadius;
                if (!navigation.automaticRadius)
                {
                    navigation.radius = radius;
                }
                else
                {
                    navigation.CalculateAutomaticRadius();
                }
                EditorUtility.SetDirty(target);

                for (int i = 1; i < targets.Length; i++)
                {
                    Mercuna3DNavigation nav = (Mercuna3DNavigation) targets[i];
                    nav.radius = navigation.radius;
                    nav.automaticRadius = navigation.automaticRadius;
                }
            }
        }
    }
}