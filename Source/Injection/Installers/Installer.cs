using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// Abstract installer for registering services in a ServiceContainer.
    /// </summary>
    public abstract class Installer : MonoBehaviour
    {
        /// <summary>
        /// Installs services into the provided ServiceContainer.
        /// </summary>
        public abstract void Install(ServiceContainer container);
    }
}
