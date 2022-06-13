// Copyright (C) 2020-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;
using UnityEditor;

namespace Mercuna.Editor
{
    public class MercunaMenu : ScriptableObject
    {
        [MenuItem("Tools/Mercuna/Debug Draw/General", false, 1)]
        static void DebugDrawGeneral()
        {
            Mercuna.ToggleDebugDraw("General", false);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Debug Draw/General", true)]
        static bool DebugDrawGeneralValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Debug Draw/General", Mercuna.IsDebugDrawEnabled("General"));
            return true;
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Obstacles", false, 2)]
        static void DebugDrawObstacles()
        {
            Mercuna.ToggleDebugDraw("Obstacles", false);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Obstacles", true)]
        static bool DebugDrawGeneralObstacles()
        {
            Menu.SetChecked("Tools/Mercuna/Debug Draw/Obstacles", Mercuna.IsDebugDrawEnabled("Obstacles"));
            return true;
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Paths", false, 3)]
        static void DebugDrawPaths()
        {
            Mercuna.ToggleDebugDraw("Paths", false);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Paths", true)]
        static bool DebugDrawPathsValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Debug Draw/Paths", Mercuna.IsDebugDrawEnabled("Paths"));
            return true;
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Steering", false, 20)]
        static void DebugDrawSteering()
        {
            Mercuna.ToggleDebugDraw("Steering", true);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Steering", true)]
        static bool DebugDrawSteeringValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Debug Draw/Steering", Mercuna.IsDebugDrawEnabled("Steering"));
            return true;
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Avoidance", false, 21)]
        static void DebugDrawAvoidance()
        {
            Mercuna.ToggleDebugDraw("Avoidance", true);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Debug Draw/Avoidance", true)]
        static bool DebugDrawAvoidanceValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Debug Draw/Avoidance", Mercuna.IsDebugDrawEnabled("Avoidance"));
            return true;
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Enable debug draw %m", false, 2)]
        static void OctreeDebugDrawEnable()
        {
            MerSettings.octreeDebugDrawMode = (MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Disabled ?
                                                  MerSettings.lastOctreeDebugDrawMode : MercunaNavOctree.EDebugDrawMode.Disabled);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Enable debug draw %m", true)]
        static bool OctreeDebugDrawEnableValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Enable debug draw %m", MerSettings.octreeDebugDrawMode != MercunaNavOctree.EDebugDrawMode.Disabled);
            return true;
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Unnavigable cells", false, 20)]
        static void OctreeDebugDrawModeUnnavigable()
        {
            MerSettings.octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Unnavigable;
            MerSettings.lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Unnavigable;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Unnavigable cells", true)]
        static bool OctreeDebugDrawModeUnnavigableValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Unnavigable cells", MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Unnavigable);
            return true;
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Navigable cells", false, 21)]
        static void OctreeDebugDrawModeNavigable()
        {
            MerSettings.octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Navigable;
            MerSettings.lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Navigable;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Navigable cells", true)]
        static bool OctreeDebugDrawModeNavigableValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Navigable cells", MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Navigable);
            return true;
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Navigable and unnavigable cells", false, 22)]
        static void OctreeDebugDrawModeBoth()
        {
            MerSettings.octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Both;
            MerSettings.lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Both;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Navigable and unnavigable cells", true)]
        static bool OctreeDebugDrawModeBothValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Navigable and unnavigable cells", MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Both);
            return true;
        }
        
        [MenuItem("Tools/Mercuna/Octree Debug Draw/Last pathfind for debug actor", false, 40)]
        static void OctreeDebugDrawModePathfind()
        {
            MerSettings.octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Pathfind;
            MerSettings.lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Pathfind;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Last pathfind for debug actor", true)]
        static bool OctreeDebugDrawModePathfindValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Last pathfind for debug actor", MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Pathfind);
            return true;
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Last reachability query", false, 41)]
        static void OctreeDebugDrawModeReachable()
        {
            MerSettings.octreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Reachable;
            MerSettings.lastOctreeDebugDrawMode = MercunaNavOctree.EDebugDrawMode.Reachable;
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Octree Debug Draw/Last reachability query", true)]
        static bool OctreeDebugDrawModeReachableValidate()
        {
            Menu.SetChecked("Tools/Mercuna/Octree Debug Draw/Last reachability query", MerSettings.octreeDebugDrawMode == MercunaNavOctree.EDebugDrawMode.Reachable);
            return true;
        }

        [MenuItem("Tools/Mercuna/Add Mercuna To Scene", false, 21)]
        static void AddMercunaToScene()
        {
            Mercuna mercunaInstance = Mercuna.instance;
            if (FindObjectsOfType(typeof(MercunaNavOctree)).Length == 0)
            {
                GameObject navOctree = new GameObject("Nav Octree", typeof(MercunaNavOctree));
                navOctree.transform.SetParent(mercunaInstance.transform);
            }
            if (FindObjectsOfType(typeof(MercunaNavVolume)).Length == 0)
            {
                GameObject navVolume = new GameObject("Nav Volume", typeof(MercunaNavVolume));
                navVolume.transform.SetParent(mercunaInstance.transform);

                Bounds sceneBounds = MercunaNavVolume.CalculateSceneBounds();
                navVolume.transform.position = sceneBounds.center;
                navVolume.transform.localScale = sceneBounds.size;
            }
            if (FindObjectsOfType(typeof(MercunaNavSeed)).Length == 0)
            {
                GameObject navSeed = new GameObject("Nav Seed", typeof(MercunaNavSeed));
                navSeed.transform.SetParent(mercunaInstance.transform);

                // Find a MainCamera and put the initial seed there as this is likely to be in open space.
                foreach (Camera cam in FindObjectsOfType<Camera>())
                {
                    if (cam.gameObject.tag == "MainCamera")
                    {
                        navSeed.transform.position = cam.transform.position;
                        break;
                    }
                }
            }
        }

        [MenuItem("Tools/Mercuna/Add Mercuna To Scene", true)]
        static bool AddMercunaToSceneValidate()
        {
            if ((FindObjectsOfType(typeof(Mercuna)).Length >= 1) &&
                (FindObjectsOfType(typeof(MercunaNavOctree)).Length >= 1) &&
                (FindObjectsOfType(typeof(MercunaNavVolume)).Length >= 1) &&
                (FindObjectsOfType(typeof(MercunaNavSeed)).Length == 1))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [MenuItem("Tools/Mercuna/Set Debug Object", false, 22)]
        static void SetMercunaDebugObject()
        {
            if (Selection.activeGameObject)
            {
                Mercuna.instance.SetDebugObject(Selection.activeGameObject);
                foreach (MercunaNavOctree octree in FindObjectsOfType<MercunaNavOctree>())
                {
                    octree.ForceDebugDrawDirty();
                }

                SceneView.RepaintAll();
            }
        }

        [MenuItem("Tools/Mercuna/Set Debug Object", true)]
        static bool SetMercunaDebugObjectValidate()
        {
            return (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Mercuna3DNavigation>());
        }

        [MenuItem("Tools/Mercuna/Clear Debug Object", false, 22)]
        static void ClearMercunaDebugObject()
        {
            Mercuna.instance.SetDebugObject(null);
            foreach (MercunaNavOctree octree in FindObjectsOfType<MercunaNavOctree>())
            {
                octree.ForceDebugDrawDirty();
            }
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Mercuna/Clear Debug Object", true)]
        static bool ClearMercunaDebugObjectValidate()
        {
            return Mercuna.instance.GetDebugObject() != null;
        }

        [MenuItem("Tools/Mercuna/Open Log File Directory", false, 49)]
        static void OpenLogFile()
        {
            EditorUtility.RevealInFinder(MerLogger.logFilePath);
        }

        [MenuItem("Tools/Mercuna/Open Log File Directory", true)]
        static bool OpenLogFileValidate()
        {
            return MerSettings.extraLogging;
        }

        [MenuItem("Tools/Mercuna/Help", false, 50)]
        static void Help()
        {
            Application.OpenURL("https://mercuna.com/documentation/unity-user-guide/");
        }

        //---------------------------------------------------------------------

        [MenuItem("GameObject/Mercuna 3D Navigation/Mercuna Nav Volume", false, 10)]
        static void CreateMercunaNavVolume(MenuCommand menuCommand)
        {
            // Create game object
            GameObject navVolume = new GameObject("Nav Volume", typeof(MercunaNavVolume));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(navVolume, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(navVolume, "Create " + navVolume.name);
            Selection.activeObject = navVolume;
        }

        [MenuItem("GameObject/Mercuna 3D Navigation/Mercuna Nav Seed", false, 10)]
        static void CreateMercunaNavSeed(MenuCommand menuCommand)
        {
            // Create game object
            GameObject navSeed = new GameObject("Nav Seed", typeof(MercunaNavSeed));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(navSeed, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(navSeed, "Create " + navSeed.name);
            Selection.activeObject = navSeed;
        }

        [MenuItem("GameObject/Mercuna 3D Navigation/Mercuna Nav Modifier Volume", false, 10)]
        static void CreateMercunaNavModifierVolume(MenuCommand menuCommand)
        {
            // Create game object
            GameObject navVolume = new GameObject("Nav Modifier Volume", typeof(MercunaNavModifierVolume));

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(navVolume, menuCommand.context as GameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(navVolume, "Create " + navVolume.name);
            Selection.activeObject = navVolume;
        }
    }
}
