// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using UnityEditor;
using UnityEngine;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(MercunaNavTesting))]
    public class MercunaNavTestingEditor : UnityEditor.Editor
    {
        private SerializedProperty m_otherObjectProp;
        private SerializedProperty m_radiusProp;
        private SerializedProperty m_drawModeProp;

        private void OnEnable()
        {
            m_otherObjectProp = serializedObject.FindProperty("end");
            m_radiusProp = serializedObject.FindProperty("radius");
            m_drawModeProp = serializedObject.FindProperty("drawMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MercunaNavTesting navTesting = (MercunaNavTesting)target;

            EditorGUILayout.PropertyField(m_otherObjectProp, new GUIContent("End"));

            float newRadius = EditorGUILayout.FloatField("Radius", m_radiusProp.floatValue);
            if (m_radiusProp.floatValue != newRadius)
            {
                m_radiusProp.floatValue = newRadius;
                navTesting.UpdatePath();
            }

            bool bPathExists;
            bool bPathPartial;
            float pathLength;
            int pathSections;
            float pathFindTime;
            int pathNodesUsed;
            bool bPathOutOfNodes;

            navTesting.GetPathInfo(out bPathExists, out bPathPartial, out pathLength, out pathSections, out pathFindTime, out pathNodesUsed, out bPathOutOfNodes);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Path Info", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Path Exists", bPathExists);
            EditorGUILayout.Toggle("Partial Path", bPathPartial);
            EditorGUILayout.LabelField("Total Length", pathLength.ToString("0.0") + "m");
            EditorGUILayout.LabelField("Number of Sections", pathSections.ToString());
            EditorGUILayout.LabelField("Find Time", pathFindTime.ToString("0.000") + "s");
            EditorGUILayout.LabelField("Nodes Used", pathNodesUsed.ToString());
            EditorGUILayout.Toggle("Out of Nodes", bPathOutOfNodes);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
