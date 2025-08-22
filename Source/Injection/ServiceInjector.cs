using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    public class ServiceInjector
    {
        private Context[] _contexts;
        public void Inject(object obj, MemberInfo injectableMember)
        {
            try
            {
                var typeToInjection = injectableMember.GetMemberType();
                if (typeToInjection.IsSubclassOf(typeof(Component)))
                {
                    Debug.LogError("Inject component to service error.");
                    return;
                }
                var currentContext = SelectContext(typeToInjection);
                
                if (typeToInjection.IsInterface)
                {
                    InjectAsInterface(injectableMember, currentContext, obj);
                    return;
                }

                if (currentContext is not null) injectableMember.SetValue(obj, currentContext.Container.Resolve(typeToInjection));
                else
                {
                    if (typeToInjection.IsDefined(typeof(Register)))
                    {
                        currentContext = RegisterService(typeToInjection);

                        if (currentContext is not null) injectableMember.SetValue(obj, currentContext.Container.Resolve(typeToInjection));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during service injection: {e.Message}");
            }
        }

        private void InjectAsInterface(MemberInfo injectableMember, Context currentContext, object obj)
        {
            var typeToInjection = injectableMember.GetMemberType();
            var injectByInterfaceAttr = injectableMember.GetCustomAttribute<InjectByInterface>();

            var realisationTag = injectByInterfaceAttr.RealisationTag;

            foreach (var context in _contexts)
            {
                if (context.Container.ResolveByInterface(typeToInjection, realisationTag) is not null) 
                    currentContext = context;
            }

            if (currentContext is null)
            {
                Debug.LogError($"Please register realisations to {typeToInjection.Name} interface with installers");
                return;
            }

            var realisation = currentContext.Container.ResolveByInterface(typeToInjection, realisationTag);
            if (!typeToInjection.IsAssignableFrom(realisation.GetType())) 
                Debug.LogError($"{realisation.GetType().Name} it is not inherited from the {typeToInjection.Name} interface.");
                    
            injectableMember.SetValue(obj, realisation);
        }

        private Context SelectContext(Type typeToInjection)
        {
            _contexts ??= Object.FindObjectsByType<Context>(FindObjectsSortMode.None);

            if (_contexts.Length == 0)
            {
                Debug.LogError("No Context found in the scene. Please add a Context before attempting to inject dependencies.");
                return null;
            }

            foreach (var context in _contexts)
            {
                if (!context.Installed) context.Install();
                    
                if (context.Container.Has(typeToInjection))
                {
                    if (typeToInjection.IsDefined(typeof(Register))) Debug.LogWarning($"Use only install or register attribute for {typeToInjection.Name}");
                    return context;
                }
            }

            return null;
        }

        private Context RegisterService(Type typeToInjection)
        {
            if (!typeToInjection.IsDefined(typeof(Register)))
            {
                Debug.LogError($"Service type '{typeToInjection.Name}' must be registered before injection. Please annotate it with [Register].");
                return null;
            }
            
            var registerAttr = typeToInjection.GetCustomAttribute<Register>();

            var lifetime = registerAttr.Lifetime;
            var contextLifetime = registerAttr.Context;

            var context = GetContextByLifetime(contextLifetime, _contexts);
            
            if (context is null) throw new Exception($"Failed to register service type '{typeToInjection.Name}': no matching Context found in the scene for the specified lifetime.");

            context.Container.Register(typeToInjection, lifetime);

            return context;
        }

        private Context GetContextByLifetime(ContextLifetime lifetime, Context[] contexts)
        {
            return contexts.First(c => c.Lifetime == lifetime);
        }
    }
}