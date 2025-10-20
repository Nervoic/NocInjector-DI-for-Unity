using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal abstract class MemberInjector
    {
        public abstract InjectorType InjectorType { get; protected set; }
        private GameContext[] _gameContexts;

        internal MemberInjector()
        {
            InitContexts();
        }

        public abstract void Inject(MemberInfo injectableMember, object instance, ContainerView container = null);

        protected object GetDependency(Type dependencyType, Inject injectAttr, ContainerView container)
        {
            var contextLifetime = injectAttr.ContextType;

            if ((GetDependencyFromContainer(dependencyType, injectAttr, container, out var dependencyInstance) && contextLifetime == ContextType.Object) || contextLifetime == ContextType.Object)
                return dependencyInstance
                       ?? throw new Exception($"Unable to retrieve dependency {dependencyType.Name}. It may not be registered or you may have specified the injection parameters incorrectly");
            
            var contexts = GetContexts(contextLifetime);
                
            GetDependencyFromContexts(dependencyType, injectAttr, contexts, out dependencyInstance);

            return dependencyInstance 
                  ?? throw new Exception($"Unable to retrieve dependency {dependencyType.Name}. It may not be registered or you may have specified the injection parameters incorrectly");
        }
        
        private bool GetDependencyFromContainer(Type dependencyType, Inject injectAttr, ContainerView container, out object dependencyInstance)
        {
            var tag = injectAttr.Tag;
            
            dependencyInstance = ResolveDependency(dependencyType, tag, container);
            return dependencyInstance is not null;
        }
        
        private bool GetDependencyFromContexts(Type dependencyType, Inject injectAttr,  GameContext[] contexts, out object dependencyInstance)
        {
            dependencyInstance = null;
            var tag = injectAttr.Tag;
            
            foreach (var context in contexts)
            {
                var currentContainer = context.Container;
                dependencyInstance = ResolveDependency(dependencyType, tag, currentContainer);

                if (dependencyInstance is not null)
                    return true;
            }
            
            return false;
        }

        private object ResolveDependency(Type dependencyType, string tag, ContainerView container)
        {
            if (container is null)
                return null;
            
            container.TryResolve(dependencyType, tag, out var instance);

            return instance;
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
            .Where(context => context.Lifetime != ContextLifetime.Object)
            .ToArray();
    }
}