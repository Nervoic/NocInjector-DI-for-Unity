
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NocInjector
{
    internal sealed class MemberInjectorFactory
    {
        private readonly List<MemberInjector> _injectors = new()
        {
            new FieldInjector(),
            new MethodInjector(),
            new PropertyInjector()
        };
        
        public MemberInjector GetInjector(InjectorType injectionType) => _injectors.FirstOrDefault(injector => injector.InjectorType == injectionType);
    }
}