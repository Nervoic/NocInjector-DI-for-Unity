using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    public class ObjectInjector
    {
        private ServiceInjector _currentServiceInjector;

        public void Inject(Component[] objectComponents, ObjectContainer container)
        {
            try
            {
                RegisterComponents(objectComponents, container);

                foreach (var component in objectComponents)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject)) || m.IsDefined(typeof(InjectByInterface))))
                    {
                        var typeToInjection = injectableMember.GetMemberType();
                        
                        if (typeToInjection.IsSubclassOf(typeof(Component)))
                            InjectAsComponent(injectableMember, component, container);
                        else if (typeToInjection.IsInterface)
                            InjectAsInterface(injectableMember, component, container);
                        else 
                            InjectAsService(component, injectableMember);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during object injection: {e.Message}");
            }
        }

        private void InjectAsComponent(MemberInfo injectableMember, Component component, ObjectContainer container)
        {
            var typeToInjection = injectableMember.GetMemberType();
            if (injectableMember.IsDefined(typeof(InjectByInterface)))
            {
                Debug.LogError($"Inject by interface on {component.name}, injection - {typeToInjection.Name} works only for interfaces");
                return;
            }
                            
            if (injectableMember.GetType().IsDefined(typeof(Register))) Debug.LogWarning($"Component {component.name} does not register.");
            var componentToInjection = container.Resolve(typeToInjection);
                            
            injectableMember.SetValue(component, componentToInjection);
        }

        private void InjectAsInterface(MemberInfo injectableMember, Component component, ObjectContainer container)
        {
            var typeToInjection = injectableMember.GetMemberType();
            if (injectableMember.IsDefined(typeof(Inject)))
            {
                Debug.LogError($"Inject attribute on {component.name}, injection - {typeToInjection.Name} works only for fields and properties");
                return;
            }

            var injectByInterfaceAttr = injectableMember.GetCustomAttribute<InjectByInterface>();
            var realisationTag = injectByInterfaceAttr.RealisationTag;
            var realisation = container.ResolveByInterface(typeToInjection, realisationTag);

            if (realisation is not null)
            {
                if (!typeToInjection.IsAssignableFrom(realisation.GetType()))
                {
                    Debug.LogError($"{realisation.GetType().Name} it is not inherited from the {typeToInjection.Name} interface.");
                    return;
                }
                                
                injectableMember.SetValue(component, realisation);
            }
            else 
                InjectAsService(component, injectableMember);
        }

        private void InjectAsService(object obj, MemberInfo member)
        {
            _currentServiceInjector ??= new ServiceInjector();
            _currentServiceInjector.Inject(obj, member);
        }

        private void RegisterComponents(Component[] components, ObjectContainer container)
        {
            foreach (var component in components)
            {
                if (component.GetType() == typeof(InjectObject) || component.Equals(null)) continue;
                
                var type = component.GetType();
                container.Register(type, component);
            }
        }
    }
}
