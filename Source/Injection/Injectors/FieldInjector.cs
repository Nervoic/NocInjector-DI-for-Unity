

using System;
using System.Reflection;

namespace NocInjector
{
    internal sealed class FieldInjector : MemberInjector
    {
        public override InjectorType InjectorType { get; protected set; } = InjectorType.Field;

        public override void Inject(MemberInfo injectableMember, object instance, ContainerView container = null)
        {
            var field = GetField(injectableMember);

            var injectAttr = field.GetCustomAttribute<Inject>();
            var dependencyType = field.FieldType;
            
            var dependencyInstance = GetDependency(dependencyType, injectAttr, container);
            field.SetValue(instance, dependencyInstance);
        }

        private FieldInfo GetField(MemberInfo injectableMember)
        {
            var field = (FieldInfo)injectableMember;

            return field 
                   ?? throw new PassedIncorrectMemberException(injectableMember, InjectorType);
        }
    }
}