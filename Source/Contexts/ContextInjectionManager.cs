
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public class ContextInjectionManager
    {
        private readonly Injector _injector;
        private readonly ComponentInjector _componentInjector;
        public GameObject CurrentInjected { get; private set; }

        public ContextInjectionManager(CallView callView)
        {
            _injector = new Injector(callView);
            _componentInjector = new ComponentInjector(_injector);
        }
        
        public void InjectTo(GameObject injectableObject)
        {
            CurrentInjected = injectableObject;
            
            _componentInjector.InjectTo(injectableObject);
        }

        public void InjectTo(object instance) => _injector.InjectTo(instance);
    }
}