using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Container for registering and resolving services with specified lifetimes.
    /// </summary>
    public class ServiceContainer
    {
        private readonly Dictionary<Type, ServiceLifetime> _servicesContainer = new();
        private readonly Dictionary<Type, object> _singletonContainer = new();
        private readonly Dictionary<string, Type> _interfaceRealisations = new();

        private ServiceInjector _currentInjector;
        
        /// <summary>
        /// Registers a service type with the specified lifetime.
        /// </summary>
        public ContainerRegister Register(Type typeToRegister, ServiceLifetime lifetime)
        {
            if (HasRegisterError(typeToRegister))
                return null;
            
            if (!_servicesContainer.TryAdd(typeToRegister, lifetime)) 
                Debug.LogWarning($"The {typeToRegister.Name} service is already registered in the container.");
            
            return new ContainerRegister(this, typeToRegister);
        }
        
        /// <summary>
        /// Registers a service T type with the specified lifetime.
        /// </summary>
        public ContainerRegister Register<T>(ServiceLifetime lifetime)
        {
            var type = typeof(T);
            return Register(type, lifetime);
        }

        public class ContainerRegister
        {
            private readonly ServiceContainer _container;
            private readonly Type _currentServiceType;

            public ContainerRegister(ServiceContainer container, Type currentServiceType)
            {
                _container = container;
                _currentServiceType = currentServiceType;
            }
            
            /// <summary>
            /// Registers the type as an interface implementation.
            /// </summary>
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
            
            
            /// <summary>
            /// Registers the type as an interface implementation.
            /// </summary>
            public void AsImplementation<T>(string realisationTag)
            {
                var interfaceType = typeof(T);
                
                AsImplementation(interfaceType, realisationTag);
            }
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        public object Resolve(Type serviceToResolve)
        {
            try
            {
                if (_servicesContainer.TryGetValue(serviceToResolve, out var lifetime))
                {
                    object instance;
                    
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            if (_singletonContainer.TryGetValue(serviceToResolve, out var value)) return value;
                            instance = Activator.CreateInstance(serviceToResolve);
                            _singletonContainer.TryAdd(serviceToResolve, instance);
                            break;
                        case ServiceLifetime.Transient:
                            instance = Activator.CreateInstance(serviceToResolve);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), $"ServiceLifetime value '{(int)lifetime}' is not supported. Please use a valid ServiceLifetime enum value.");
                    }
                    InjectToCreatedObject(instance);
                    return instance;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while resolving service of type '{serviceToResolve.FullName}': {e.Message}");
            }

            return null;
        }
        
        /// <summary>
        /// Resolves an instance of the T service type.
        /// </summary>

        public T Resolve<T>() where T : class
        {
            var serviceToResolve = typeof(T);

            return (T)Resolve(serviceToResolve);
        }

        /// <summary>
        /// Resolves a realisation of this interface by tag
        /// </summary>

        public object ResolveImplementation(string realisationTag)
        {
            return _interfaceRealisations.TryGetValue(realisationTag, out var realisationType) ? Resolve(realisationType) : null;
        }
        
        /// <summary>
        /// Resolves a realisation of T interface by tag
        /// </summary>
        
        public T ResolveImplementation<T>(string realisationTag) where T : class
        {
            return (T)ResolveImplementation(realisationTag);
        }

        private void InjectToCreatedObject(object obj)
        {
            _currentInjector ??= new ServiceInjector();
            foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
            {
                _currentInjector.InjectToField(obj, member);
            }
        }

        private bool HasRegisterError(Type typeToRegister)
        {
            if (typeToRegister.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError($"The {typeToRegister.Name} component cannot be added to the service container.");
                return true;
            }

            if (typeToRegister.IsInterface)
            {
                Debug.LogError($"Cannot register the {typeToRegister.Name} interface in the services container.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the service type is registered in the container.
        /// </summary>
        public bool Has(Type type)
        {
            return _servicesContainer.ContainsKey(type);
        }
        
        /// <summary>
        /// Checks if the service type is registered in the container.
        /// </summary>
        public bool Has<T>()
        {
            return _servicesContainer.ContainsKey(typeof(T));
        }
    }
}
