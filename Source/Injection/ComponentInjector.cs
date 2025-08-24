
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    public class ComponentInjector
    {
        public bool Inject(Component component, MemberInfo injectableMember, ObjectContext context)
        {
            var typeToInjection = injectableMember.GetMemberType();
            var injected = false;
            
            if (typeToInjection.IsSubclassOf(typeof(Component)))
                injected = InjectAsComponent(injectableMember, component, context.ComponentContainer);
            else if (typeToInjection.IsInterface)
                injected = InjectAsInterface(injectableMember, component, context.ComponentContainer);
            if (!injected)
                injected = InjectAsService(injectableMember, component, context.ServiceContainer);

            return injected;
        }

        private bool InjectAsComponent(MemberInfo injectableMember, Component component, ComponentContainer container)
        {
            var typeToInjection = injectableMember.GetMemberType();
            if (injectableMember.IsDefined(typeof(InjectByRealisation)))
            {
                Debug.LogError($"Inject by interface on {component.name}, injection - {typeToInjection.Name} works only for interfaces");
                return false;
            }

            if (typeToInjection.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {component.GetType().Name}");
                return false;
            }
                            
            if (injectableMember.GetType().IsDefined(typeof(Register))) Debug.LogWarning($"Component {component.name} does not register.");
            var componentToInjection = container.Resolve(typeToInjection);
                            
            injectableMember.SetValue(component, componentToInjection);
            if (componentToInjection is null)
            {
                Debug.LogError($"You forgot to add the {typeToInjection.Name} component to {component.gameObject.name}.");
            }


            return true;
        }

        private bool InjectAsInterface(MemberInfo injectableMember, Component component, ComponentContainer container)
        {
            var typeToInjection = injectableMember.GetMemberType();
            if (injectableMember.IsDefined(typeof(Inject)))
            {
                Debug.LogError($"Inject attribute on {component.name}, injection - {typeToInjection.Name} works only for fields and properties");
                return false;
            }
            
            if (typeToInjection.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {component.GetType().Name}");
                return false;
            }

            var injectByInterfaceAttr = injectableMember.GetCustomAttribute<InjectByRealisation>();
            var realisationTag = injectByInterfaceAttr.RealisationTag;
            var realisation = container.ResolveByInterface(typeToInjection, realisationTag);

            if (realisation is not null)
            {
                if (!typeToInjection.IsAssignableFrom(realisation.GetType()))
                {
                    Debug.LogError($"{realisation.GetType().Name} it is not inherited from the {typeToInjection.Name} interface.");
                    return false;
                }
            }
            
            injectableMember.SetValue(component, realisation);

            return realisation is not null;

        }

        private bool InjectAsService(MemberInfo injectableMember, Component component, ServiceContainer container)
        {
            var typeToInjection = injectableMember.GetMemberType();
            
            if (typeToInjection.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {component.GetType().Name}");
                return false;
            }
            
            if (typeToInjection.IsInterface)
            {
                var injectByInterfaceAttr = injectableMember.GetCustomAttribute<InjectByRealisation>();
                var realisationTag = injectByInterfaceAttr.RealisationTag;
                var realisation = container.ResolveByInterface(typeToInjection, realisationTag);

                if (realisation is not null)
                {
                    if (!typeToInjection.IsAssignableFrom(realisation.GetType()))
                    {
                        Debug.LogError($"{realisation.GetType().Name} it is not inherited from the {typeToInjection.Name} interface.");
                        return false;
                    }
                }
                
                injectableMember.SetValue(component, realisation);

                return realisation is not null;
            }

            if (!container.Has(typeToInjection))
            {
                if (typeToInjection.IsDefined(typeof(Register)))
                {
                    var registerAttr = typeToInjection.GetCustomAttribute<Register>();

                    var serviceLifeTime = registerAttr.Lifetime;
                    var contextLifeTime = registerAttr.Context;

                    if (contextLifeTime is not RegisterToContextLifetime.Object)
                        return false;

                    container.Register(typeToInjection, serviceLifeTime);
                }
                else 
                    return false;
            }

            var serviceToInjection = container.Resolve(typeToInjection);
            injectableMember.SetValue(component, serviceToInjection);

            return serviceToInjection is not null;

        }
    }
}
