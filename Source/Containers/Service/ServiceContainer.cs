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

        private ServiceInjector _currentInjector;
        
        /// <summary>
        /// Registers a service type with the specified lifetime.
        /// </summary>
        public void Register(Type type, ServiceLifetime lifetime)
        {
            if (HasRegisterError(type))
                return;
            
            if (!_servicesContainer.TryAdd(type, lifetime))
                Debug.LogWarning($"The {type.Name} service is already registered in the container.");
        }
        
        /// <summary>
        /// Registers a service T type with the specified lifetime.
        /// </summary>
        public void Register<T>(ServiceLifetime lifetime)
        {
            var type = typeof(T);
            Register(type, lifetime);
        }

        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>
        public object Resolve(Type type)
        {
            try
            {
                if (_servicesContainer.TryGetValue(type, out var lifetime))
                {
                    object obj;
                    
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            if (_singletonContainer.TryGetValue(type, out var value)) return value;
                            obj = Activator.CreateInstance(type);
                            _singletonContainer.TryAdd(type, obj);
                            break;
                        case ServiceLifetime.Transient:
                            obj = Activator.CreateInstance(type);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), $"ServiceLifetime value '{(int)lifetime}' is not supported. Please use a valid ServiceLifetime enum value.");
                    }
                    InjectToCreatedObject(obj);
                    return obj;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while resolving service of type '{type.FullName}': {e.Message}");
            }

            return null;
        }
        
        /// <summary>
        /// Resolves an instance of the T service type.
        /// </summary>

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);

            return (T)Resolve(type);
        }
        
        /// <summary>
        /// Resolves a realisation of this interface by tag
        /// </summary>
        
        public object ResolveByInterface(Type interfaceType, string realisationTag)
        {
            if (!interfaceType.IsInterface) throw new ArgumentException($"{interfaceType.Name} is not interface.");

            var realisationType = _servicesContainer.FirstOrDefault(s =>
                s.Key.IsDefined(typeof(RegisterAsRealisation)) &&
                s.Key.GetCustomAttribute<RegisterAsRealisation>().InterfaceType == interfaceType &&
                s.Key.GetCustomAttribute<RegisterAsRealisation>().RealisationTag == realisationTag).Key;

            if (realisationType is not null) return Resolve(realisationType);
            return null;
        }
        
        /// <summary>
        /// Resolves a realisation of T interface by tag
        /// </summary>
        
        public T ResolveByInterface<T>(string realisationTag) where T : class
        {
            var interfaceType = typeof(T);
            
            return (T)ResolveByInterface(interfaceType, realisationTag);
        }

        private void InjectToCreatedObject(object obj)
        {
            _currentInjector ??= new ServiceInjector();
            foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
            {
                _currentInjector.Inject(obj, member);
            }
        }

        private bool HasRegisterError(Type type)
        {
            if (type.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError($"The {type.Name} component cannot be added to the service container.");
                return true;
            }

            if (type.IsInterface)
            {
                Debug.LogError($"Cannot register the {type.Name} interface in the services container.");
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
