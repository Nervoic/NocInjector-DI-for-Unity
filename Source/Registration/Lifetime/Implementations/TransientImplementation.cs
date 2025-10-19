using System;
using NocInjector.Calls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class TransientImplementation : ILifetimeImplementation
    {
        public Lifetime Lifetime { get; } = Lifetime.Transient;

        public void Resolve(DependencyInfo dependency, CallView systemView, out object instance)
        {
            var dependencyType = dependency.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));
            
            if (isComponent)
            {
                var dependencyObject = dependency.GameObject;
                
                if (dependencyObject is null)
                    throw new Exception($"{dependency.DependencyType} was not registered as a component and cannot be resolved");

                instance = Object.Instantiate(dependencyObject).GetComponent(dependencyType);
            }
            else
            {
                instance = Activator.CreateInstance(dependencyType);
                systemView.Call(new DependencyCreatedCall(instance));
            }
        }
    }
}