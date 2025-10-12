using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    internal class MemberInjector : Injector
    {
        public object InjectToMember(MemberInfo injectableMember, object obj, ContainerView containerView = null)
        {
            if (injectableMember is MethodInfo method)
            {
                InjectToMethod(method, obj, containerView);
                return null;
            }
            
            var dependencyType = injectableMember.GetMemberType();

            var injectAttr = injectableMember.GetCustomAttribute<Inject>();
            
            var instance = GetInstance(dependencyType, injectAttr, containerView);
                        
            injectableMember.SetValue(obj, instance);

            return instance;
        }
        
        private void InjectToMethod(MethodInfo method, object obj, ContainerView containerView)
        {
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            var injectAttr = method.GetCustomAttribute<Inject>();

            for (int i = 0; i < parameters.Length; i++)
            {
                var dependencyType = parameters[i].ParameterType;
                
                var instance = GetInstance(dependencyType, injectAttr, containerView);
                
                values[i] = instance;
            }

            method.Invoke(obj, values);

        }
    }
}