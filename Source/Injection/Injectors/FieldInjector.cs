

using System;
using System.Reflection;

namespace NocInjector
{
    internal sealed class FieldInjector : MemberInjector
    {
        public override InjectorType InjectionType { get; } = InjectorType.Field;

        public override void Inject(MemberInfo injectableMember, object instance, Context context)
        {
            var field = (FieldInfo)injectableMember;
            
            if (field is null)
                throw new Exception($"An incorrect {nameof(MemberInfo)} - {injectableMember.Name} was passed, and it cannot be converted to {InjectionType}");

            var injectAttr = field.GetCustomAttribute<Inject>();
            var dependencyType = field.FieldType;

            var dependencyInstance = GetDependency(dependencyType, injectAttr, context?.Container);
            field.SetValue(instance, dependencyInstance);
        }
    }
}