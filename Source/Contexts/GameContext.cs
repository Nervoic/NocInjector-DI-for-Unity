using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    
    public class GameContext : Context
    {
        [SerializeField] private ContextLifetime lifetime;
        public override DependencyContainer Container { get; protected set; }

        /// <summary>
        /// Gets the lifetime of the context.
        /// </summary>
        public ContextLifetime Lifetime => lifetime;
        

        private void Awake()
        {
            if (lifetime is ContextLifetime.Project)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        protected override void Install()
        {
            try
            {
                Container = new DependencyContainer(gameObject);

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
