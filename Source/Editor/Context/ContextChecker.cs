using UnityEditor;
using UnityEngine;

namespace NocInjector
{
    [InitializeOnLoad]
    public class ContextChecker
    {
        static ContextChecker()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            var context = Object.FindAnyObjectByType<Context>();

            if (context is null) return;
            
            var contextManager = Object.FindAnyObjectByType<ContextManager>();

            if (contextManager is null)
                Debug.LogWarning($"Add {nameof(ContextManager)} to the scene for NocInjector to work");
            
        }
    }
}