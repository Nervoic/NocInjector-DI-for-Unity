using UnityEditor;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Editor window for creating NocInjector contexts from prefabs.
    /// </summary>
    public class ContextTool : EditorWindow
    {
        private const string SceneContextPath = "Assets/NocInjector/Prefabs/SceneContext.prefab";
        private const string ProjectContextPath = "Assets/NocInjector/Prefabs/ProjectContext.prefab";
        
        /// <summary>
        /// Creates a SceneContext prefab instance in the current scene.
        /// </summary>
        [MenuItem("Tools/NocInjector/Create SceneContext")]
        private static void CreateSceneContext()
        {
            GameObject sceneContextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SceneContextPath);

            if (sceneContextPrefab is not null)
            {
                GameObject sceneContext = (GameObject)PrefabUtility.InstantiatePrefab(sceneContextPrefab);
                
                Undo.RegisterCreatedObjectUndo(sceneContext, "Created SceneContext");
            }
        }
        
        /// <summary>
        /// Creates a ProjectContext prefab instance in the current scene.
        /// </summary>
        [MenuItem("Tools/NocInjector/Create ProjectContext")]
        private static void CreateProjectContext()
        {
            GameObject projectContextPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ProjectContextPath);

            if (projectContextPrefab is not null)
            {
                GameObject projectContext = (GameObject)PrefabUtility.InstantiatePrefab(projectContextPrefab);
                
                Undo.RegisterCreatedObjectUndo(projectContext, "Created ProjectContext");
            }
        }
    }
}
