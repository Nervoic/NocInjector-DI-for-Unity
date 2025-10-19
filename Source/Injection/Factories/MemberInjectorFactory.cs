
using System.Collections.Generic;
using System.Linq;

namespace NocInjector
{
    internal sealed class MemberInjectorFactory
    {
        private readonly List<IInjector> _injectors = new()
        {
            new FieldInjector(),
            new MethodInjector(),
            new PropertyInjector()
        };
        
        public IInjector GetInjector(InjectorType injectionType) => _injectors.FirstOrDefault(injector => injector.InjectionType == injectionType);
    }
}