using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    internal class ServiceContainer : Container
    {
        protected override Dictionary<ObjectInfo, Lifetime> ObjectContainer { get; set; } = new();
        protected override Dictionary<ObjectInfo, object> SingletonContainer { get; set; } = new();

        private readonly DependencyInjector _injector = new();
        
        public override void Register(Type typeToRegister, Lifetime lifetime)
        {
            var newObject = new ObjectInfo(typeToRegister);
            
            if (!ObjectContainer.TryAdd(newObject, lifetime)) 
                Debug.LogError($"The {typeToRegister.Name} service is already registered in the container.");
        }
        
        public override object Resolve(Type objectToResolve, string tag = null)
        {
            if (!Has(objectToResolve, tag))
                throw new Exception($"{objectToResolve.Name} is not registered in the container. Register it before resolving"); 
            
            var objectInfo = GetObject(objectToResolve, tag);
            
            if (ObjectContainer.TryGetValue(objectInfo, out var lifetime))
            {
                    object instance;
                    
                    switch (lifetime)
                    {
                        case Lifetime.Singleton:
                            var singletonInfo = GetSingleton(objectToResolve, tag);

                            if (singletonInfo is null)
                            {
                                instance = Activator.CreateInstance(objectInfo.ObjectType);
                                var newSingletonInfo = new ObjectInfo(objectInfo.ObjectType, objectInfo.ImplementsInterface, objectInfo.ObjectTag);
                                SingletonContainer.TryAdd(newSingletonInfo, instance);
                            }
                            else
                                instance = SingletonContainer[singletonInfo];
                            
                            break;
                        case Lifetime.Transient:
                            instance = Activator.CreateInstance(objectInfo.ObjectType);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), $"Lifetime value '{(int)lifetime}' is not supported. Please use a valid Lifetime enum value.");
                    }

                    return InjectToResolvedObject(instance);
            } 
            
            return null;
        }

        private object InjectToResolvedObject(object obj)
        {
            if (obj is not null)
            {
                foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
                {
                    _injector.InjectToMember(member, obj);
                }
            }

            return obj;
        }
    }
}
