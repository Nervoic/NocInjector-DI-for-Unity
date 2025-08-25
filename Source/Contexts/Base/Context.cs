using System;
using UnityEngine;

namespace NocInjector
{
    public abstract class Context : MonoBehaviour
    {
        [SerializeField] protected Installer[] serviceInstallers;
        
        /// <summary>
        /// Indicates whether the context has been installed.
        /// </summary>
        protected bool Installed { get; private set; }
        
        /// <summary>
        /// Installs all services using the provided installers.
        /// </summary>
        public abstract void Install();
    }
}