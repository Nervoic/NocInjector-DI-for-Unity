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
        private readonly GameObject _containerObject;
        private readonly Dictionary<Type, Component> _componentsContainer = new();

        public ObjectContainer(GameObject gameObject)
        {
            _containerObject = gameObject;
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        public void Register(Type type, Component component)
        {
            if (component is null)
            {
                Debug.LogError($"The {type.Name} component is null.");
                return;
            }

            if (component.gameObject != _containerObject)
            {
                Debug.LogError($"The {type.Name} component is not located on the {_containerObject.name} container that you are trying to add it to.");
                return;
            }

            if (!_componentsContainer.TryAdd(type, component)) 
                Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        
        public void Register<T>(Component component) where T : Component
        {
            var type = typeof(T);
            if (component is null)
            {
                Debug.LogError($"The {type.Name} component is null.");
                return;
            }

            if (component.gameObject != _containerObject)
            {
                Debug.LogError($"The {type.Name} component is not located on the {_containerObject.name} container that you are trying to add it to.");
                return;
            }

            if (!_componentsContainer.TryAdd(type, component)) 
                Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");
        }
        
        /// <summary>
        /// Resolves a component instance by its type.
        /// </summary>
        public Component Resolve(Type type)
        {
            if (type.IsInterface)
            {
                Debug.LogError($"To get an instance by interface {type.Name}, use the ResolveByInterface method");
                return null;
            }

            if (_componentsContainer.TryGetValue(type, out var value))
            {
                if (value is not null) return value;

                _componentsContainer.Remove(type);
                Debug.LogWarning($"Component of type {type.Name} is missing in the container.");
            }
            
            return null;
        }
        
        /// <summary>
        /// Resolves a component instance by T type.
        /// </summary>

        public T Resolve<T>() where T : Component
        {
            var type = typeof(T);

            if (type.IsInterface)
            {
                Debug.LogError($"To get an instance by interface {type.Name}, use the ResolveByInterface method");
                return null;
            }

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
            if (!interfaceType.IsInterface)
            {
                Debug.LogError($"{interfaceType.Name} is not an interface. To get a component from a container, use the Resolve method");
                return null;
            }
            
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
