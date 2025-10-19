using NocInjector.Calls;

namespace NocInjector
{
    internal interface ILifetimeImplementation
    {
        public Lifetime Lifetime { get; }
        public void Resolve(DependencyInfo dependency, CallView systemView, out object instance);
    }
}