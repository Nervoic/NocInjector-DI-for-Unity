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
        public abstract object Resolve(Type objectToResolve, string tag = null);
        public abstract T Resolve<T>(string tag = null);
        public bool TryResolve(Type objectToResolve, string tag, out object instance)
        {
            if (!Has(objectToResolve, tag))
                instance = null;
            else 
                instance = Resolve(objectToResolve, tag);
            
            return instance is not null;
        }

        public bool TryResolve<T>(string tag, out T instance)
        {
            if (!Has<T>(tag))
                instance = default;
            else 
                instance = Resolve<T>(tag);
            
            return instance is not null;
        }

        public void ChangeImplementation(Type implementsType, Type interfaceType)
        {
            var info = GetInfoByType(implementsType);

            ResetObject(info, interfaceType, info.ObjectTag);
        }

        public void ChangeImplementation<TImplementsType, TInterfaceType>()
        {
            ChangeImplementation(typeof(TImplementsType), typeof(TInterfaceType));
        }

        public void ChangeTag(Type typeToAddId, string tag)
        {
            var info = GetInfoByType(typeToAddId);
            
            ResetObject(info, info.ImplementsInterface, tag);
        }

        public void ChangeTag<T>(string tag)
        {
            ChangeTag(typeof(T), tag);
        }

        private void ResetObject(ObjectInfo info, Type newImplementsType, string newTag)
        {
            var newInfo = new ObjectInfo(info.ObjectType, newImplementsType, newTag);
            var lifetime = ObjectContainer[info];

            ObjectContainer.Remove(info);
            ObjectContainer.TryAdd(newInfo, lifetime);
        }

        protected ObjectInfo GetObject(Type objectType, string tag = null)
        {
            return objectType.IsInterface 
                ? ObjectContainer.FirstOrDefault(o => 
                    o.Key.ImplementsInterface == objectType && o.Key.ObjectTag == tag).Key 
                : ObjectContainer.FirstOrDefault(o => 
                    o.Key.ObjectType == objectType && o.Key.ObjectTag == tag).Key;
        }

        protected ObjectInfo GetSingleton(Type singletonType, string tag = null)
        {
            return singletonType.IsInterface
                ? SingletonContainer.FirstOrDefault(s =>
                    s.Key.ImplementsInterface == singletonType && s.Key.ObjectTag == tag).Key
                : SingletonContainer.FirstOrDefault(o =>
                    o.Key.ObjectType == singletonType && o.Key.ObjectTag == tag).Key;
        }
        
        protected ObjectInfo[] GetObjects(Type objectType, string[] tags)
        {
            return objectType.IsInterface
                ? ObjectContainer.Where(o =>
                    o.Key.ImplementsInterface == objectType && tags.Contains(o.Key.ObjectTag)).Select(o => o.Key).ToArray()
                : ObjectContainer.Where(o =>
                    o.Key.ObjectType == objectType && tags.Contains(o.Key.ObjectTag)).Select(o => o.Key).ToArray();
        }

        protected ObjectInfo[] GetSingletons(Type singletonType, string[] tags)
        {
            return singletonType.IsInterface
                ? SingletonContainer.Where(s => 
                    s.Key.ImplementsInterface == singletonType && tags.Contains(s.Key.ObjectTag)).Select(s => s.Key).ToArray() 
                : SingletonContainer.Where(s => 
                    s.Key.ObjectType == singletonType && tags.Contains(s.Key.ObjectTag)).Select(s => s.Key).ToArray();
        }

        protected ObjectInfo GetInfoByType(Type type)
        {
            return ObjectContainer.FirstOrDefault(i => i.Key.ObjectType == type).Key;
        }
        
        public bool Has(Type typeToCheck, string tag = null)
        {
            return ObjectContainer.FirstOrDefault(i => (i.Key.ObjectType == typeToCheck || i.Key.ImplementsInterface == typeToCheck) && i.Key.ObjectTag == tag).Key is not null;
        }

        public bool Has<T>(string tag = null)
        {
            return Has(typeof(T), tag);
        }
    }
}