using System;
using System.Linq;
using System.Reflection;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal sealed class Injector
    {
        private readonly MemberInjectorFactory _injectorFactory = new();
        internal Injector(CallView systemView)
        {
            systemView.Follow<DependencyCreatedCall>(InjectToCreated);
        }

        public void InjectTo(GameObject injectableObject)
        {
            var components = injectableObject.GetComponents<Component>().Where(c => c is not null).ToArray();
            
            foreach (var component in components)
                InjectTo(component, component.GetComponent<GameContext>());
        }
        
        public void InjectTo(object instance, GameContext context = null)
        {
            var injectableMembers = instance.GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(Inject)));
                    
            foreach (var injectableMember in injectableMembers)
            {
                var currentInjector = _injectorFactory.GetInjector(injectableMember.GetInjectorType());
                currentInjector.Inject(injectableMember, instance, context?.Container);
            }
                    
            InvokeInjectedMethods(instance);
        }
        
        private void InvokeInjectedMethods(object instance)
        {
            var type = instance.GetType();
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

                injectedMethod.Invoke(instance, values);
            }
        }
        
        private void InjectToCreated(DependencyCreatedCall call)
        {
            var injectableMembers = call.DependencyInstance.GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.IsDefined(typeof(Inject)));
            
            var instance = call.DependencyInstance;

            foreach (var injectableMember in injectableMembers)
            {
                var injector = _injectorFactory.GetInjector(injectableMember.GetInjectorType());
                injector.Inject(injectableMember, instance);
            }
            
            InvokeInjectedMethods(instance);
        }
    }
}