using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEditor.AI;
using UnityEditorInternal;

#if UNITY_AI_NAVIGATION
using Unity.AI.Navigation;
#endif

namespace GuidanceLine
{
    /// <summary>
    /// Custom Editor window to make NavMesh setup easier
    /// Access via: Window → Guidance Line → NavMesh Quick Setup
    /// </summary>
    public class NavMeshQuickSetup : EditorWindow
    {
        private bool groundSetupFoldout = true;
        private bool wallSetupFoldout = true;
        private bool bakeSettingsFoldout = true;

        [MenuItem("Window/Guidance Line/NavMesh Quick Setup")]
        public static void ShowWindow()
        {
            GetWindow<NavMeshQuickSetup>("NavMesh Quick Setup");
        }

        void OnGUI()
        {
            GUILayout.Label("NavMesh Quick Setup for GuidanceLine", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This tool helps you quickly prepare your scene for automatic pathfinding.", MessageType.Info);

            EditorGUILayout.Space();

            // Ground Setup Section
            groundSetupFoldout = EditorGUILayout.Foldout(groundSetupFoldout, "Step 1: Setup Ground/Floor", true);
            if (groundSetupFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Select all ground/floor objects in the Hierarchy, then:");
                
                if (GUILayout.Button("Mark Selected as Walkable", GUILayout.Height(30)))
                {
                    MarkSelectedAsWalkable();
                }
                
                EditorGUILayout.HelpBox("This marks objects for NavMesh baking. Objects need to be Static.", MessageType.None);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // Wall Setup Section
            wallSetupFoldout = EditorGUILayout.Foldout(wallSetupFoldout, "Step 2: Setup Walls/Obstacles", true);
            if (wallSetupFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("Select all walls/obstacles in the Hierarchy, then:");
                
                if (GUILayout.Button("Mark Selected as Obstacles", GUILayout.Height(30)))
                {
                    MarkSelectedAsObstacles();
                }
                
                EditorGUILayout.HelpBox("This ensures the NavMesh avoids these objects.", MessageType.None);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // Bake Section
            bakeSettingsFoldout = EditorGUILayout.Foldout(bakeSettingsFoldout, "Step 3: Bake NavMesh", true);
            if (bakeSettingsFoldout)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                
#if UNITY_AI_NAVIGATION
                EditorGUILayout.LabelField("With AI Navigation Package:");
                
                if (GUILayout.Button("Bake NavMesh Now", GUILayout.Height(30)))
                {
                    BakeNavMesh();
                }
                
                EditorGUILayout.Space();
                
                if (GUILayout.Button("Apply Indoor Settings & Bake", GUILayout.Height(25)))
                {
                    SetIndoorNavMeshSettings();
                    BakeNavMesh();
                }
                
                if (GUILayout.Button("Apply Outdoor Settings & Bake", GUILayout.Height(25)))
                {
                    SetOutdoorNavMeshSettings();
                    BakeNavMesh();
                }
#else
                EditorGUILayout.HelpBox("Unity 6 requires the 'AI Navigation' package for NavMesh baking.", MessageType.Warning);
                
                if (GUILayout.Button("Install AI Navigation Package", GUILayout.Height(35)))
                {
                    InstallAINavigationPackage();
                }
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Manual Alternative:");
                if (GUILayout.Button("Open Package Manager", GUILayout.Height(25)))
                {
                    UnityEditor.PackageManager.UI.Window.Open("com.unity.ai.navigation");
                }
#endif

                EditorGUILayout.HelpBox("Unity 6 uses component-based NavMesh.\nInstall 'AI Navigation' package for automatic baking.", MessageType.Info);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // Package Detection Section
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Package Detection:", EditorStyles.boldLabel);
            
#if UNITY_AI_NAVIGATION
            EditorGUILayout.HelpBox("✓ AI Navigation package detected and enabled!", MessageType.Info);
#else
            if (IsAINavigationPackageInstalled())
            {
                EditorGUILayout.HelpBox("⚠ AI Navigation package is installed but not detected by scripts.\nClick button below to enable it.", MessageType.Warning);
                if (GUILayout.Button("Enable AI Navigation Support", GUILayout.Height(30)))
                {
                    EnableAINavigationDefine();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("AI Navigation package not installed.", MessageType.Info);
            }
#endif
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Status Section
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("NavMesh Status:", EditorStyles.boldLabel);
            
#if UNITY_AI_NAVIGATION
            // Check for NavMeshSurface
            NavMeshSurface[] surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
            if (surfaces.Length > 0)
            {
                EditorGUILayout.HelpBox($"✓ Found {surfaces.Length} NavMeshSurface(s) in scene", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("ℹ No NavMeshSurface found. Using scene-based baking.", MessageType.Info);
                if (GUILayout.Button("Add NavMeshSurface to Scene"))
                {
                    CreateNavMeshSurface();
                }
            }
#else
            EditorGUILayout.HelpBox("ℹ Using built-in NavMesh (scene-based baking).\n\nOptional: Install 'AI Navigation' package for NavMeshSurface support.", MessageType.Info);
            if (GUILayout.Button("Open Package Manager"))
            {
                UnityEditor.PackageManager.UI.Window.Open("com.unity.ai.navigation");
            }
#endif
            
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            if (triangulation.vertices.Length > 0)
            {
                EditorGUILayout.HelpBox($"✓ NavMesh is baked! ({triangulation.vertices.Length} vertices)", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("⚠ No NavMesh found. Please bake the NavMesh.", MessageType.Warning);
            }
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Validation Section
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Scene Validation:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Check Scene Setup"))
            {
                ValidateSceneSetup();
            }
            EditorGUILayout.EndVertical();
        }

        void MarkSelectedAsWalkable()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select GameObjects in the Hierarchy first.", "OK");
                return;
            }

            Undo.RecordObjects(selected, "Mark as Walkable");

            int count = 0;
            foreach (GameObject obj in selected)
            {
                // Set as static
                obj.isStatic = true;
                
#if UNITY_AI_NAVIGATION
                // Add or update NavMeshModifier component for walkable area
                NavMeshModifier modifier = obj.GetComponent<NavMeshModifier>();
                if (modifier == null)
                {
                    modifier = Undo.AddComponent<NavMeshModifier>(obj);
                }
                modifier.overrideArea = true;
                modifier.area = 0; // 0 = Walkable
#endif
                
                // Ensure it has a collider
                if (obj.GetComponent<Collider>() == null)
                {
                    if (obj.GetComponent<MeshFilter>() != null)
                    {
                        Undo.AddComponent<MeshCollider>(obj);
                    }
                    else
                    {
                        Undo.AddComponent<BoxCollider>(obj);
                    }
                }
                
                EditorUtility.SetDirty(obj);
                count++;
            }

#if UNITY_AI_NAVIGATION
            Debug.Log($"Marked {count} object(s) as Walkable (Static + NavMeshModifier)");
            EditorUtility.DisplayDialog("Success", $"Marked {count} object(s) as walkable.\n\nAdded NavMeshModifier components (area = Walkable).", "OK");
#else
            Debug.Log($"Marked {count} object(s) as Walkable (Static)");
            EditorUtility.DisplayDialog("Success", $"Marked {count} object(s) as walkable (Static).\n\nNote: Install 'AI Navigation' package for NavMeshModifier support.", "OK");
#endif
        }

        void MarkSelectedAsObstacles()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select GameObjects in the Hierarchy first.", "OK");
                return;
            }

            Undo.RecordObjects(selected, "Mark as Obstacles");

            int count = 0;
            foreach (GameObject obj in selected)
            {
                // Set as static
                obj.isStatic = true;
                
#if UNITY_AI_NAVIGATION
                // Add or update NavMeshModifier component for not walkable area
                NavMeshModifier modifier = obj.GetComponent<NavMeshModifier>();
                if (modifier == null)
                {
                    modifier = Undo.AddComponent<NavMeshModifier>(obj);
                }
                modifier.overrideArea = true;
                modifier.area = 1; // 1 = Not Walkable
#endif
                
                // Ensure it has a collider
                if (obj.GetComponent<Collider>() == null)
                {
                    Undo.AddComponent<BoxCollider>(obj);
                }
                
                EditorUtility.SetDirty(obj);
                count++;
            }

#if UNITY_AI_NAVIGATION
            Debug.Log($"Marked {count} object(s) as Obstacles (Static + NavMeshModifier with Not Walkable)");
            EditorUtility.DisplayDialog("Success", $"Marked {count} object(s) as obstacles.\n\nAdded NavMeshModifier components (area = Not Walkable).", "OK");
#else
            Debug.Log($"Marked {count} object(s) as Obstacles (Static)");
            EditorUtility.DisplayDialog("Success", $"Marked {count} object(s) as obstacles (Static).\n\nNote: Install 'AI Navigation' package for NavMeshModifier support.", "OK");
#endif
        }

        void SetIndoorNavMeshSettings()
        {
            // Settings are applied through the Navigation window's serialized object
            var navMeshSettingsObject = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
            if (navMeshSettingsObject != null)
            {
                SerializedObject serializedObject = new SerializedObject(navMeshSettingsObject);
                SerializedProperty agentTypeArray = serializedObject.FindProperty("m_Settings");
                if (agentTypeArray != null && agentTypeArray.arraySize > 0)
                {
                    SerializedProperty agentType = agentTypeArray.GetArrayElementAtIndex(0);
                    agentType.FindPropertyRelative("agentRadius").floatValue = 0.5f;
                    agentType.FindPropertyRelative("agentHeight").floatValue = 2.0f;
                    agentType.FindPropertyRelative("agentSlope").floatValue = 45.0f;
                    agentType.FindPropertyRelative("agentClimb").floatValue = 0.4f;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            Debug.Log("Applied Indoor NavMesh settings (Radius: 0.5, Height: 2.0, Slope: 45, Climb: 0.4)");
        }

        void SetOutdoorNavMeshSettings()
        {
            var navMeshSettingsObject = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshProjectSettings");
            if (navMeshSettingsObject != null)
            {
                SerializedObject serializedObject = new SerializedObject(navMeshSettingsObject);
                SerializedProperty agentTypeArray = serializedObject.FindProperty("m_Settings");
                if (agentTypeArray != null && agentTypeArray.arraySize > 0)
                {
                    SerializedProperty agentType = agentTypeArray.GetArrayElementAtIndex(0);
                    agentType.FindPropertyRelative("agentRadius").floatValue = 0.75f;
                    agentType.FindPropertyRelative("agentHeight").floatValue = 2.0f;
                    agentType.FindPropertyRelative("agentSlope").floatValue = 60.0f;
                    agentType.FindPropertyRelative("agentClimb").floatValue = 0.8f;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            Debug.Log("Applied Outdoor NavMesh settings (Radius: 0.75, Height: 2.0, Slope: 60, Climb: 0.8)");
        }

        void BakeNavMesh()
        {
#if UNITY_AI_NAVIGATION
            // Use the modern NavMesh surface baking
            NavMeshSurface[] surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
            
            if (surfaces.Length > 0)
            {
                // Bake all NavMeshSurface components
                foreach (var surface in surfaces)
                {
                    surface.BuildNavMesh();
                }
                Debug.Log($"Baked {surfaces.Length} NavMeshSurface(s)!");
                EditorUtility.DisplayDialog("Success", $"Baked {surfaces.Length} NavMeshSurface(s) successfully!", "OK");
                return;
            }
#endif
            
            // Fallback to opening Navigation window for manual baking
            EditorUtility.DisplayDialog("Manual Baking Required", 
                "Scene-based NavMesh baking requires the Navigation window.\n\n" +
                "Steps:\n" +
                "1. Window → AI → Navigation\n" +
                "2. Go to Bake tab\n" +
                "3. Click 'Bake' button\n\n" +
                "Or install 'AI Navigation' package for automatic baking.", 
                "OK");
            
            OpenNavigationWindow();
            Debug.Log("Please use Navigation window to bake NavMesh manually.");
        }
        
        void OpenNavigationWindow()
        {
            // Try different menu paths for different Unity versions
            if (!EditorApplication.ExecuteMenuItem("Window/AI/Navigation"))
            {
                if (!EditorApplication.ExecuteMenuItem("Window/Navigation"))
                {
                    EditorUtility.DisplayDialog("Navigation Window", 
                        "Could not open Navigation window automatically.\n\n" +
                        "Please open it manually:\n" +
                        "Window → AI → Navigation (Unity 2022+)\n" +
                        "or Window → Navigation (older versions)", 
                        "OK");
                }
            }
        }

        void ValidateSceneSetup()
        {
            string report = "=== Scene Validation Report ===\n\n";

            // Check for walkable surfaces
            GameObject[] staticObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int walkableCount = 0;
            int obstacleCount = 0;

            foreach (GameObject obj in staticObjects)
            {
                if (obj.isStatic)
                {
#if UNITY_AI_NAVIGATION
                    // Check NavMeshModifier component for area
                    NavMeshModifier modifier = obj.GetComponent<NavMeshModifier>();
                    if (modifier != null && modifier.overrideArea)
                    {
                        if (modifier.area == 0) // Walkable
                        {
                            walkableCount++;
                        }
                        else if (modifier.area == 1) // Not Walkable
                        {
                            obstacleCount++;
                        }
                    }
#else
                    // Count based on naming convention as fallback
                    if (obj.name.ToLower().Contains("ground") || obj.name.ToLower().Contains("floor"))
                    {
                        walkableCount++;
                    }
                    else if (obj.name.ToLower().Contains("wall") || obj.name.ToLower().Contains("obstacle"))
                    {
                        obstacleCount++;
                    }
#endif
                }
            }

            report += $"✓ Walkable objects found: {walkableCount}\n";
            report += $"✓ Obstacle objects found: {obstacleCount}\n\n";

            // Check NavMesh
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            if (triangulation.vertices.Length > 0)
            {
                report += $"✓ NavMesh is baked ({triangulation.vertices.Length} vertices)\n\n";
            }
            else
            {
                report += "⚠ NavMesh not baked yet\n\n";
            }

            // Check for GuidanceLineAuto
            GuidanceLineAuto guideLine = Object.FindFirstObjectByType<GuidanceLineAuto>();
            if (guideLine != null)
            {
                report += "✓ GuidanceLineAuto component found\n";
                report += $"  - Start Point: {(guideLine.startPoint != null ? "✓" : "✗ Missing")}\n";
                report += $"  - End Point: {(guideLine.endPoint != null ? "✓" : "✗ Missing")}\n";
            }
            else
            {
                report += "⚠ No GuidanceLineAuto component in scene\n";
            }

            Debug.Log(report);
            EditorUtility.DisplayDialog("Validation Report", report, "OK");
        }

        void CreateNavMeshSurface()
        {
#if UNITY_AI_NAVIGATION
            GameObject navMeshObj = new GameObject("NavMeshSurface");
            NavMeshSurface surface = navMeshObj.AddComponent<NavMeshSurface>();
            
            // Configure for typical indoor/AR use
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            
            Undo.RegisterCreatedObjectUndo(navMeshObj, "Create NavMeshSurface");
            Selection.activeGameObject = navMeshObj;
            
            Debug.Log("Created NavMeshSurface GameObject. Configure it and bake!");
            EditorUtility.DisplayDialog("NavMeshSurface Created", 
                "NavMeshSurface component has been added to the scene.\n\n" +
                "This is the modern way to bake NavMesh.\n\n" +
                "Now you can bake using the buttons in this window!", 
                "OK");
#else
            EditorUtility.DisplayDialog("AI Navigation Package Required", 
                "NavMeshSurface requires the 'AI Navigation' package.\n\n" +
                "Click 'Install AI Navigation Package' button to install it automatically.", 
                "OK");
#endif
        }

        void InstallAINavigationPackage()
        {
            bool install = EditorUtility.DisplayDialog(
                "Install AI Navigation Package?",
                "This will install the 'AI Navigation' package which provides:\n\n" +
                "• NavMeshSurface component\n" +
                "• NavMeshModifier component\n" +
                "• Automatic NavMesh baking\n" +
                "• Runtime baking support (perfect for AR)\n\n" +
                "Package: com.unity.ai.navigation\n\n" +
                "Install now?",
                "Install",
                "Cancel"
            );

            if (install)
            {
                Debug.Log("Installing AI Navigation package...");
                UnityEditor.PackageManager.Client.Add("com.unity.ai.navigation");
                
                EditorUtility.DisplayDialog(
                    "Package Installing",
                    "AI Navigation package is being installed.\n\n" +
                    "This may take a moment.\n\n" +
                    "After installation:\n" +
                    "1. Scripts will recompile automatically\n" +
                    "2. Close and reopen this window\n" +
                    "3. Use 'Bake NavMesh Now' button",
                    "OK"
                );
            }
        }

        bool IsAINavigationPackageInstalled()
        {
            var listRequest = UnityEditor.PackageManager.Client.List(true, false);
            while (!listRequest.IsCompleted)
            {
                System.Threading.Thread.Sleep(10);
            }
            
            if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == "com.unity.ai.navigation")
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        void EnableAINavigationDefine()
        {
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] definesArray);
            var defines = new System.Collections.Generic.List<string>(definesArray);
            
            const string DEFINE_SYMBOL = "UNITY_AI_NAVIGATION";
            
            if (!defines.Contains(DEFINE_SYMBOL))
            {
                defines.Add(DEFINE_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines.ToArray());
                
                Debug.Log($"Added '{DEFINE_SYMBOL}' scripting define symbol. Scripts will recompile...");
                
                EditorUtility.DisplayDialog(
                    "Scripting Define Added",
                    "The 'UNITY_AI_NAVIGATION' define symbol has been added.\n\n" +
                    "Unity is now recompiling scripts...\n\n" +
                    "After recompilation:\n" +
                    "1. Close and reopen this window\n" +
                    "2. You'll see 'Bake NavMesh Now' button\n" +
                    "3. Advanced features will be available!",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Already Enabled",
                    "The AI Navigation define symbol is already set.\n\n" +
                    "Try closing and reopening this window.",
                    "OK"
                );
            }
        }
    }
}
