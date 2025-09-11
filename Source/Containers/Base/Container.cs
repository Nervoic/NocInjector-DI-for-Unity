using System;
using System.Collections.Generic;
using System.Linq;

namespace NocInjector
{
    internal abstract class Container
    {
        protected abstract Dictionary<ObjectInfo, Lifetime> ObjectContainer { get; set; }
        protected abstract Dictionary<ObjectInfo, object> SingletonContainer { get; set; }
        public abstract void Register(Type typeToRegister, Lifetime lifetime);
        public abstract void Register<T>(Lifetime lifetime);
        public abstract object Resolve(Type objectToResolve, string id = null);
        public abstract T Resolve<T>(string id = null);
        
        public bool TryResolve(Type objectToResolve, string id, out object instance)
        {
            if (!Has(objectToResolve, id))
                instance = null;
            else 
                instance = Resolve(objectToResolve, id);
            
            return instance is not null;
        }

        public bool TryResolve<T>(string id, out T instance)
        {
            if (!Has<T>(id))
                instance = default;
            else 
                instance = Resolve<T>(id);
            
            return instance is not null;
        }

        public void AddImplementation(Type implementsType, Type interfaceType)
        {
            var info = GetInfoByType(implementsType);

            ResetObject(info, interfaceType, info.ObjectId);
        }

        public void AddImplementation<TImplementsType, TInterfaceType>()
        {
            AddImplementation(typeof(TImplementsType), typeof(TInterfaceType));
        }

        public void AddId(Type typeToAddId, string id)
        {
            var info = GetInfoByType(typeToAddId);
            
            ResetObject(info, info.ImplementsInterface, id);
        }

        public void AddId<T>(string id)
        {
            AddId(typeof(T), id);
        }

        private void ResetObject(ObjectInfo info, Type newImplementsType, string newId)
        {
            var newInfo = new ObjectInfo(info.ObjectType, newImplementsType, newId);
            var lifetime = ObjectContainer[info];

            ObjectContainer.Remove(info);
            ObjectContainer.TryAdd(newInfo, lifetime);
        }

        protected ObjectInfo GetObject(Type objectType, string id = null)
        {
            return objectType.IsInterface 
                ? ObjectContainer.FirstOrDefault(o => 
                    o.Key.ImplementsInterface == objectType && o.Key.ObjectId == id).Key 
                : ObjectContainer.FirstOrDefault(o => 
                    o.Key.ObjectType == objectType && o.Key.ObjectId == id).Key;
        }

        protected ObjectInfo GetSingleton(Type singletonType, string id = null)
        {
            return singletonType.IsInterface
                ? SingletonContainer.FirstOrDefault(s =>
                    s.Key.ImplementsInterface == singletonType && s.Key.ObjectId == id).Key
                : SingletonContainer.FirstOrDefault(o =>
                    o.Key.ObjectType == singletonType && o.Key.ObjectId == id).Key;
        }
        
        protected ObjectInfo[] GetObjects(Type objectType)
        {
            return objectType.IsInterface
                ? ObjectContainer.Where(o =>
                    o.Key.ImplementsInterface == objectType).Select(o => o.Key).ToArray()
                : ObjectContainer.Where(o =>
                    o.Key.ObjectType == objectType).Select(o => o.Key).ToArray();
        }

        protected ObjectInfo[] GetSingletons(Type singletonType, string id = null)
        {
            return singletonType.IsInterface
                ? SingletonContainer.Where(s => 
                    s.Key.ImplementsInterface == singletonType).Select(s => s.Key).ToArray() 
                : SingletonContainer.Where(o => 
                    o.Key.ObjectType == singletonType).Select(s => s.Key).ToArray();
        }

        protected ObjectInfo GetInfoByType(Type type)
        {
            return ObjectContainer.FirstOrDefault(i => i.Key.ObjectType == type).Key;
        }
        
        public bool Has(Type typeToCheck, string id = null)
        {
            return ObjectContainer.FirstOrDefault(i => (i.Key.ObjectType == typeToCheck || i.Key.ImplementsInterface == typeToCheck) && i.Key.ObjectId == id).Key is not null;
        }

        public bool Has<T>(string id = null)
        {
            return Has(typeof(T), id);
        }
    }
}