// Copyright (C) 2020 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEditor;
using UnityEngine;

namespace Mercuna.Editor
{
    [CustomPropertyDrawer(typeof(MerAgentUsageFlags))]
    public class MerAgentUsageFlagsDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 7 : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                // Write the headings
                EditorGUI.LabelField(new Rect(position.x + 60, position.y + EditorGUIUtility.singleLineHeight, 60, 16), "Required");
                EditorGUI.LabelField(new Rect(position.x + 125, position.y + EditorGUIUtility.singleLineHeight, 60, 16), "Allowed");
                EditorGUI.LabelField(new Rect(position.x + 180, position.y + EditorGUIUtility.singleLineHeight, 80, 16), "Not Allowed");

                // Get the actual set of flags
                MerAgentUsageFlags agentUsageFlags = fieldInfo.GetValue(property.serializedObject.targetObject) as MerAgentUsageFlags;

                MerUsageTypes requiredUsageFlags = agentUsageFlags.requiredUsageFlags;
                MerUsageTypes allowedUsageFlags = agentUsageFlags.allowedUsageFlags;

                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel++;
                DrawUsageType(0, position, ref requiredUsageFlags.UsageType0, ref allowedUsageFlags.UsageType0);
                DrawUsageType(1, position, ref requiredUsageFlags.UsageType1, ref allowedUsageFlags.UsageType1);
                DrawUsageType(2, position, ref requiredUsageFlags.UsageType2, ref allowedUsageFlags.UsageType2);
                DrawUsageType(3, position, ref requiredUsageFlags.UsageType3, ref allowedUsageFlags.UsageType3);
                DrawUsageType(4, position, ref requiredUsageFlags.UsageType4, ref allowedUsageFlags.UsageType4);
                EditorGUI.indentLevel--;

                if (EditorGUI.EndChangeCheck())
                {
                    //fieldInfo.SetValue(property.serializedObject.targetObject, agentUsageFlags);
                    //property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            EditorGUI.EndProperty();
        }

        private void DrawUsageType(int rowNum, Rect position, ref bool requiredUsageType, ref bool allowedUsageType)
        {
            EditorGUI.LabelField(new Rect(position.x, position.y + (rowNum + 2) * EditorGUIUtility.singleLineHeight, 200, 16), "Type" + rowNum);

            int selected;
            if (requiredUsageType)
            {
                selected = 0;
            }
            else if (allowedUsageType)
            {
                selected = 1;
            }
            else
            {
                selected = 2;
            }

            string[] options = new string[] { "", "", "" };
            int newSelected = GUI.SelectionGrid(new Rect(position.x + 80, position.y + (rowNum + 2) * EditorGUIUtility.singleLineHeight, 180, 16), selected, options, options.Length, GUI.skin.toggle);

            if (newSelected == 0)
            {
                requiredUsageType = true;
                allowedUsageType = true;
            }
            else if (newSelected == 1)
            {
                requiredUsageType = false;
                allowedUsageType = true;
            }
            else
            {
                requiredUsageType = false;
                allowedUsageType = false;
            }
        }
    }
}