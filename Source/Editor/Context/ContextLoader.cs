using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NocInjector
{
    
    [InitializeOnLoad]
    public class ContextLoader
    {
        private static ContextManager _contextManager;
        
        static ContextLoader()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            UpdateContextManager();
        }

        private static void UpdateContextManager()
        {
            _contextManager ??= Object.FindAnyObjectByType<ContextManager>();

            if (_contextManager is null)
                return;

            var contexts = Object.FindObjectsByType<Context>(FindObjectsSortMode.None).Where(c => !_contextManager.Contexts.Contains(c));
            
            _contextManager.Contexts.AddRange(contexts);
        }
    }
}
