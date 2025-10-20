using System;

namespace NocInjector
{
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class Inject : Attribute
    {
        /// <summary>
        /// Injected tags
        /// </summary>
        public string Tag { get; }
        
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
            Tag = null;
            ContextType = contextType;
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
