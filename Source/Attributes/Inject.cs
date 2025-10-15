using System;

namespace NocInjector
{
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class Inject : Attribute
    {
        /// <summary>
        /// Injected tags
        /// </summary>
        public string[] Tags { get; }
        
        /// <summary>
        /// The type of context from which the dependency will be injected. By default, All
        /// </summary>
        public InjectContextLifetime InjectContextLifetime { get; }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        /// <param name="tag">Dependency ID</param>
        /// <param name="injectContextLifetime">The type of context from which the dependency will be injected. By default, All</param>
        
        public Inject(string tag = null, InjectContextLifetime injectContextLifetime = InjectContextLifetime.All)
        {
            Tags = new[] { tag };
            InjectContextLifetime = injectContextLifetime;
        }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        /// <param name="injectContextLifetime">The type of context from which the dependency will be injected. By default, All</param>
        
        public Inject(InjectContextLifetime injectContextLifetime = InjectContextLifetime.All)
        {
            Tags = Array.Empty<string>();
            InjectContextLifetime = injectContextLifetime;
        }

        public Inject(InjectContextLifetime injectContextLifetime, params string[] tags)
        {
            InjectContextLifetime = injectContextLifetime;
            Tags = tags;
        }

        public Inject(params string[] tags)
        {
            Tags = tags;
        }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        public Inject()
        {
            Tags = Array.Empty<string>();
            InjectContextLifetime = InjectContextLifetime.All;
        }
        
    }
}
