using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    public class InjectorManager
    {
        private readonly ComponentInjector _componentInjector = new ComponentInjector();
        private readonly ServiceInjector _serviceInjector = new ServiceInjector();

        public void Inject(ObjectContext context)
        {
            try
            {
                var components = context.GetComponents<Component>().Where(c => c is not null).ToArray();
                RegisterComponents(components, context.ComponentContainer);
                
                foreach (var component in components)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject)) || m.IsDefined(typeof(InjectByRealisation))))
                    {
                        if (_componentInjector.Inject(component, injectableMember, context)) 
                            continue;
                        
                        _serviceInjector.Inject(component, injectableMember);
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
                container.Register(type, component);
            }
        }
    }
}
