using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal abstract class Injector
    {
        protected object GetInstance(Type dependencyType, Inject injectAttr, ContainerView containerView = null)
        {
            if (dependencyType.IsArray)
                throw new Exception($"Cannot inject array of {dependencyType.Name}");
            
            var tags = injectAttr.Tags;
            var contextType = injectAttr.ContextType;
            
            var instance = ResolveByTags(tags, dependencyType, containerView);
                        
            if (instance is null && contextType != ContextType.Object)
            {
                var gameContexts = GetContexts(contextType);

                foreach (var gameContext in gameContexts)
                {
                    instance = ResolveByTags(tags, dependencyType, gameContext.Container);
                    
                    if (instance is not null)
                        break;
                }
            }

            return instance;
        }

        private object ResolveByTags(string[] tags, Type dependencyType, ContainerView containerView = null)
        {
            object instance = null;

            if (!tags.Any())
                containerView?.TryResolve(dependencyType, null, out instance);
            else
            {
                foreach (var currentTag in tags)
                {
                    containerView?.TryResolve(dependencyType, currentTag, out instance);

                    if (instance is not null)
                        break;
                }
            }

            return instance;
        }
        
        private GameContext[] GetContexts(ContextType contextType)
        {
            var gameContexts = Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None);
                
            if (contextType is not ContextType.All)
            {
                gameContexts = gameContexts.Where(c => contextType == ContextType.Project
                    ? c.Lifetime == ContextLifetime.Project
                    : c.Lifetime == ContextLifetime.Scene).ToArray();
            }

            return gameContexts;
        }
    }
}