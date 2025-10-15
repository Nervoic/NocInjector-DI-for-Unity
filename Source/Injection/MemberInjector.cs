using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class MemberInjector
    {
        public void InjectToMember(MemberInfo injectableMember, object instance, ContainerView containerView = null)
        {
            var memberType = injectableMember.GetMemberType();
            var injectAttr = injectableMember.GetCustomAttribute<Inject>();
            
            if (memberType == typeof(MethodInfo))
                InjectToMethod(injectableMember.ToMethod(), instance, injectAttr, containerView);
            else 
                InjectToField(injectableMember, instance, injectAttr, containerView);
        }

        private void InjectToField(MemberInfo injectableField, object instance, Inject injectAttr, ContainerView containerView)
        {
            var dependencyType = injectableField.GetFieldType();
            
            if (dependencyType.IsArray)
                throw new Exception($"Cannot inject an array into the {injectableField.Name} in {instance.GetType().Name}");

            var dependencyInstance = ResolveInstance(dependencyType, injectAttr, containerView);
            injectableField.SetValue(instance, dependencyInstance);
        }
        
        private void InjectToMethod(MethodInfo method, object instance, Inject injectAttr, ContainerView containerView)
        {
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;

                if (dependencyType.IsArray)
                    throw new Exception($"Cannot inject an array into the {method.Name} method in {instance.GetType().Name}");
                
                var dependencyInstance = ResolveInstance(dependencyType, injectAttr, containerView);
                
                values[i] = dependencyInstance;
            }

            method.Invoke(instance, values);

        }
        
        private object ResolveInstance(Type dependencyType, Inject injectAttr, ContainerView containerView = null)
        {
            var tags = injectAttr.Tags;
            var contextType = injectAttr.InjectContextLifetime;
            
            ResolveByTags(tags, dependencyType, containerView, out var instance);
                        
            if (instance is null && contextType != InjectContextLifetime.Object)
            {
                var gameContexts = GetContexts(contextType);

                foreach (var gameContext in gameContexts)
                {
                    ResolveByTags(tags, dependencyType, gameContext.Container, out instance);
                    
                    if (instance is not null)
                        break;
                }
            }

            return instance;
        }

        private void ResolveByTags(string[] tags, Type dependencyType, ContainerView containerView, out object instance)
        {
            instance = null;
            
            if (containerView is null)
                return;
            
            if (!tags.Any())
                containerView.TryResolve(dependencyType, null, out instance);
            else
            {
                foreach (var currentTag in tags)
                {
                    containerView.TryResolve(dependencyType, currentTag, out instance);

                    if (instance is not null)
                        return;
                }
            }
        }
        
        private GameContext[] GetContexts(InjectContextLifetime injectContextLifetime)
        {
            var gameContexts = Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None);
                
            if (injectContextLifetime is not InjectContextLifetime.All)
            {
                gameContexts = gameContexts.Where(c => injectContextLifetime == InjectContextLifetime.Project
                    ? c.Lifetime == ContextLifetime.Project
                    : c.Lifetime == ContextLifetime.Scene).ToArray();
            }

            return gameContexts;
        }
    }
}