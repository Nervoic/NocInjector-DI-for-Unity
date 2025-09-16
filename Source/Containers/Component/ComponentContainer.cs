using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class ComponentContainer : Container
    {
        protected override Dictionary<ObjectInfo, Lifetime> ObjectContainer { get; set; } = new();
        protected override Dictionary<ObjectInfo, object> SingletonContainer { get; set; } = new();
        private readonly Dictionary<ObjectInfo, GameObject> _componentsContainer = new();
        
        private readonly GameObject _containerObject;

        public ComponentContainer(GameObject gameObject)
        {
            _containerObject = gameObject;
        }
        
        public override void Register(Type typeToRegister, Lifetime lifetime)
        {
            var newObject = new ObjectInfo(typeToRegister);

            if (!ObjectContainer.TryAdd(newObject, lifetime))
            {
                Debug.LogError($"Failed to add {typeToRegister.Name} to container. Component already exists in the container.");
                return;
            }
            _componentsContainer.Add(newObject, _containerObject);
        }
        
        
        public override void Register<T>(Lifetime lifetime)
        {
            Register(typeof(T), lifetime);
        }

        public void RegisterComponent(Type typeToRegister, GameObject gameObject)
        {
            var info = GetInfoByType(typeToRegister);
            
            if (!_componentsContainer.TryGetValue(info, out var obj))
                return;

            _componentsContainer[info] = gameObject;
        }
        
        public override object Resolve(Type objectToResolve, string tag = null)
        {
            if (!Has(objectToResolve, tag))
                throw new Exception($"{objectToResolve.Name} is not registered in the container. Register it before resolving"); 

            var objectInfo = GetObject(objectToResolve, tag);
            
            if (ObjectContainer.TryGetValue(objectInfo, out var lifetime))
            {
                
                if (_componentsContainer[objectInfo] is null)
                {
                    _componentsContainer.Remove(objectInfo);
                    return null;
                }
                
                switch (lifetime)
                {
                    case Lifetime.Singleton:
                        var singletonInfo = GetSingleton(objectToResolve, tag);
                        
                        Component component;
                        if (singletonInfo is null)
                        {
                            
                            component = _componentsContainer[objectInfo].GetComponent(objectInfo.ObjectType);
                            
                            var newSingleton = new ObjectInfo(objectInfo.ObjectType, objectInfo.ImplementsInterface,
                                objectInfo.ObjectTag);
                            SingletonContainer.TryAdd(newSingleton, component);
                        }
                        else
                            component = SingletonContainer[singletonInfo] as Component;
                        return component;
                    case Lifetime.Transient:
                        return Object.Instantiate(_componentsContainer[objectInfo]).GetComponent(objectInfo.ObjectType);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), $"ServiceLifetime value '{(int)lifetime}' is not supported. Please use a valid ServiceLifetime enum value.");
                }
            }
            
            return null;
        }

        public override T Resolve<T>(string tag = null)
        {
            return (T)Resolve(typeof(T));
        }
    }
}
