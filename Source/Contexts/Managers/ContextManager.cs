
using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {
        private Injector _injector;
        
        private readonly List<GameObject> _injectedObjects = new();
        private readonly List<Context> _globalContexts = new();
        
        /// <summary>
        /// The CallView used by the library.
        /// </summary>
        public readonly CallView SystemView = new();
        public GameObject CurrentInjected { get; private set; }
        private void Awake()
        {
            InitializeInjector();
            
            InstallContexts();
            InjectToObjects();
        }

        private void InitializeInjector()
        {
            _injector = new Injector(SystemView);
        }

        private void InstallContexts()
        {
            var contexts = FindObjectsByType<Context>(FindObjectsSortMode.None);
            
            foreach (var context in contexts)
            {
                context.InstallContext(this);
                
                if (context is GameContext globalContext && globalContext.Lifetime != ContextLifetime.Object)
                    _globalContexts.Add(globalContext);
            }
        }

        private void InjectToObjects()
        {
            var injectableObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(g => !g.isStatic);
            foreach (var gameObj in injectableObjects)
            {
                InjectToObject(gameObj);
            }
        }
        
        
        /// <summary>
        /// Registers dependencies in the context on the specified GameObject if the context is available, and injects dependencies into components.
        /// </summary>
        /// <param name="obj">The object on which the context will be set and dependencies will be injected</param>
        public void InstallToObject(GameObject obj)
        {
            var context = obj.GetComponent<Context>();
            context?.InstallContext(this);
            
            InjectToObject(obj);
        }
        
        /// <summary>
        /// Returns a dependency from the first GameContext in which it is registered
        /// </summary>
        /// <param name="objectType">Dependency type</param>
        /// <param name="instanceTag">Dependency tag</param>
        /// <returns></returns>
        public object ResolveFromAnyContext(Type objectType, string instanceTag)
        {
            object instance = null;
            var context = _globalContexts.FirstOrDefault(c => c.Container.TryResolve(objectType, instanceTag, out instance));

            return context is null ? null : instance;
        }
        
        /// <summary>
        /// Returns a dependency from the first GameContext in which it is registered
        /// </summary>
        /// <param name="instanceTag">Dependency tag</param>
        /// <typeparam name="T">Dependency type</typeparam>
        /// <returns></returns>
        public T ResolveFromAnyContext<T>(string instanceTag)
        {
            T instance = default;
            var context = _globalContexts.FirstOrDefault(c => c.Container.TryResolve(instanceTag, out instance));

            return context is null ? default : instance;

        }

        private void InjectToObject(GameObject obj)
        {
            if (_injectedObjects.Contains(obj)) return;

            CurrentInjected = obj;
            
            _injector.InjectTo(obj);
            _injectedObjects.Add(obj);
        }
    }
}
