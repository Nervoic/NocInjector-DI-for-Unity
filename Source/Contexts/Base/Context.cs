
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
        public abstract ContainerView Container { get; protected set; }
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        protected bool Installed { get; private set; }
        
        protected CallView SystemView { get; private set; }

        public void InstallContext(CallView systemView)
        {
            if (!Installed)
            {
                SystemView = systemView;
                Install();
                
                Installed = true;
                
            }
        }

        public void InstallNew(Installer[] newInstallers)
        {
            foreach (var installer in newInstallers)
            {
                installer.Initialize(Container);
            }
        }
        
        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        protected abstract void Install();
    }
}