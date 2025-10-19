
using System;
using System.Reflection;

namespace NocInjector
{
    internal sealed class PropertyInjector : MemberInjector
    {
        public override InjectorType InjectionType { get; } = InjectorType.Property;

        public override void Inject(MemberInfo injectableMember, object instance, Context context)
        {
            var property = (PropertyInfo)injectableMember;
            
            if (property.GetValue(instance) is not null)
                return;
            
            if (property is null)
                throw new Exception($"An incorrect {nameof(MemberInfo)} - {injectableMember.Name} was passed, and it cannot be converted to {InjectionType}");

            var injectAttr = property.GetCustomAttribute<Inject>();
            var dependencyType = property.PropertyType;
            var dependencyInstance = GetDependency(dependencyType, injectAttr, context?.Container);
            property.SetValue(instance, dependencyInstance);
        }
    }
}