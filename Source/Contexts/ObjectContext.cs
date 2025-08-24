using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// MonoBehaviour for injecting dependencies into attached components.
    /// </summary>
    public class ObjectContext : Context
    {
        /// <summary>
        /// Object container holding registered components.
        /// </summary>
        public ComponentContainer ComponentContainer { get; private set; }
        
        /// <summary>
        /// Object container holding installed services.
        /// </summary>
        public ServiceContainer ServiceContainer { get; private set; }
        
        protected override void Install()
        {
            try
            {
                ComponentContainer = new ComponentContainer(gameObject);
                ServiceContainer = new ServiceContainer();
                
                foreach (var installer in installers.Where(i => i is not null))
                    installer.Install(ServiceContainer);
                
                InjectToComponents();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        private void InjectToComponents()
        {
            new InjectorManager().Inject(this);
        }
    }
}
