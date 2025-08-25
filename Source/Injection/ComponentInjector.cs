
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    public class ComponentInjector
    {
        public bool InjectToComponent(Component component, MemberInfo injectableMember, ObjectContext context)
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
            if (injectableMember.IsDefined(typeof(InjectImplementation)))
            {
                Debug.LogError($"Inject by interface on {component.name}, injection to {typeToInjection.Name} works only for interfaces");
                return false;
            }

            if (typeToInjection.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {component.GetType().Name}");
                return false;
            }
            
            var componentToInjection = container.Resolve(typeToInjection);
                            
            injectableMember.SetValue(component, componentToInjection);
            if (componentToInjection is null)
                Debug.LogError($"You forgot to add the {typeToInjection.Name} component to {component.gameObject.name}.");


            return true;
        }

        private bool InjectAsInterface(MemberInfo injectableMember, Component component, ComponentContainer container)
        {
            var interfaceType = injectableMember.GetMemberType();
            if (injectableMember.IsDefined(typeof(Inject)))
            {
                Debug.LogError($"{nameof(Inject)} attribute on {component.name}, injection - {interfaceType.Name} works only for fields and properties");
                return false;
            }
            
            if (interfaceType.IsArray)
            {
                Debug.LogError($"Cannot inject an array into {injectableMember.Name}, component {component.GetType().Name}");
                return false;
            }

            var injectImplementationAttr = injectableMember.GetCustomAttribute<InjectImplementation>();
            var implementationTag = injectImplementationAttr.ImplementationTag;
            
            var implementation = container.ResolveImplementation(implementationTag);
            
            injectableMember.SetValue(component, implementation);

            return implementation is not null;

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
                return InjectServiceAsInterface(injectableMember, component, container);

            if (!container.Has(typeToInjection))
                return false;

            var serviceToInjection = container.Resolve(typeToInjection);
            injectableMember.SetValue(component, serviceToInjection);

            return serviceToInjection is not null;

        }

        private bool InjectServiceAsInterface(MemberInfo injectableMember, Component component, ServiceContainer container)
        {
            var injectAsImplementationAttr = injectableMember.GetCustomAttribute<InjectImplementation>();
            var implementationTag = injectAsImplementationAttr.ImplementationTag;
            
            var implementation = container.ResolveImplementation(implementationTag);
                
            injectableMember.SetValue(component, implementation);

            return implementation is not null;
        }
    }
}
