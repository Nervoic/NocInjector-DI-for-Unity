using NocInjector.Calls;

namespace NocInjector
{
    internal abstract class LifetimeImplementation
    {
        public abstract Lifetime Lifetime { get; protected set; }
        public abstract void Resolve(DependencyInfo dependency, CallView systemView, out object instance);
    }
}