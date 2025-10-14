using System.Reflection;

namespace NocInjector
{
    internal class InstanceResolvedCall
    {
        public MemberInfo[] InjectableMembers { get; }
        public object Obj { get; }
        public InstanceResolvedCall(MemberInfo[] injectableMembers, object obj)
        {
            InjectableMembers = injectableMembers;
            Obj = obj;
        }
    }
}