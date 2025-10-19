
using System;
using System.Linq;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public abstract class Context : MonoBehaviour
    {
        [Header("Installation")]
        [SerializeField] protected GameInstaller[] installers;
        
        /// <summary>
        /// Container associated with this context.
        /// </summary>
        public ContainerView Container { get; private set; }
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        protected bool Installed { get; private set; }

        public void InstallContext(CallView systemView)
        {
            if (Installed) 
                return;
            
            InitializeContainer(systemView);
            Install();
            
            Installed = true;
            
        }

        private void InitializeContainer(CallView systemView) => Container = new ContainerView(systemView);

        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        private void Install()
        {
            try
            {
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