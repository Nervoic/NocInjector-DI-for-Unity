using System;
using UnityEngine;

namespace NocInjector
{
    public abstract class Context : MonoBehaviour
    {
        [SerializeField] protected Installer[] installers;
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        public bool Installed { get; private set; }

        protected virtual void Awake()
        {
            if (!Installed)
            {
                Install();
                Installed = true;
            }
        }
        
        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        protected abstract void Install();

        public void TryInstall()
        {
            if (!Installed)
                Install();
        }
    }
}