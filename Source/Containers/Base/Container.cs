using System;
using System.Collections.Generic;
using System.Linq;

namespace NocInjector
{
    internal abstract class Container
    {
        protected abstract Dictionary<DependencyInfo, Lifetime> DependenciesContainer { get; set; }
        public abstract void Register(Type typeToRegister, Lifetime lifetime);
        public abstract object Resolve(Type dependencyToResolve, string tag = null);

        public void Register<T>(Lifetime lifetime) => Register(typeof(T), lifetime);
        public T Resolve<T>(string tag = null) => (T)Resolve(typeof(T), tag);
        
        public bool TryResolve(Type objectToResolve, string tag, out object instance)
        {
            instance = Has(objectToResolve, tag) ? Resolve(objectToResolve, tag) : null;
            
            return instance is not null;
        }

        public bool TryResolve<T>(string tag, out T instance)
        {

            instance = Has<T>(tag) ? Resolve<T>(tag) : default;
            
            return instance is not null;
        }

        public void ChangeImplementation(Type implementsType, Type interfaceType)
        {
            var info = GetInfoByType(implementsType);

            ResetObject(info, interfaceType, info.DependencyTag, info.Instance);
        }

        public void ChangeTag(Type typeToAddId, string tag)
        {
            var info = GetInfoByType(typeToAddId);
            
            ResetObject(info, info.ImplementsInterface, tag, info.Instance);
        }

        protected void SetInstance(DependencyInfo info, object instance)
        {
            ResetObject(info, info.ImplementsInterface, info.DependencyTag, instance);
        }

        private void ResetObject(DependencyInfo info, Type newImplementsType, string newTag, object newInstance)
        {
            var newInfo = new DependencyInfo(info.DependencyType, newImplementsType, newTag, newInstance);
            var lifetime = DependenciesContainer[info];

            DependenciesContainer.Remove(info);
            DependenciesContainer.TryAdd(newInfo, lifetime);
        }

        protected DependencyInfo GetDependency(Type dependencyType, string tag = null)
        {
            return dependencyType.IsInterface 
                ? DependenciesContainer.FirstOrDefault(o => 
                    o.Key.ImplementsInterface == dependencyType && o.Key.DependencyTag == tag).Key 
                : DependenciesContainer.FirstOrDefault(o => 
                    o.Key.DependencyType == dependencyType && o.Key.DependencyTag == tag).Key;
        }
        protected DependencyInfo GetInfoByType(Type type)
        {
            return DependenciesContainer.FirstOrDefault(i => i.Key.DependencyType == type).Key;
        }
        
        
        public bool Has(Type typeToCheck, string tag = null) => DependenciesContainer.FirstOrDefault(i => (i.Key.DependencyType == typeToCheck || i.Key.ImplementsInterface == typeToCheck) && i.Key.DependencyTag == tag).Key is not null;

        public bool Has<T>(string tag = null) => Has(typeof(T), tag);
    }
}