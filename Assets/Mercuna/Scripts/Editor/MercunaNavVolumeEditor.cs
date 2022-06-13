// Copyright (C) 2020-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.

using UnityEditor;
using UnityEngine;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(MercunaNavVolume))]
    class MercunaNavVolumeEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUIContent buttonContent = new GUIContent("Resize volume", "Resize volume to encompass all geometry in the scene");

            if (GUILayout.Button(buttonContent))
            {
                MercunaNavVolume navVolume = (MercunaNavVolume) target;
                Bounds sceneBounds = MercunaNavVolume.CalculateSceneBounds();
                navVolume.transform.position = sceneBounds.center;
                navVolume.transform.localScale = sceneBounds.size;
            }
        }
    }
}
