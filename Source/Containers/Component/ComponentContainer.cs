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
    public class ComponentContainer
    {
        private readonly GameObject _containerObject;
        private readonly Dictionary<Type, Component> _componentsContainer = new();

        public ComponentContainer(GameObject gameObject)
        {
            _containerObject = gameObject;
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        public void Register(Type type, Component component)
        {
            if (HasRegisterError(component, type))
                return;

            if (!_componentsContainer.TryAdd(type, component)) 
                Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        
        public void Register<T>(Component component) where T : Component
        {
            var type = typeof(T);
            
            Register(type, component);
        }
        
        /// <summary>
        /// Resolves a component instance by its type.
        /// </summary>
        public Component Resolve(Type type)
        {
            if (HasResolveError(type))
                return null;

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

            return (T)Resolve(type);
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
                c.Key.IsDefined(typeof(RegisterAsRealisation), true) &&
                c.Key.GetCustomAttribute<RegisterAsRealisation>().RealisationTag == realisationTag &&
                c.Key.GetCustomAttribute<RegisterAsRealisation>().InterfaceType == interfaceType).Value;

            return realisation;
        }
        
        /// <summary>
        /// Resolves a component instance by T interface
        /// </summary>

        public T ResolveByInterface<T>(string realisationTag) where T : class
        {
            var interfaceType = typeof(T);

            return ResolveByInterface(interfaceType, realisationTag) as T;
        }
        

        private bool HasRegisterError(Component component, Type type)
        {
            if (component is null)
            {
                Debug.LogError($"The {type.Name} component is null.");
                return true;
            }

            if (type.IsInterface)
            {
                Debug.LogError($"Cannot register the {type.Name} interface in the ComponentContainer on {_containerObject.name}");
                return true;
            }

            if (component.gameObject != _containerObject)
            {
                Debug.LogError($"The {type.Name} component is not located on the {_containerObject.name} container that you are trying to add it to.");
                return true;
            }

            return false;
        }

        private bool HasResolveError(Type type)
        {
            if (type.IsInterface)
            {
                Debug.LogError($"To get an instance by interface {type.Name}, use the ResolveByInterface method");
                return true;
            }

            return false;
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
