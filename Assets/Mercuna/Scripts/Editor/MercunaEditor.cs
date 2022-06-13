// Copyright (C) 2018-2021 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(Mercuna))]
    public class MercunaEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mercuna Advanced Settings", EditorStyles.boldLabel);

            EditorGUIUtility.labelWidth = 150;

            String logTooltip =
                "When enabled all Mercuna log messages, including debug log messages, are output to a separate Mercuna.log file";

            bool bExtraLogging = EditorGUILayout.Toggle(new GUIContent("Extra logging", logTooltip), MerSettings.extraLogging);
            if (bExtraLogging != MerSettings.extraLogging)
            {
                MerSettings.extraLogging = bExtraLogging;
                Mercuna.SetExtraLogging(bExtraLogging);
            }

            if (bExtraLogging)
            {
                EditorGUI.indentLevel++;

                String flushTooltip = 
                    "Flush the Mercuna log file after every write. This can impact performance but can be useful " +
                    "to ensure that all log messages have been written to the file in event of a crash";

                bool bFlushLog = EditorGUILayout.Toggle(new GUIContent("Flush log on write", flushTooltip), MerSettings.flushLog);
                if (bFlushLog != MerSettings.flushLog)
                {
                    MerSettings.flushLog = bFlushLog;
                    Mercuna.SetFlushLogFile(bFlushLog);
                }

                EditorGUI.indentLevel--;
            }

            String errorTooltip =
                "Whether errors relating to pawn configuration should always be displayed on screen, even when " +
                "the General debug draw category is disabled.";

            bool bAlwaysShowErrors = EditorGUILayout.Toggle(new GUIContent("Always show errors", errorTooltip), MerSettings.alwaysShowErrors);
            if (bAlwaysShowErrors != MerSettings.alwaysShowErrors)
            {
                MerSettings.alwaysShowErrors = bAlwaysShowErrors;
                Mercuna.SetAlwaysShowErrors(bAlwaysShowErrors);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mercuna Info", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Version", Mercuna.GetVersion());
            EditorGUILayout.LabelField("Memory Used", String.Format("{0}KB", GetMemInUse() / 1024));
            EditorGUI.EndDisabledGroup();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern int GetMemInUse();
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern bool HasInstrumentedAlloc();
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        static extern void DumpAllocs();
    }
}
