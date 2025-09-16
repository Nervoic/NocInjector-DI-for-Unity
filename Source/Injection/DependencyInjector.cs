
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class DependencyInjector
    {
        public void InjectToObject(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>().Where(c => c is not null).ToArray();
                foreach (var component in components)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject))))
                    {
                        var context = gameObject.GetComponent<Context>();
                        InjectToMember(injectableMember, component, context?.Container);
                    }
                }
        }

        public void InjectToMember(MemberInfo injectableMember, object obj, ContainerView containerView = null)
        {
            if (injectableMember is MethodInfo method)
            {
                InjectToMethod(method, obj, containerView);
                return;
            }
            
            var dependencyType = injectableMember.GetMemberType();

            var injectAttr = injectableMember.GetCustomAttribute<Inject>();
            
            var instance = GetInstance(dependencyType, containerView, injectAttr);
                        
            injectableMember.SetValue(obj, instance);
        }

        private void InjectToMethod(MethodInfo method, object obj, ContainerView containerView)
        {
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            var injectAttr = method.GetCustomAttribute<Inject>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;
                
                var instance = GetInstance(dependencyType, containerView, injectAttr);
                
                values[i] = instance;
            }

            method.Invoke(obj, values);

        }

        private object GetInstance(Type dependencyType, ContainerView containerView, Inject injectAttr)
        {
            if (dependencyType.IsArray)
                throw new Exception($"Cannot inject array of {dependencyType.Name}");
            
            var tag = injectAttr.Tag;
            var tags = injectAttr.Tags;
            var contextType = injectAttr.ContextType;
            
            var instance = ResolveByTag(tag, tags, dependencyType, containerView);
                        
            if (instance is null && contextType != ContextType.Object)
            {
                var gameContexts = GetContexts(contextType);

                foreach (var gameContext in gameContexts)
                {
                    instance = ResolveByTag(tag, tags, dependencyType, gameContext.Container);
                    
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

        private object ResolveByTag(string tag, string[] tags, Type dependencyType, ContainerView containerView)
        {
            object instance = null;
            
            if (tags is not null)
            {
                foreach (var currentTag in tags)
                {
                    containerView?.TryResolve(dependencyType, currentTag, out instance);
                    
                    if (instance is not null)
                        break;
                }
            } 
            else 
                containerView?.TryResolve(dependencyType, tag, out instance);

            return instance;
        }
        
    }
}
