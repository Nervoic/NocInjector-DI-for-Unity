using System;
using System.Reflection;

namespace NocInjector
{
    internal interface IInjector
    {
        public InjectorType InjectionType { get; }

        public void Inject(MemberInfo injectableMember, object instance, Context context = null);
    }
}