
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public class ContextInjectionManager
    {
        private readonly Injector _injector;
        public GameObject CurrentInjected { get; private set; }

        public ContextInjectionManager(CallView callView)
        {
            _injector = new Injector(callView);
        }
        
        public void InjectTo(GameObject injectableObject)
        {
            CurrentInjected = injectableObject;
            _injector.InjectTo(injectableObject);
        }

        public void InjectTo(object instance) => _injector.InjectTo(instance);
    }
}