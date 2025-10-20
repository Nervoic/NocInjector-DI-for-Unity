using System;
using System.Linq;
using System.Reflection;
using NocInjector.Calls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class TransientImplementation : LifetimeImplementation
    {
        public override Lifetime Lifetime { get; protected set; } = Lifetime.Transient;

        public override void Resolve(DependencyInfo dependency, CallView systemView, out object instance)
        {
            var dependencyType = dependency.DependencyType;
            var isComponent = dependencyType.IsSubclassOf(typeof(Component));

            if (dependency.Instance is not null && !isComponent)
            {
                instance = SetValuesFromInstance(dependency, systemView);
                return;
            }
            
            if (isComponent)
            {
                var dependencyObject = dependency.GameObject;
                
                if (dependencyObject is null)
                    throw new Exception($"{dependency.DependencyType} GameObject is null, but was not registered as a component and cannot be resolved");

                instance = Object.Instantiate(dependencyObject).GetComponent(dependencyType);
            }
            else
            {
                GetInstance(dependencyType, out instance);
                
                systemView.Call(new DependencyCreatedCall(instance));
            }
        }

        private object SetValuesFromInstance(DependencyInfo dependency, CallView systemView)
        {
            var dependencyType = dependency.DependencyType;
            var dependencyMembers = dependencyType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (!TryInvokeConstructor(dependencyType, out var newInstance))
                newInstance = Activator.CreateInstance(dependencyType);
            
            foreach (var member in dependencyMembers)
            {
                if (TryGetValue(dependency.Instance, member, out var value)) 
                    member.SetValue(newInstance, value);
            }
            
            systemView.Call(new DependencyCreatedCall(newInstance));
            
            return newInstance;
        }

        private bool TryGetValue(object dependencyInstance, MemberInfo member, out object value)
        {
            value = null;
            
            if (member is FieldInfo field)
                value = field.GetValue(dependencyInstance);
            
            else if (member is PropertyInfo property)
                value = property.GetValue(dependencyInstance);

            return value is not null;
        }

        private void GetInstance(Type dependencyType, out object instance)
        {
            if (!TryInvokeConstructor(dependencyType, out instance)) 
                instance = Activator.CreateInstance(dependencyType);
        }

        private bool TryInvokeConstructor(Type dependencyType, out object dependencyInstance)
        {
            dependencyInstance = null;
            var constructor = dependencyType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();

            if (constructor is null) 
                return false;
            
            var parameters = constructor.GetParameters();
            var values = new object[parameters.Length];

            dependencyInstance = constructor.Invoke(values);
            return true;
        }
    }
}