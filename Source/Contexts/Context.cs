using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Represents a dependency injection context in the scene or project.
    /// </summary>
    public class Context : MonoBehaviour
    {
        [SerializeField] private ContextLifetime lifetime;

        [SerializeField] private Installer[] installers;
        
        /// <summary>
        /// Service container associated with this context.
        /// </summary>
        public ServiceContainer Container { get; private set; }

        /// <summary>
        /// Gets the lifetime of the context.
        /// </summary>
        public ContextLifetime Lifetime => lifetime;
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        public bool Installed { get; private set; }
        

        private void Awake()
        {
            if (lifetime is ContextLifetime.Project) DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        public void Install()
        {
            try
            {
                Container = new ServiceContainer();

                foreach (var installer in installers.Where(i => i is not null))
                {
                    installer.Install(Container);
                }

                Installed = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
