
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NocInjector
{
    internal class DependencyInjector
    {
        
        public void InjectToObject(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>().Where(c => c is not null).ToArray();
            
                foreach (var component in components)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject))))
                    {
                        var context = gameObject.GetComponent<Context>();
                        var memberInjector = new MemberInjector();
                        
                        memberInjector.InjectToMember(injectableMember, component, context?.Container);
                    }
                    
                    InvokeInjectedMethods(component, component.GetType());
                }
        }

        private void InvokeInjectedMethods(object obj, Type type)
        {
            var injectedMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(OnInjected)));

            foreach (var injectedMethod in injectedMethods)
            {
                if (injectedMethod.IsDefined(typeof(Inject)))
                    throw new Exception($"You cannot inject dependencies into the {nameof(OnInjected)} method {injectedMethod.Name} in {type.Name}");

                var passedParameters = injectedMethod.GetCustomAttribute<OnInjected>().Parameters;
                
                var parameters = injectedMethod.GetParameters();
                var values = new object[parameters.Length];
                
                if (passedParameters.Length != parameters.Length)
                    throw new Exception($"The parameters of the {nameof(OnInjected)} attribute must have the same number of parameters as the {injectedMethod.Name} method in {type.Name}");

                for (var i = 0; i < parameters.Length; i++)
                {
                    var currentParameter = parameters[i];
                    var parameterType = currentParameter.ParameterType;

                    if (parameterType != passedParameters[i].GetType())
                        throw new Exception($"The passed parameter number {i} in the attribute {nameof(OnInjected)} does not match the accepted parameter of the method {injectedMethod.Name} in {type.Name}");

                    values[i] = passedParameters[i];
                }

                injectedMethod.Invoke(obj, values);
            }
        }
    }
}
