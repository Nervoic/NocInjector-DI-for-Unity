using System;
using System.Collections.Generic;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal class SingletonImplementation : LifetimeImplementation
    {
        public override Lifetime Lifetime { get; protected set; } = Lifetime.Singleton;

        private readonly List<DependencyInfo> _instancesInjected = new();

        public override void Resolve(DependencyInfo dependency, CallView systemView, out object instance)
        {
            var dependencyType = dependency.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));
            
            if (dependency.Instance is not null)
            {
                if (!_instancesInjected.Contains(dependency) && !isComponent)
                {
                    systemView.Call(new DependencyCreatedCall(dependency.Instance));
                    _instancesInjected.Add(dependency);
                }

                instance = dependency.Instance;
                return;
            }

            if (isComponent)
            {
                var dependencyObject = dependency.GameObject;
                
                if (dependencyObject is null)
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