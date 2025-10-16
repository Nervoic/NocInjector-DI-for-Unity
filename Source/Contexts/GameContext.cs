using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    
    public sealed class GameContext : Context
    {
        [SerializeField] private ContextLifetime lifetime = ContextLifetime.Object;

        /// <summary>
        /// Gets the lifetime of the context.
        /// </summary>
        public ContextLifetime Lifetime => lifetime;
        

        private void Awake()
        {
            if (lifetime is ContextLifetime.Project)
                DontDestroyOnLoad(gameObject);
        }
        protected override void Install()
        {
            try
            {
                foreach (var installer in installers.Where(i => i is not null)) 
                    installer.Initialize(Container);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
