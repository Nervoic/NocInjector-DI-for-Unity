using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal class GameContainer : Container
    {
        protected override Dictionary<DependencyInfo, ILifetimeImplementation> DependenciesContainer { get; set; } = new();
        protected override CallView SystemView { get; }
        protected override LifetimeFactory LifetimeFactory { get; } = new();

        internal GameContainer(CallView systemView)
        {
            SystemView = systemView;
        }
        
        public override DependencyInfo Register(Type typeToRegister, Lifetime lifetime)
        {
            var newDependency = new DependencyInfo(typeToRegister);
            var lifetimeImplementation = LifetimeFactory.GetLifetime(lifetime);
            
            if (!DependenciesContainer.TryAdd(newDependency, lifetimeImplementation))
                throw new Exception($"Failed to add {typeToRegister.Name} to container. Dependency already exists in the container.");

            return newDependency;
        }
        
        public override object Resolve(Type dependencyToResolve, string tag = null)
        {
            ResolveValidate(dependencyToResolve, tag);

            var dependency = GetDependency(dependencyToResolve, tag);
            var lifetime = DependenciesContainer[dependency];
            
            lifetime.Resolve(dependency, SystemView, out var instance);
            return instance;
        }
        
        public void RegisterComponent(DependencyInfo dependency, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<DependencyObject>(out var dependencyObject))
                dependencyObject = gameObject.AddComponent<DependencyObject>();
            
            dependency.GameObject = dependencyObject;
            dependencyObject.Follow<DependencyObjectDestroyedCall>(DisposeObject);
        }

        private void DisposeObject(DependencyObjectDestroyedCall call)
        {
            var dependencyObject = call.DependencyObject;
            
            var objectDependencies = DependenciesContainer
                .Where(dependency => dependency.Key.GameObject == dependencyObject)
                .Select(dependency => dependency.Key).ToList();

            foreach (var dependency in objectDependencies)
            {
                CancelRegister(dependency);
            }
        }
        
        
    }
}
