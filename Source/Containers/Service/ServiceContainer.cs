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

        public override void Register<T>(Lifetime lifetime)
        {
            Register(typeof(T), lifetime);
        }
        
        public override object Resolve(Type objectToResolve, string id = null)
        {
            if (!Has(objectToResolve, id))
                throw new Exception($"{objectToResolve.Name} is not registered in the container. Register it before resolving"); 
            
            var objectInfo = GetObject(objectToResolve, id);
            
            if (ObjectContainer.TryGetValue(objectInfo, out var lifetime))
            {
                    object instance;
                    
                    switch (lifetime)
                    {
                        case Lifetime.Singleton:
                            var singletonInfo = GetSingleton(objectToResolve, id);

                            if (singletonInfo is null)
                            {
                                instance = Activator.CreateInstance(objectInfo.ObjectType);
                                var newSingletonInfo = new ObjectInfo(objectInfo.ObjectType, objectInfo.ImplementsInterface, objectInfo.ObjectId);
                                SingletonContainer.TryAdd(newSingletonInfo, instance);
                            }
                            else
                                instance = SingletonContainer[singletonInfo];
                            
                            break;
                        case Lifetime.Transient:
                            instance = Activator.CreateInstance(objectInfo.ObjectType);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), $"ServiceLifetime value '{(int)lifetime}' is not supported. Please use a valid ServiceLifetime enum value.");
                    }

                    return InjectToResolvedObject(instance);
            } 
            
            return null;
        }

        public override T Resolve<T>(string id = null)
        {
            return (T)Resolve(typeof(T));
        }

        private object InjectToResolvedObject(object obj)
        {
            if (obj is not null)
            {
                foreach (var member in obj.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject), true)))
                {
                    _injector.InjectToField(member, obj);
                }
            }

            return obj;
        }
    }
}
