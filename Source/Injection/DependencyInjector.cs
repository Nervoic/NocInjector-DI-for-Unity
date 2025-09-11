
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    internal class DependencyInjector
    {
        public void Inject(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>().Where(c => c is not null).ToArray();
                foreach (var component in components)
                {
                    foreach (var injectableMember in component.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.IsDefined(typeof(Inject))))
                    {
                        var context = gameObject.GetComponent<Context>();
                        InjectToField(injectableMember, component, context?.Container);
                    }
                }
        }

        public void InjectToField(MemberInfo injectableMember, object obj, DependencyContainer container = null)
        {
            var dependencyType = injectableMember.GetMemberType();

            var injectAttr = injectableMember.GetCustomAttribute<Inject>();
            var id = injectAttr.Id;
            var contextType = injectAttr.ContextType;

            object instance = null; 
            
            container?.TryResolve(dependencyType, id, out instance);
                        
            if (instance is null && contextType != ContextType.Object)
            {
                var gameContexts = Object.FindObjectsByType<GameContext>(FindObjectsSortMode.None);
                
                if (contextType is not ContextType.All)
                {
                    gameContexts = gameContexts.Where(c => contextType == ContextType.Project
                        ? c.Lifetime == ContextLifetime.Project
                        : c.Lifetime == ContextLifetime.Scene).ToArray();
                }

                foreach (var gameContext in gameContexts)
                { 
                    gameContext.Container.TryResolve(dependencyType, id, out instance);
                    
                    if (instance is not null)
                        break;
                }
            }
                        
            injectableMember.SetValue(obj, instance);
        }
    }
}
