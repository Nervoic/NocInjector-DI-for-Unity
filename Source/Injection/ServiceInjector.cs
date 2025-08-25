using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    public class ServiceInjector
    {
        private GameContext[] _contexts;
        public void InjectToField(object obj, MemberInfo injectableMember)
        {
            FindContexts();
            
            var serviceToInjection = injectableMember.GetMemberType();
            try
            {
                if (HasInjectError(serviceToInjection, injectableMember, obj))
                    return;
                
                if (serviceToInjection.IsInterface)
                {
                    InjectAsInterface(injectableMember, obj);
                    return;
                }
                
                var currentContext = SelectContext(serviceToInjection);
                
                if (currentContext is not null) 
                    injectableMember.SetValue(obj, currentContext.Container.Resolve(serviceToInjection));
                else
                    Debug.LogError($"{serviceToInjection.Name} is not registered in any of the {nameof(GameContext)}s. Register it before injection.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during service injection: {e.Message}");
            }
        }

        private void InjectAsInterface(MemberInfo injectableMember, object obj)
        {
            var interfaceType = injectableMember.GetMemberType();
            var injectImplementationAttr = injectableMember.GetCustomAttribute<InjectImplementation>();

            var implementationTag = injectImplementationAttr.ImplementationTag;

            GameContext currentGameContext = null;

            foreach (var context in _contexts)
            {
                if (context.Container.ResolveImplementation(implementationTag) is not null)
                    currentGameContext = context;
            }

            if (currentGameContext is null)
            {
                Debug.LogError($"Please register realisations to {interfaceType.Name} interface with installers.");
                return;
            }

            var implementation = currentGameContext.Container.ResolveImplementation(implementationTag);
            if (!interfaceType.IsAssignableFrom(implementation.GetType())) 
                Debug.LogError($"{implementation.GetType().Name} it is not inherited from the {interfaceType.Name} interface.");
                    
            injectableMember.SetValue(obj, implementation);
        }

        private GameContext SelectContext(Type serviceToInjection)
        {
            foreach (var context in _contexts)
            {
                if (context.Container.Has(serviceToInjection))
                    return context;
            }

            return null;
        }

        private void FindContexts()
        {
            _contexts ??= Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None);
            
            if (_contexts.Length == 0)
                Debug.LogError("No GameContext found in the scene. Please add a GameContext before attempting to inject dependencies.");
        }

        private bool HasInjectError(Type typeToInjection, MemberInfo injectableMember, object obj)
        {
            if (typeToInjection.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {obj.GetType().Name}");
                return true;
            }
                
            if (typeToInjection.IsSubclassOf(typeof(Component)))
            {
                Debug.LogError($"Inject component {typeToInjection.Name} on {obj.GetType().Name} as service error.");
                return true;
            }

            return false;
        }
    }
}