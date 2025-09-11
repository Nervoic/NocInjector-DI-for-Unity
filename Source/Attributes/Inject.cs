using System;

namespace NocInjector
{
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class Inject : Attribute
    {
        
        /// <summary>
        /// Dependency ID
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// The type of context from which the dependency will be injected. By default, All
        /// </summary>
        public ContextType ContextType { get; }
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        /// <param name="id">Dependency ID</param>
        /// <param name="contextType">The type of context from which the dependency will be injected. By default, All</param>
        
        public Inject(string id = null, ContextType contextType = ContextType.All)
        {
            Id = id;
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
        
        /// <summary>
        /// Used for automatic injection of dependencies into fields and properties
        /// </summary>
        public Inject()
        {
            Id = null;
            ContextType = ContextType.All;
        }
    }
}
