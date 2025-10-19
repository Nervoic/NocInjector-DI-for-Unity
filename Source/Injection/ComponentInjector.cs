
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    internal sealed class ComponentInjector
    {
        private readonly Injector _injector;

        public ComponentInjector(Injector injector)
        {
            _injector = injector;
        }
        
        public void InjectTo(GameObject injectableObject)
        {
            var components = injectableObject.GetComponents<Component>().Where(c => c is not null).ToArray();
            
            foreach (var component in components)
                _injector.InjectTo(component, component.GetComponent<GameContext>());
        }
    }
}
