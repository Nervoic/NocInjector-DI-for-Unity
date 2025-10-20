using System.Collections.Generic;
using System.Linq;

namespace NocInjector
{
    internal class LifetimeFactory
    {
        private readonly List<LifetimeImplementation> _implementations = new()
        {
            new SingletonImplementation(), 
            new TransientImplementation()
        };

        public LifetimeImplementation GetLifetime(Lifetime lifetime) => _implementations.FirstOrDefault(implementation => implementation.Lifetime == lifetime);
    }
}
