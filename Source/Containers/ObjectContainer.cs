using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Container for registering and resolving Unity components by type.
    /// </summary>
    public class ObjectContainer
    {

        private readonly Dictionary<Type, Component> _componentsContainer = new();
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        public void Register(Type type, Component component)
        {
            if (!_componentsContainer.TryAdd(type, component))
            {
                if (component is not null) Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");
                else throw new Exception($"Failed to register component of type {type.Name}: component is null.");
            }
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        
        public void Register<T>(Component component) where T : Component
        {
            var type = typeof(T);
            if (!_componentsContainer.TryAdd(type, component))
            {
                if (component is not null) Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");
                else throw new Exception($"Failed to register component of type {type.Name}: component is null.");
            }
        }
        
        /// <summary>
        /// Resolves a component instance by its type.
        /// </summary>
        public Component Resolve(Type type)
        {
            if (_componentsContainer.TryGetValue(type, out var value))
            {
                if (value is not null) return value;

                _componentsContainer.Remove(type);
                Debug.LogError($"Component of type {type.Name} is missing in the container.");
            } 
            else if (type.IsInterface) return _componentsContainer.FirstOrDefault(c => type.IsAssignableFrom(c.Key)).Value;

            return null;
        }
        
        /// <summary>
        /// Resolves a component instance by its type.
        /// </summary>

        public T Resolve<T>() where T : Component
        {
            var type = typeof(T);
            
            if (_componentsContainer.TryGetValue(type, out var value))
            {
                if (value is not null) return (T)value;

                _componentsContainer.Remove(type);
                Debug.LogError($"Component of type {type.Name} is missing in the container.");
            }
            else if (type.IsInterface) return (T)_componentsContainer.FirstOrDefault(c => type.IsAssignableFrom(c.Key)).Value;
            return null;
        }
        
        /// <summary>
        /// Resolves a component instance by interface
        /// </summary>
        public Component ResolveByInterface(Type interfaceType, string realisationTag)
        {
            var realisation = _componentsContainer.FirstOrDefault(c =>
                c.Key.IsDefined(typeof(RegisterByInterface), true) &&
                c.Key.GetCustomAttribute<RegisterByInterface>().RealisationTag == realisationTag &&
                c.Key.GetCustomAttribute<RegisterByInterface>().InterfaceType == interfaceType).Value;

            return realisation;
        }
        
        /// <summary>
        /// Checks if the service type is registered in the container.
        /// </summary>
        public bool Has(Type type)
        {
            return _componentsContainer.ContainsKey(type);
        }
        
        /// <summary>
        /// Checks if the service type is registered in the container.
        /// </summary>
        public bool Has<T>()
        {
            return _componentsContainer.ContainsKey(typeof(T));
        }
        
    }
}
