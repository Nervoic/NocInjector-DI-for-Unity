using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class GameContainer : Container
    {
        protected override Dictionary<ObjectInfo, Lifetime> ObjectContainer { get; set; } = new();
        protected override Dictionary<ObjectInfo, object> SingletonContainer { get; set; } = new();
        private readonly Dictionary<ObjectInfo, GameObject> _componentsContainer = new();
        
        private readonly DependencyInjector _injector = new();
        
        public override void Register(Type typeToRegister, Lifetime lifetime)
        {
            var newObject = new ObjectInfo(typeToRegister);

            if (!ObjectContainer.TryAdd(newObject, lifetime))
            {
                Debug.LogWarning($"Failed to add {typeToRegister.Name} to container. Component already exists in the container.");
                return;
            }
            
            if (typeToRegister.IsSubclassOf(typeof(Component))) 
                _componentsContainer.Add(newObject, null);
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

            if (!ObjectContainer.TryGetValue(objectInfo, out var lifetime)) return null;
            
            var objectType = objectInfo.ObjectType;
            var isComponent = objectType.IsSubclassOf(typeof(Component));
            
            if (isComponent)
            {
                if (_componentsContainer[objectInfo] is null)
                { 
                    _componentsContainer.Remove(objectInfo);
                    throw new Exception($"GameObject is not installed for component {objectType.Name}");
                }
            }

            object instance;
            
            switch (lifetime)
            {
                case Lifetime.Singleton: 
                    ResolveSingleton(objectInfo, tag, out instance);
                    break;
                case Lifetime.Transient:
                    ResolveTransient(objectInfo, tag, out instance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), $"Lifetime value '{(int)lifetime}' is not supported. Please use a valid Lifetime enum value.");
            }
            
            if (!isComponent)
                InjectToResolvedObject(instance);

            return instance;
        }

        private void ResolveSingleton(ObjectInfo objectInfo, string tag, out object instance)
        {
            var objectType = objectInfo.ObjectType;
            var isComponent = objectType.IsSubclassOf(typeof(Component));
            
            var singletonInfo = GetSingleton(objectType, tag);

            if (singletonInfo is null)
            {
                instance = isComponent
                    ? _componentsContainer[objectInfo].GetComponent(objectInfo.ObjectType)
                    : Activator.CreateInstance(objectType);
                            
                var newSingleton = new ObjectInfo(objectInfo.ObjectType, objectInfo.ImplementsInterface, objectInfo.ObjectTag);
                SingletonContainer.TryAdd(newSingleton, instance);
            }
            else 
                instance = SingletonContainer[singletonInfo];
            
        }

        private void ResolveTransient(ObjectInfo objectInfo, string tag, out object instance)
        {
            var objectType = objectInfo.ObjectType;
            var isComponent = objectType.IsSubclassOf(typeof(Component));
            
            instance = isComponent
                ? Object.Instantiate(_componentsContainer[objectInfo]).GetComponent(objectInfo.ObjectType) 
                : Activator.CreateInstance(objectType);
        }
        
        private void InjectToResolvedObject(object obj)
        {
            if (obj is null) return;
            
            foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
            { 
                _injector.InjectToMember(member, obj);
            }
                
        }
        
        
    }
}
