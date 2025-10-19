using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal abstract class Container
    {
        protected abstract Dictionary<DependencyInfo, ILifetimeImplementation> DependenciesContainer { get; set; }
        protected abstract CallView SystemView { get; }
        protected abstract LifetimeFactory LifetimeFactory { get; }
        
        public abstract DependencyInfo Register(Type typeToRegister, Lifetime lifetime);
        public DependencyInfo Register<T>(Lifetime lifetime) => Register(typeof(T), lifetime);
        
        public abstract object Resolve(Type dependencyToResolve, string tag = null);
        public T Resolve<T>(string tag = null) => (T)Resolve(typeof(T), tag);
        
        public void CancelRegister(DependencyInfo dependency) => DependenciesContainer.Remove(dependency);
        
        public bool TryResolve(Type objectToResolve, string tag, out object instance)
        {
            instance = Has(objectToResolve, tag) ? Resolve(objectToResolve, tag) : null;
            
            return instance is not null;
        }

        public bool TryResolve<T>(string tag, out T instance)
        {
            instance = Has(typeof(T), tag) ? Resolve<T>(tag) : default;
            
            return instance is not null;
        }

        public void ChangeImplementation(DependencyInfo dependency, Type interfaceType)
        {
            dependency.ImplementsInterface = interfaceType;
        }

        public void ChangeTag(DependencyInfo dependency, string tag)
        {
            MoreTagsValidate(dependency.DependencyType, tag);
            
            dependency.DependencyTag = tag;
        }

        protected void SetInstance(DependencyInfo dependency, object instance)
        {
            dependency.Instance = instance;
        }

        protected DependencyInfo GetDependency(Type dependencyType, string tag = null)
        {
            return dependencyType.IsInterface 
                ? DependenciesContainer.FirstOrDefault(o => 
                    o.Key.ImplementsInterface == dependencyType && o.Key.DependencyTag == tag).Key 
                : DependenciesContainer.FirstOrDefault(o => 
                    o.Key.DependencyType == dependencyType && o.Key.DependencyTag == tag).Key;
        }
        
        private bool Has(Type typeToCheck, string tag = null) => DependenciesContainer.FirstOrDefault(dependency => 
            (dependency.Key.DependencyType == typeToCheck || dependency.Key.ImplementsInterface == typeToCheck) && 
            dependency.Key.DependencyTag == tag).Key is not null;
        
        protected void ResolveValidate(Type dependencyToResolve, string tag)
        {
            MoreRegistrationsValidate(dependencyToResolve, tag);
            
            if (Has(dependencyToResolve, tag)) return;
            
            if (dependencyToResolve.IsSubclassOf(typeof(Component)))
                throw new Exception($"{dependencyToResolve.Name} is not registered in the container, or the GameObject that the component belonged to has been deleted.");
                
            if (dependencyToResolve.IsInterface)
                throw new Exception($"The interface {dependencyToResolve.Name} is not implemented by any class with the tag {tag ?? "{Null}"}");
                
            throw new Exception($"{dependencyToResolve.Name} is not registered in the container. Register it before resolving");
        }

        private void MoreTagsValidate(Type dependencyType, string tag)
        {
            if (Has(dependencyType, tag))
                throw new Exception($"Dependency {dependencyType.Name} with tag {tag ?? "Null"} already registered in the container. Use other tag");
        }

        private void MoreRegistrationsValidate(Type interfaceType, string tag)
        {
            if (!interfaceType.IsInterface) return;
            
            var hasMore = DependenciesContainer.Count(dependency =>
                dependency.Key.ImplementsInterface == interfaceType &&
                dependency.Key.DependencyTag == tag) > 1;
            
            if (hasMore)
                throw new Exception($"Tags for the {interfaceType.Name} interface must not be repeated during registration.");
        }
        
    }
}