using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal sealed class MemberInjector
    {
        private GameContext[] _contexts;

        internal MemberInjector()
        {
            InitContexts();
        }
        public void InjectToMember(MemberInfo injectableMember, object instance, ContainerView objectContainer = null)
        {
            var memberType = injectableMember.GetMemberType();
            var injectAttr = injectableMember.GetCustomAttribute<Inject>();
            
            if (memberType == typeof(MethodInfo))
                InjectToMethod(injectableMember.ToMethod(), instance, injectAttr, objectContainer);
            else 
                InjectToField(injectableMember, instance, injectAttr, objectContainer);
        }

        private void InjectToField(MemberInfo injectableField, object instance, Inject injectAttr, ContainerView containerView)
        {
            var dependencyType = injectableField.GetFieldType();
            
            if (dependencyType.IsArray)
                throw new Exception($"Cannot inject an array into the {injectableField.Name} in {instance.GetType().Name}");

            var dependencyInstance = ResolveDependency(dependencyType, injectAttr, containerView);
            injectableField.SetValue(instance, dependencyInstance);
        }
        
        private void InjectToMethod(MethodInfo method, object instance, Inject injectAttr, ContainerView objectContainer)
        {
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;

                if (dependencyType.IsArray)
                    throw new Exception($"Cannot inject an array into the {method.Name} method in {instance.GetType().Name}");
                
                var dependencyInstance = ResolveDependency(dependencyType, injectAttr, objectContainer);
                
                values[i] = dependencyInstance;
            }

            method.Invoke(instance, values);

        }
        
        private object ResolveDependency(Type dependencyType, Inject injectAttr, ContainerView objectContainer = null)
        {
            var tags = injectAttr.Tags;
            var injectLifetime = injectAttr.InjectContextLifetime;
            
            TryResolveByTags(tags, dependencyType, objectContainer, out var instance);
                        
            if (instance is null && injectLifetime != InjectContextLifetime.Object)
            {
                var gameContexts = GetContextsByLifetime(injectLifetime);

                foreach (var gameContext in gameContexts)
                {
                    TryResolveByTags(tags, dependencyType, gameContext.Container, out instance);
                    
                    if (instance is not null)
                        break;
                }
            }

            return instance;
        }

        private void TryResolveByTags(string[] tags, Type dependencyType, ContainerView objectContainer, out object instance)
        {
            instance = null;
            
            if (objectContainer is null)
                return;
            
            if (!tags.Any())
                objectContainer.TryResolve(dependencyType, null, out instance);
            else
            {
                foreach (var currentTag in tags)
                {
                    objectContainer.TryResolve(dependencyType, currentTag, out instance);

                    if (instance is not null)
                        return;
                }
            }
        }
        
        private GameContext[] GetContextsByLifetime(InjectContextLifetime injectContextLifetime)
        {
            if (injectContextLifetime is InjectContextLifetime.All) return _contexts;
            
            var selectedContexts = _contexts.Where(c => injectContextLifetime == InjectContextLifetime.Project
                ? c.Lifetime == ContextLifetime.Project
                : c.Lifetime == ContextLifetime.Scene).ToArray();

            return selectedContexts;
        }
        
        private void InitContexts() => _contexts = Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None);
    }
}