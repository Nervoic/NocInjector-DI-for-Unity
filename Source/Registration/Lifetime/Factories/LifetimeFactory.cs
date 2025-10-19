using System.Collections.Generic;
using System.Linq;

namespace NocInjector
{
    internal class LifetimeFactory
    {
        private readonly List<ILifetimeImplementation> _implementations = new()
        {
            new SingletonImplementation(), 
            new TransientImplementation()
        };

        public ILifetimeImplementation GetLifetime(Lifetime lifetime) => _implementations.FirstOrDefault(implementation => implementation.Lifetime == lifetime);
    }
}
