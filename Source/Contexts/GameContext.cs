using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Represents a dependency injection context in the scene or project.
    /// </summary>
    public class GameContext : Context
    {
        [SerializeField] private ContextLifetime lifetime;
        
        /// <summary>
        /// Service container associated with this context.
        /// </summary>
        public ServiceContainer Container { get; private set; }

        /// <summary>
        /// Gets the lifetime of the context.
        /// </summary>
        public ContextLifetime Lifetime => lifetime;
        

        protected override void Awake()
        {
            if (lifetime is ContextLifetime.Project) 
                DontDestroyOnLoad(gameObject);
        }
        protected override void Install()
        {
            try
            {
                Container = new ServiceContainer();

                foreach (var installer in installers.Where(i => i is not null)) 
                    installer.Install(Container);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
