
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    public abstract class Context : MonoBehaviour
    {
        [Header("Installation")]
        [SerializeField] protected Installer[] installers;
        
        /// <summary>
        /// Container associated with this context.
        /// </summary>
        public ContainerView Container { get; private set; }
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        protected bool Installed { get; private set; }
        
        protected CallView SystemView { get; private set; }

        public void InstallContext(ContextManager manager)
        {
            if (Installed) 
                return;

            SystemView = manager.SystemView;
            InitializeContainer();
            Install();
            
            Installed = true;
            
        }

        private void InitializeContainer()
        {
            Container = new ContainerView(SystemView);
        }
        
        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        protected abstract void Install();
    }
}