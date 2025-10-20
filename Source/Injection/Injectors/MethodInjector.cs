using System;
using System.Linq;
using System.Reflection;

namespace NocInjector
{
    internal sealed class MethodInjector : MemberInjector
    {
        public override InjectorType InjectorType { get; protected set; } = InjectorType.Method;

        public override void Inject(MemberInfo injectableMember, object instance, ContainerView container = null)
        {
            var method = GetMethod(injectableMember);

            var injectAttr = method.GetCustomAttribute<Inject>();
            
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;

                if (dependencyType.IsArray)
                    throw new Exception($"Cannot inject an array into the {method.Name} method in {instance.GetType().Name}");
                
                var dependencyInstance = GetDependency(dependencyType, injectAttr, container);
                
                values[i] = dependencyInstance;
            }
            
            method.Invoke(instance, values);
        }

        private MethodInfo GetMethod(MemberInfo injectableMember)
        {
            var method = (MethodInfo)injectableMember;
            
            return method 
                   ?? throw new PassedIncorrectMemberException(injectableMember, InjectorType);
        }
    }
}