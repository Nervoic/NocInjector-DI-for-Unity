using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    public class DependencyInjector
    {
        private readonly ComponentInjector _componentInjector = new();
        private readonly ServiceInjector _serviceInjector = new();

        public void Inject(ObjectContext context)
        {
            try
            {
                var components = context.GetComponents<Component>().Where(c => c is not null).ToArray();
                RegisterComponents(components, context.ComponentContainer);
                
                foreach (var component in components)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject)) || m.IsDefined(typeof(InjectImplementation))))
                    {
                        if (_componentInjector.InjectToComponent(component, injectableMember, context)) 
                            continue;
                        
                        _serviceInjector.InjectToField(component, injectableMember);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during object injection: {e.Message}");
            }
        }

        private void RegisterComponents(Component[] components, ComponentContainer container)
        {
            foreach (var component in components)
            {
                if (component.GetType() == typeof(ObjectContext)) continue;
                
                var type = component.GetType();
                if (type.IsDefined(typeof(RegisterComponentAsImplementation)))
                {
                    var registerComponentAsImplementationAttr = type.GetCustomAttribute<RegisterComponentAsImplementation>();
                    var interfaceType = registerComponentAsImplementationAttr.InterfaceType;
                    var implementationTag = registerComponentAsImplementationAttr.RealisationTag;
                    
                    container.Register(type, component).AsImplementation(interfaceType, implementationTag);
                    continue;
                }
                container.Register(type, component);
            }
        }
    }
}
