
using System.Reflection;

namespace NocInjector
{
    internal sealed class PropertyInjector : MemberInjector
    {
        public override InjectorType InjectorType { get; protected set; } = InjectorType.Property;

        public override void Inject(MemberInfo injectableMember, object instance, ContainerView container = null)
        {
            var property = GetProperty(injectableMember);

            var injectAttr = property.GetCustomAttribute<Inject>();
            var dependencyType = property.PropertyType;
            
            var dependencyInstance = GetDependency(dependencyType, injectAttr, container);
            property.SetValue(instance, dependencyInstance);
        }

        private PropertyInfo GetProperty(MemberInfo injectableMember)
        {
            var property = (PropertyInfo)injectableMember;

            return property 
                   ?? throw new PassedIncorrectMemberException(injectableMember, InjectorType);
        }
    }
}