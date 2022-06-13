// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEditor;
using UnityEngine;

namespace Mercuna.Editor
{
    [CustomEditor(typeof(MercunaNavOctree))]
    public class MercunaNavOctreeEditor : UnityEditor.Editor
    {
        public void BuildCommand()
        {
            MercunaNavOctree navOctree = (MercunaNavOctree)target;
            navOctree.Build();

            EditorApplication.update -= BuildUpdate;
            EditorApplication.update += BuildUpdate;
        }

        private void BuildUpdate()
        {
            MercunaNavOctree navOctree = (MercunaNavOctree)target;

            if (navOctree.IsBuilding())
            {
                float buildProgess = navOctree.GetBuildProgress();
                float buildProgressPercent = 100.0f * buildProgess;
                if (EditorUtility.DisplayCancelableProgressBar("Mercuna Nav Octree", 
                    "Build " + buildProgressPercent.ToString("0.0") + "% complete", buildProgess))
                {
                    navOctree.CancelBuild();
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= BuildUpdate;
                SceneView.RepaintAll();
            }
        }

        [MenuItem("Tools/Mercuna/Build Octree", false, 20)]
        static void BuildOctree()
        {
            MercunaNavOctree octree = FindObjectOfType<MercunaNavOctree>();
            MercunaNavOctreeEditor octreeEditor = (MercunaNavOctreeEditor)UnityEditor.Editor.CreateEditor(octree);
            octreeEditor.BuildCommand();
        }

        [MenuItem("Tools/Mercuna/Build Octree", true)]
        static bool BuildOctreeValidate()
        {
            return FindObjectOfType<MercunaNavOctree>() != null;
        }
    }
}