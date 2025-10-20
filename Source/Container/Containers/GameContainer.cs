using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal class GameContainer : Container
    {
        protected override Dictionary<DependencyInfo, LifetimeImplementation> DependenciesContainer { get; set; } = new();
        protected override CallView SystemView { get; }
        protected override LifetimeFactory LifetimeFactory { get; } = new();

        internal GameContainer(CallView systemView)
        {
            SystemView = systemView;
        }
        
        public override DependencyInfo Register<TRegisteredType>(Lifetime lifetime)
        {
            var registeredType = typeof(TRegisteredType);
            
            var newDependency = new DependencyInfo(registeredType);
            var lifetimeImplementation = LifetimeFactory.GetLifetime(lifetime);

            DependenciesContainer.TryAdd(newDependency, lifetimeImplementation);

            return newDependency;
        }
        
        public override object Resolve(Type dependencyType, string tag)
        {
            MoreRegistrationsValidate(dependencyType, tag);

            if (dependencyType.IsArray)
                return ResolveArray(dependencyType, tag);
            
            if (!TryGetDependency(dependencyType, tag, out var dependency))
                ResolveValidate(dependencyType, tag);
            
            var lifetime = DependenciesContainer[dependency];

            lifetime.Resolve(dependency, SystemView, out var instance);
            return instance;
        }

        private object ResolveArray(Type arrayType, string tag)
        {
            var dependencyType = arrayType.GetElementType();
            
            if (dependencyType is null)
                return null;
            
            if (!TryGetDependencies(dependencyType, tag, out var dependencies))
                ResolveValidate(dependencyType, tag);
            
            var dependenciesInstances = new object[dependencies.Length];
            for (var i = 0; i < dependenciesInstances.Length; i++)
            {
                var currentDependency = dependencies[i];
                
                var lifetime = DependenciesContainer[currentDependency];

                lifetime.Resolve(currentDependency, SystemView, out var instance);
                dependenciesInstances[i] = instance;
            }
            
            var instancesArray = Array.CreateInstance(dependencyType, dependenciesInstances.Length);
            Array.Copy(dependenciesInstances, instancesArray, dependenciesInstances.Length);

            return instancesArray;
        }

        public void RegisterComponent(DependencyInfo dependency, GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<DependencyObject>(out var dependencyObject))
                dependencyObject = gameObject.AddComponent<DependencyObject>();
            
            dependency.GameObject = dependencyObject;
            dependencyObject.FollowDestroy<DependencyObjectDestroyedCall>(DisposeObject);
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
