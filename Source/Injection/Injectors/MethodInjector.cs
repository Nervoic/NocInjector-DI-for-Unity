using System;
using System.Reflection;

namespace NocInjector
{
    internal sealed class MethodInjector : MemberInjector
    {
        public override InjectorType InjectionType { get; } = InjectorType.Method;

        public override void Inject(MemberInfo injectableMember, object instance, Context context)
        {
            var method = (MethodInfo)injectableMember;
            
            if (method is null)
                throw new Exception($"An incorrect {nameof(MemberInfo)} - {injectableMember.Name} was passed, and it cannot be converted to {InjectionType}");

            var injectAttr = method.GetCustomAttribute<Inject>();
            
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;

                if (dependencyType.IsArray)
                    throw new Exception($"Cannot inject an array into the {method.Name} method in {instance.GetType().Name}");
                
                var dependencyInstance = GetDependency(dependencyType, injectAttr, context?.Container);
                
                values[i] = dependencyInstance;
            }

            method.Invoke(instance, values);
        }
    }
}