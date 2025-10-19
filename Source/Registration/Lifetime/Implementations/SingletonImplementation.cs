using System;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal class SingletonImplementation : ILifetimeImplementation
    {
        public Lifetime Lifetime { get; } = Lifetime.Singleton;

        public void Resolve(DependencyInfo dependency, CallView systemView, out object instance)
        {
            if (dependency.Instance is not null)
            {
                instance = dependency.Instance;
                return;
            }

            var dependencyType = dependency.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));

            if (isComponent)
            {
                var dependencyObject = dependency.GameObject;
                
                if (dependencyObject)
                    throw new Exception($"{dependency.DependencyType} was not registered as a component and cannot be resolved");

                instance = dependencyObject.GetComponent(dependencyType);
            }
            else
            {
                instance = Activator.CreateInstance(dependencyType);
                systemView.Call(new DependencyCreatedCall(instance));
            }

            dependency.Instance = instance;
        }
    }
}