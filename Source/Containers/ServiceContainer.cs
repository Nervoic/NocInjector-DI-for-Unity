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
            _servicesContainer.TryAdd(type, lifetime);
        }
        
        /// <summary>
        /// Registers a service type with the specified lifetime.
        /// </summary>
        public void Register<T>(ServiceLifetime lifetime)
        {
            var type = typeof(T);
            _servicesContainer.TryAdd(type, lifetime);
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

            throw new ArgumentException($"Service of type '{type.FullName}' is not registered in the container. Please register the service before resolving.");
        }
        
        /// <summary>
        /// Resolves an instance of the specified service type.
        /// </summary>

        public T Resolve<T>()
        {
            var type = typeof(T);
            
            try
            {
                if (_servicesContainer.TryGetValue(type, out var lifetime))
                {
                    object obj;
                    
                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            if (_singletonContainer.TryGetValue(type, out var value)) return (T)value;
                            obj = Activator.CreateInstance(type);
                            _singletonContainer.TryAdd(type, obj);
                            break;
                        case ServiceLifetime.Transient:
                            obj = (T)Activator.CreateInstance(type);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), $"ServiceLifetime value '{(int)lifetime}' is not supported. Please use a valid ServiceLifetime enum value.");
                    }
                    InjectToCreatedObject(obj);
                    return (T)obj;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while resolving service of type '{type.FullName}': {e.Message}");
            }

            throw new ArgumentException($"Service of type '{type.FullName}' is not registered in the container or a cyclical dependence has been discovered.");
        }
        
        /// <summary>
        /// Resolves a realisation of this interface by tag
        /// </summary>
        
        public object ResolveByInterface(Type interfaceType, string realisationTag)
        {
            if (!interfaceType.IsInterface) throw new ArgumentException($"{interfaceType.Name} is not interface.");

            var realisationType = _servicesContainer.FirstOrDefault(s =>
                s.Key.IsDefined(typeof(RegisterByInterface)) &&
                s.Key.GetCustomAttribute<RegisterByInterface>().InterfaceType == interfaceType &&
                s.Key.GetCustomAttribute<RegisterByInterface>().RealisationTag == realisationTag).Key;

            if (realisationType is not null) return Resolve(realisationType);
            return null;
        }
        
        /// <summary>
        /// Resolves a realisation of T interface by tag
        /// </summary>
        
        public T ResolveByInterface<T>(string realisationTag) where T : class
        {
            var interfaceType = typeof(T);
            if (!interfaceType.IsInterface) throw new ArgumentException($"{interfaceType.Name} is not interface.");

            var realisationType = _servicesContainer.FirstOrDefault(s =>
                s.Key.IsDefined(typeof(RegisterByInterface)) &&
                s.Key.GetCustomAttribute<RegisterByInterface>().InterfaceType == interfaceType &&
                s.Key.GetCustomAttribute<RegisterByInterface>().RealisationTag == realisationTag).Key;

            if (realisationType is not null) return (T)Resolve(realisationType);
            return null;
        }

        private void InjectToCreatedObject(object obj)
        {
            _currentInjector ??= new ServiceInjector();
            foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
            {
                _currentInjector.Inject(obj, member);
            }
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
