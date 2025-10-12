using System;
using UnityEngine;

namespace NocInjector
{

    public abstract class Installer : MonoBehaviour
    {
        [Header("Installer settings")]
        [Tooltip("Destroys the Installer after Installing dependencies")]
        [SerializeField] protected bool destroyAfterInstall;
        
        private ContainerView _container;
        
        /// <summary>
        /// Installs dependencies into the provided container
        /// </summary>
        protected abstract void Install();

        protected ContainerRegister Register(Type typeToRegister, Lifetime lifetime = Lifetime.Singleton)
        {
            return _container.Register(typeToRegister, lifetime);
        }
        
        protected ContainerRegister Register<T>(Lifetime lifetime = Lifetime.Singleton)
        {
            return _container.Register<T>(lifetime);
        }

        public void Initialize(ContainerView container)
        {
            _container = container;
            
            Install();

            if (destroyAfterInstall)
                DestroyImmediate(this);
        }
    }
}
