
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
        
    }
}
