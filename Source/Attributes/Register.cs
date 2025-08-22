using System;

namespace NocInjector
{
    /// <summary>
    /// Attribute for marking a class as a service to be registered in the DI container.
    /// Specifies the service lifetime and context lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Register : Attribute
    {
        /// <summary>
        /// Gets the lifetime of the registered service.
        /// </summary>
        public ServiceLifetime Lifetime { get; }
        
        /// <summary>
        /// Gets the context lifetime in which the service will be available (Scene or Project).
        /// </summary>
        public ContextLifetime Context { get; }
        public Register(ServiceLifetime lifetime, ContextLifetime context)
        {
            Lifetime = lifetime;
            Context = context;
        }
    }
}
