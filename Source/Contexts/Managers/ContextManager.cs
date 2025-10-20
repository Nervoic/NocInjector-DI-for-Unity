
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {
        public ContextInjectionManager InjectionManager { get; private set; }
        private readonly CallView _systemView = new();
        private void Awake()
        {
            InitializeInjector();
            
            InstallContexts();
            InjectToObjects();
        }

        private void InitializeInjector() => InjectionManager = new ContextInjectionManager(_systemView);
        private void InstallContexts()
        {
            var contexts = FindObjectsByType<GameContext>(FindObjectsSortMode.None);
            
            foreach (var context in contexts)
                context.InstallContext(_systemView);
        }

        private void InjectToObjects()
        {
            var injectableObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(g => !g.isStatic);
            
            foreach (var injectableObject in injectableObjects)
                InjectionManager.InjectTo(injectableObject);
        }
        
        
        /// <summary>
        /// Registers dependencies in the context on the specified GameObject if the context is available, and injects dependencies into components.
        /// </summary>
        /// <param name="contextObject">The object on which the context will be set and dependencies will be injected</param>
        public void InstallObject(GameObject contextObject)
        {
            var context = contextObject.GetComponent<Context>();
            context?.InstallContext(_systemView);
        }
    }
}
