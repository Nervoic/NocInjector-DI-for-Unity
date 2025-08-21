using System;

namespace NocInjector
{
    /// <summary>
    /// Attribute for marking a class as a service to be registered as interface realisation.
    /// Mandatory manual installation of this class in the context
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterByInterface : Attribute
    {
        /// <summary>
        /// The interface that this class implements
        /// </summary>
        public Type InterfaceType { get; }
        
        /// <summary>
        /// Tag for implementation
        /// </summary>
        public string RealisationTag { get; }
        public RegisterByInterface(Type interfaceType, string realisationTag)
        {
            InterfaceType = interfaceType;
            RealisationTag = realisationTag;
        }
    }
}