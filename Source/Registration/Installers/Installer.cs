using UnityEngine;

namespace NocInjector
{

    public abstract class Installer : MonoBehaviour
    {
        /// <summary>
        /// Installs dependencies into the provided container
        /// </summary>
        public abstract void Install(ContainerView container);
    }
}
