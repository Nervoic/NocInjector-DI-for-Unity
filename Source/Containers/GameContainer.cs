using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NocInjector.Calls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class GameContainer : Container
    {
        protected override Dictionary<DependencyInfo, Lifetime> DependenciesContainer { get; set; } = new();
        private readonly Dictionary<DependencyInfo, DependencyObject> _componentsContainer = new();
        
        private readonly CallView _systemView;

        internal GameContainer(CallView systemView)
        {
            _systemView = systemView;
        }
        
        public override void Register(Type typeToRegister, Lifetime lifetime)
        {
            var newObject = new DependencyInfo(typeToRegister);

            if (!DependenciesContainer.TryAdd(newObject, lifetime))
            {
                Debug.LogWarning($"Failed to add {typeToRegister.Name} to container. Component already exists in the container.");
                return;
            }
            
            if (typeToRegister.IsSubclassOf(typeof(Component))) 
                _componentsContainer.Add(newObject, null);
        }

        public void RegisterComponent(Type typeToRegister, GameObject gameObject)
        {
            var info = GetInfoByType(typeToRegister);
            
            if (info is null) 
                throw new Exception($"{typeToRegister.Name} is not registered in the container and cannot be registered as a component.");

            if (!gameObject.TryGetComponent<DependencyObject>(out var dependencyObject))
                dependencyObject = gameObject.AddComponent<DependencyObject>();
            
            dependencyObject.Initialize(this);
            _componentsContainer[info] = dependencyObject;
        }
        
        public override object Resolve(Type dependencyToResolve, string tag = null)
        {
            if (!Has(dependencyToResolve, tag))
            {
                if (dependencyToResolve.IsSubclassOf(typeof(Component)))
                    throw new Exception($"{dependencyToResolve.Name} is not registered in the container, or the GameObject that the component belonged to has been deleted.");
                
                throw new Exception($"{dependencyToResolve.Name} is not registered in the container. Register it before resolving");
            }

            var dependencyInfo = GetDependency(dependencyToResolve, tag);
            var lifetime = DependenciesContainer[dependencyInfo];
            
            ResolveByLifetime(lifetime, dependencyInfo, out var instance);
            
            if (!dependencyInfo.DependencyType.IsSubclassOf(typeof(Component)))
                OnInstanceResolved(instance);

            return instance;
        }

        private void ResolveByLifetime(Lifetime lifetime, DependencyInfo dependencyInfo, out object instance)
        {
            switch (lifetime)
            {
                case Lifetime.Singleton: 
                    ResolveSingleton(dependencyInfo, out instance);
                    break;
                case Lifetime.Transient:
                    ResolveTransient(dependencyInfo, out instance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), $"Lifetime value '{(int)lifetime}' is not supported. Please use a valid Lifetime enum value.");
            }
        }

        private void ResolveSingleton(DependencyInfo dependencyInfo, out object instance)
        {
            if (dependencyInfo.Instance is not null)
            {
                instance = dependencyInfo.Instance;
                return;
            }

            var dependencyType = dependencyInfo.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));

            if (isComponent)
            {
                if (!_componentsContainer.TryGetValue(dependencyInfo, out var dependencyObject))
                    throw new Exception($"{dependencyInfo.DependencyType} was not registered as a component and cannot be resolved");

                instance = dependencyObject.GetComponent(dependencyType);
            } 
            else 
                instance = Activator.CreateInstance(dependencyType);
            
            SetInstance(dependencyInfo, instance);
        }

        private void ResolveTransient(DependencyInfo dependencyInfo, out object instance)
        {
            var dependencyType = dependencyInfo.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));
            
            if (isComponent)
            {
                if (!_componentsContainer.TryGetValue(dependencyInfo, out var dependencyObject))
                    throw new Exception($"{dependencyInfo.DependencyType} was not registered as a component and cannot be resolved");

                instance = Object.Instantiate(dependencyObject).GetComponent(dependencyType);
            } 
            else 
                instance = Activator.CreateInstance(dependencyType);
        }

        public void DisposeObject(DependencyObject dependencyObject)
        {
            var objectDependencies = _componentsContainer.Where(c => c.Value == dependencyObject).Select(d => d.Key).ToList();
            
            foreach (var objectDependency in objectDependencies)
            {
                RemoveDependency(objectDependency);
            }
        }

        private void RemoveDependency(DependencyInfo dependencyInfo)
        {
            if (!Has(dependencyInfo.DependencyType, dependencyInfo.DependencyTag)) 
                return;
            
            DependenciesContainer.Remove(dependencyInfo);
            _componentsContainer.Remove(dependencyInfo);
        }
        
        private void OnInstanceResolved(object obj)
        {
            if (obj is null) return;

            var injectableMembers = obj.GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(Inject), true)).ToArray();
            
            _systemView.Call(new DependencyResolvedCall(injectableMembers, obj));
                
        }
        
        
    }
}
