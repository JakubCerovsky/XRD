using UnityEditor;
using UnityEngine;

namespace GuidanceLine
{
    /// <summary>
    /// Automatically adds the UNITY_AI_NAVIGATION define symbol when the AI Navigation package is detected
    /// </summary>
    [InitializeOnLoad]
    public class AINavigationDefineSymbol
    {
        private const string DEFINE_SYMBOL = "UNITY_AI_NAVIGATION";
        
        static AINavigationDefineSymbol()
        {
            // Check if AI Navigation package is installed
            bool hasAINavigation = HasPackage("com.unity.ai.navigation");
            
            // Get current build target using modern API
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            // Get current defines using modern API
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] definesArray);
            var defines = new System.Collections.Generic.List<string>(definesArray);
            
            bool hasDefine = defines.Contains(DEFINE_SYMBOL);
            
            if (hasAINavigation && !hasDefine)
            {
                // Add the define
                defines.Add(DEFINE_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines.ToArray());
                Debug.Log($"Added '{DEFINE_SYMBOL}' scripting define symbol. AI Navigation package detected!");
            }
            else if (!hasAINavigation && hasDefine)
            {
                // Remove the define if package was uninstalled
                defines.Remove(DEFINE_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines.ToArray());
                Debug.Log($"Removed '{DEFINE_SYMBOL}' scripting define symbol. AI Navigation package not found.");
            }
        }
        
        private static bool HasPackage(string packageId)
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
                    if (package.name == packageId)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
