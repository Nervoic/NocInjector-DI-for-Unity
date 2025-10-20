using System;
using System.Collections.Generic;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal abstract class Container
    {
        protected abstract Dictionary<DependencyInfo, LifetimeImplementation> DependenciesContainer { get; set; }
        protected abstract CallView SystemView { get; }
        protected abstract LifetimeFactory LifetimeFactory { get; }

        public abstract DependencyInfo Register<T>(Lifetime lifetime);
        
        public abstract object Resolve(Type dependencyType, string tag);
        public T Resolve<T>(string tag = null) => (T)Resolve(typeof(T), tag);
        public void CancelRegister(DependencyInfo dependency) => DependenciesContainer.Remove(dependency);
        
        public bool TryResolve(Type dependencyType, string tag, out object instance)
        {
            instance = Has(dependencyType, tag)
                ? Resolve(dependencyType, tag)
                : null;

            return instance is not null;
        }

        public bool TryResolve<T>(string tag, out T instance)
        {
            instance = Has(typeof(T), tag) 
                ? Resolve<T>(tag) 
                : default;
            
            return instance is not null;
        }

        public void SetImplementation<TInterfaceType>(DependencyInfo dependency)
        {
            dependency.ImplementsInterface = typeof(TInterfaceType);
        }

        public void SetTag(DependencyInfo dependency, string tag)
        {
            MoreTagsValidate(dependency.DependencyType, tag);

            dependency.DependencyTag = tag;
        }

        public void SetInstance(DependencyInfo dependency, object instance)
        {
            dependency.Instance = instance;
        }

        protected bool TryGetDependency(Type dependencyType, string tag, out DependencyInfo dependency)
        {
            dependency = DependenciesContainer.FirstOrDefault(dependency => 
                (dependency.Key.DependencyType == dependencyType || dependency.Key.ImplementsInterface == dependencyType) &&
                dependency.Key.DependencyTag == tag).Key;
            
            return dependency is not null;
        }
        
        protected bool TryGetDependencies(Type dependencyType, string tag, out DependencyInfo[] dependencies)
        {
            var dependenciesInfo = DependenciesContainer.Where(dependency =>
                dependency.Key.DependencyType == dependencyType ||
                dependency.Key.ImplementsInterface == dependencyType);

            if (tag is not null)
            {
                dependenciesInfo = dependenciesInfo.Where(dependency =>
                    dependency.Key.DependencyTag == tag);
            }
            
            dependencies = dependenciesInfo.Select(dependency => dependency.Key).ToArray();
            return dependencies.Length > 0;
        }

        private bool Has(Type type, string tag)
        {
            var isArray = type.IsArray;
            var dependencyType = isArray ? type.GetElementType() : type;

            if (isArray && tag is null)
                return Has(type);
            
            var hasDependency = DependenciesContainer.FirstOrDefault(dependency =>
                (dependency.Key.DependencyType == dependencyType || dependency.Key.ImplementsInterface == dependencyType) &&
                dependency.Key.DependencyTag == tag).Key is not null;

            return hasDependency;
        }

        private bool Has(Type type)
        {
            var dependencyType = type.IsArray ? type.GetElementType() : type;
            
            var hasDependency = DependenciesContainer.FirstOrDefault(dependency =>
                dependency.Key.DependencyType == dependencyType ||
                dependency.Key.ImplementsInterface == dependencyType).Key is not null;

            return hasDependency;
        }

        protected void ResolveValidate(Type dependencyType, string tag)
        {
            if (dependencyType.IsSubclassOf(typeof(Component)))
            {
                if (Has(dependencyType) && tag is null)
                    throw new Exception($"The component {dependencyType.Name} is not registered with Null-tag, but registered with other tags, or or the GameObject that the component belonged to has been deleted");
                
                throw new Exception($"{dependencyType.Name} component is not registered in the container, or the GameObject that the component belonged to has been deleted.");
            }

            if (dependencyType.IsInterface)
            {
                if (Has(dependencyType) && tag is null)
                    throw new Exception($"The interface {dependencyType.Name} is not implemented with Null-tag, but implemented with other tags");
                    
                throw new Exception($"The interface {dependencyType.Name} is not implemented by any class with the tag {tag ?? "{Null}"}");
            }

            if (Has(dependencyType) && tag is null)
                throw new Exception($"The dependency {dependencyType.Name} is not registered with Null-tag, but registered with other tags");
            
            throw new Exception($"{dependencyType.Name} is not registered in the container with tag {tag}. Register it before resolving");
        }

        private void MoreTagsValidate(Type dependencyType, string tag)
        {
            if (Has(dependencyType, tag))
                throw new Exception($"Dependency {dependencyType.Name} with tag {tag ?? "Null"} already registered in the container. Use other tag");
        }

        protected void MoreRegistrationsValidate(Type interfaceType, string tag)
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