
using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {
        [Tooltip("Automatically registers the ContextManager in each context's container")]
        [SerializeField] private bool autoRegisterManager;

        private Injector _injector;
        private readonly List<GameObject> _injectedObjects = new();
        private readonly List<GameContext> _contexts = new();

        private readonly CallView _systemView = new();
        
        public GameObject CurrentInjected { get; private set; }
        private void Awake()
        {
            InstallContexts();
        }

        private void InstallContexts()
        {
            _injector = new Injector(_systemView);
            
            var contexts = FindObjectsByType<Context>(FindObjectsSortMode.None).OrderBy(c => c.GetType() == typeof(GameContext) ? 0 : 1);

            foreach (var context in contexts)
            {
                context.InstallContext(_systemView);
                
                if (autoRegisterManager) 
                    context.Container.Register<ContextManager>().AsComponentOn(gameObject);
                
                if (context.GetType() == typeof(GameContext))
                    _contexts.Add(context as GameContext);
            }
            
            InjectToComponents();
        }

        private void InjectToComponents()
        {
            var injectableObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(g => !g.isStatic);
            foreach (var obj in injectableObjects)
            {
                InjectToObject(obj);
            }
        }
        
        
        /// <summary>
        /// Registers dependencies in the context on the specified GameObject if the context is available, and injects dependencies into components.
        /// </summary>
        /// <param name="obj">The object on which the context will be set and dependencies will be injected</param>
        public void InstallToObject(GameObject obj)
        {
            var context = obj.GetComponent<Context>();
            context?.InstallContext(_systemView);
            
            InjectToObject(obj);
        }
        
        /// <summary>
        /// Registers dependencies from Installers on the specified GameObject and in the specified context, and injects dependencies into components.
        /// </summary>
        /// <param name="obj">GameObject whose Installers will be registered in the specified context, and dependencies will be injected.</param>
        /// <param name="context">Context in which the Installers will be registered</param>
        public void InstallFromObject(GameObject obj, Context context)
        {
            var installers = obj.GetComponents<Installer>();
            context.InstallNew(installers);

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
            var context = _contexts.FirstOrDefault(c => c.Container.TryResolve(objectType, instanceTag, out instance));

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
            var context = _contexts.FirstOrDefault(c => c.Container.TryResolve<T>(instanceTag, out instance));

            return context is null ? default : instance;

        }

        private void InjectToObject(GameObject obj)
        {
            if (_injectedObjects.Contains(obj)) return;

            CurrentInjected = obj;
            
            _injector.InjectToObject(obj);
            _injectedObjects.Add(obj);
        }
    }
}
