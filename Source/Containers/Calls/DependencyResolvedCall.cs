using System.Reflection;

namespace NocInjector
{
    internal class DependencyResolvedCall
    {
        public MemberInfo[] InjectableMembers { get; }
        public object Instance { get; }
        public DependencyResolvedCall(MemberInfo[] injectableMembers, object instance)
        {
            InjectableMembers = injectableMembers;
            Instance = instance;
        }
    }
}