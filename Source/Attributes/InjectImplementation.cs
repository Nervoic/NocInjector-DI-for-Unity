using System;

namespace NocInjector
{
    /// <summary>
    /// It implements an object as an interface implementation using the tag. Services require mandatory type registration in the context
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectImplementation : Attribute
    {
        /// <summary>
        /// The implementation tag that you want to implement
        /// </summary>
        public string ImplementationTag { get; }
        
        public InjectImplementation(string implementationTag)
        {
            ImplementationTag = implementationTag;
        }
    }
}
