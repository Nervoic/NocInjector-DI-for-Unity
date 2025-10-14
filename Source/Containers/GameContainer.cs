using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NocInjector.Calls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class GameContainer : Container
    {
        protected override Dictionary<ObjectInfo, Lifetime> ObjectContainer { get; set; } = new();
        private readonly Dictionary<ObjectInfo, GameObject> _componentsContainer = new();
        
        private readonly CallView _systemView;

        internal GameContainer(CallView systemView)
        {
            _systemView = systemView;
        }
        
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
            
            if (info is null) 
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
                    _componentsContainer.Remove(objectInfo);
            }

            object instance;
            
            switch (lifetime)
            {
                case Lifetime.Singleton: 
                    ResolveSingleton(objectInfo, out instance);
                    break;
                case Lifetime.Transient:
                    ResolveTransient(objectInfo, out instance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), $"Lifetime value '{(int)lifetime}' is not supported. Please use a valid Lifetime enum value.");
            }
            
            if (!isComponent)
                OnInstanceResolved(instance);

            return instance;
        }

        private void ResolveSingleton(ObjectInfo objectInfo, out object instance)
        {
            if (objectInfo.Instance is not null)
            {
                instance = objectInfo.Instance;
                return;
            }

            var objectType = objectInfo.ObjectType;
            var isComponent = objectType.IsSubclassOf(typeof(Component));

            instance = isComponent
                ? _componentsContainer[objectInfo].GetComponent(objectInfo.ObjectType)
                : Activator.CreateInstance(objectType);
            
            SetInstance(objectInfo, instance);
        }

        private void ResolveTransient(ObjectInfo objectInfo, out object instance)
        {
            var objectType = objectInfo.ObjectType;
            var isComponent = objectType.IsSubclassOf(typeof(Component));
            
            instance = isComponent
                ? Object.Instantiate(_componentsContainer[objectInfo]).GetComponent(objectInfo.ObjectType) 
                : Activator.CreateInstance(objectType);
        }
        
        private void OnInstanceResolved(object obj)
        {
            if (obj is null) return;

            var injectableMembers = obj.GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(Inject), true)).ToArray();
            
            _systemView.Call(new InstanceResolvedCall(injectableMembers, obj));
                
        }
        
        
    }
}
