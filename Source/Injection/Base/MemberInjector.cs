using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal abstract class MemberInjector : IInjector
    {
        public abstract InjectorType InjectionType { get; }
        private GameContext[] _gameContexts;

        internal MemberInjector()
        {
            InitContexts();
        }

        public abstract void Inject(MemberInfo injectableMember, object instance, Context context);

        protected object GetDependency(Type dependencyType, Inject injectAttr, ContainerView container = null)
        {
            var tags = injectAttr.Tags;
            var contextLifetime = injectAttr.ContextType;

            var dependencyInstance = ResolveDependency(dependencyType, tags, container);

            if (dependencyInstance is not null || contextLifetime == ContextType.Object) return dependencyInstance;

            var contexts = GetContexts(contextLifetime);
            FindDependencyInContexts(dependencyType, tags, contexts, out dependencyInstance);

            // if (dependencyInstance is null)
            //     throw new Exception($"Unable to retrieve dependency {dependencyType.Name}. It may not be registered or you may have specified the injection parameters incorrectly");

            return dependencyInstance;
        } 
        
        
        private void FindDependencyInContexts(Type dependencyType, string[] tags,  GameContext[] contexts, out object dependencyInstance)
        {
            foreach (var context in contexts)
            {
                var currentContainer = context.Container;
                dependencyInstance = ResolveDependency(dependencyType, tags, currentContainer);

                if (dependencyInstance is not null)
                    return;
            }

            dependencyInstance = null;
        }

        private object ResolveDependency(Type dependencyType, string[] tags, ContainerView container)
        {
            if (container is null)
                return null;

            if (!tags.Any())
            {
                container.TryResolve(dependencyType, null, out var instance);
                return instance;
            }
            
            foreach (var currentTag in tags)
            {
                container.TryResolve(dependencyType, currentTag, out var instance);
                
                if (instance is not null) 
                    return instance;
            }

            return null;
        }
        
        private GameContext[] GetContexts(ContextType contextType)
        {
            if (contextType is ContextType.All) return _gameContexts;
            
            var selectedContexts = _gameContexts.Where(context => contextType == ContextType.Scene 
                ? context.Lifetime == ContextLifetime.Scene 
                : context.Lifetime == ContextLifetime.Project).ToArray();

            if (selectedContexts.Length <= 0)
                throw new Exception($"There is no context with the lifetime {nameof(contextType)}, and the injection cannot be performed.");

            return selectedContexts;
        }

        private void InitContexts() => _gameContexts = Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None)
            .Where(context => context.Lifetime != ContextLifetime.Object).ToArray();
    }
}