using UnityEditor;
using UnityEngine;

namespace NocInjector
{
    [InitializeOnLoad]
    public sealed class ContextChecker
    {
        static ContextChecker()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            var context = Object.FindAnyObjectByType<Context>();

            if (context is null) return;
            
            var contextManagers = Object.FindObjectsByType<ContextManager>(FindObjectsSortMode.None);

            if (contextManagers is null || contextManagers.Length < 1)
            {
                Debug.LogWarning($"Add {nameof(ContextManager)} to the scene for NocInjector to work");
                return;
            }

            if (contextManagers.Length > 1)
                Debug.LogWarning($"More than 1 {nameof(ContextManager)} was detected on the stage. Please remove the extra {nameof(ContextManager)} for stable NocInjector operation");
            
        }
    }
}