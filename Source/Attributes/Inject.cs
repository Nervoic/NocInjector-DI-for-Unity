using System;

namespace NocInjector
{
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class Inject : Attribute
    {
        
        /// <summary>
        /// Dependency tag
        /// </summary>
        public string Tag { get; }
        
        public string[] Tags { get; }
        
        /// <summary>
        /// The type of context from which the dependency will be injected. By default, All
        /// </summary>
        public ContextType ContextType { get; }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        /// <param name="tag">Dependency ID</param>
        /// <param name="contextType">The type of context from which the dependency will be injected. By default, All</param>
        
        public Inject(string tag = null, ContextType contextType = ContextType.All)
        {
            Tag = tag;
            ContextType = contextType;
        }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        /// <param name="contextType">The type of context from which the dependency will be injected. By default, All</param>
        
        public Inject(ContextType contextType = ContextType.All)
        {
            ContextType = contextType;
        }

        public Inject(ContextType contextType, params string[] tags)
        {
            ContextType = contextType;
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
            Tag = null;
            ContextType = ContextType.All;
        }
        
    }
}
