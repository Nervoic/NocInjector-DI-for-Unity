using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, Type> _interfaceRealisations = new();

        public ComponentContainer(GameObject gameObject)
        {
            _containerObject = gameObject;
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        public ContainerRegister Register(Type typeToRegister, Component component)
        {
            if (HasRegisterError(component, typeToRegister))
                return null;

            if (!_componentsContainer.TryAdd(typeToRegister, component)) 
                Debug.LogError($"Failed to add {component.name} to container. Component already exists in the container.");

            return new ContainerRegister(this, typeToRegister);
        }
        
        /// <summary>
        /// Registers a component instance by its type.
        /// </summary>
        
        public ContainerRegister Register<T>(Component component) where T : Component
        {
            var typeToRegister = typeof(T);
            
            return Register(typeToRegister, component);
        }
        
        public class ContainerRegister
        {
            private readonly ComponentContainer _container;
            private readonly Type _currentServiceType;

            public ContainerRegister(ComponentContainer container, Type currentServiceType)
            {
                _container = container;
                _currentServiceType = currentServiceType;
            }
            
            public void AsImplementation(Type interfaceType, string realisationTag)
            {
                if (!interfaceType.IsInterface)
                {
                    Debug.LogError($"You are trying to register {_currentServiceType.Name} as an implementation of {interfaceType.Name}, but {interfaceType.Name} is not an interface.");
                    return;
                }

                if (!interfaceType.IsAssignableFrom(_currentServiceType))
                {
                    Debug.LogError($"{_currentServiceType.Name} service does not implement the {interfaceType.Name} interface, and cannot be registered.");
                }
                
                if (!_container._interfaceRealisations.TryAdd(realisationTag, _currentServiceType))
                    Debug.LogWarning($"The {realisationTag} realisation tag is already registered in the container.");
            }
            
            public void AsImplementation<T>(string realisationTag)
            {
                var interfaceType = typeof(T);
                
                AsImplementation(interfaceType, realisationTag);
            }
        }
        
        /// <summary>
        /// Resolves a component instance by its type.
        /// </summary>
        public Component Resolve(Type typeToResolve)
        {
            if (HasResolveError(typeToResolve))
                return null;

            if (_componentsContainer.TryGetValue(typeToResolve, out var component))
            {
                if (component is not null) return component;

                _componentsContainer.Remove(typeToResolve);
                Debug.LogWarning($"Component of type {typeToResolve.Name} is missing in the container.");
            }
            
            return null;
        }
        
        /// <summary>
        /// Resolves a component instance by T type.
        /// </summary>

        public T Resolve<T>() where T : Component
        {
            var typeToResolve = typeof(T);

            return (T)Resolve(typeToResolve);
        }
        
        /// <summary>
        /// Resolves a component instance by interface
        /// </summary>
        public Component ResolveImplementation(string realisationTag)
        {
            return _interfaceRealisations.TryGetValue(realisationTag, out var realisationType) ? Resolve(realisationType) : null;
        }
        
        /// <summary>
        /// Resolves a component instance by T interface
        /// </summary>

        public T ResolveImplementation<T>(string realisationTag) where T : class
        {
            return ResolveImplementation(realisationTag) as T;
        }
        

        private bool HasRegisterError(Component component, Type typeToRegister)
        {
            if (component is null)
            {
                Debug.LogError($"The {typeToRegister.Name} component is null.");
                return true;
            }

            if (typeToRegister.IsInterface)
            {
                Debug.LogError($"Cannot register the {typeToRegister.Name} interface in the ComponentContainer on {_containerObject.name}");
                return true;
            }

            if (component.gameObject != _containerObject)
            {
                Debug.LogError($"The {typeToRegister.Name} component is not located on the {_containerObject.name} container that you are trying to add it to.");
                return true;
            }

            return false;
        }

        private bool HasResolveError(Type typeToResolve)
        {
            if (typeToResolve.IsInterface)
            {
                Debug.LogError($"To get an instance by interface {typeToResolve.Name}, use the ResolveByInterface method");
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
