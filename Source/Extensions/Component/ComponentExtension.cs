using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NocInjector
{
    public static class ComponentExtension
    {
        /// <summary>
        /// Accesses the object container and retrieves an instance of the component from it
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ResolveComponent<T>(this GameObject gameObject) where T : Component
        {
            if (typeof(T).IsInterface)
            {
                Debug.LogError($"To get a component by interface {typeof(T).Name}, use the ResolveComponentByInterface method");
                return null;
            }

            var injectObject = gameObject.GetComponent<InjectObject>();
            
            if (injectObject is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            return injectObject.Container.Resolve<T>();
        }
        
        /// <summary>
        /// Accesses the object container and retrieves an realisation by T interface from it
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="realisationTag"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ResolveComponentByInterface<T>(this GameObject gameObject, string realisationTag) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError($"{typeof(T).Name} is not an interface. To get a component from an object container, use the ResolveComponent method");
                return null;
            }
            var injectObject = gameObject.GetComponent<InjectObject>();
            
            if (injectObject is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            return injectObject.Container.ResolveByInterface(typeof(T), realisationTag) as T;
        }

        /// <summary>
        /// Searches for the first object on the stage that implements the T interface
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="realisationTag"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ResolveAnyComponentByInterface<T>(this GameObject gameObject, string realisationTag) where T : class
        {
            var interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                Debug.LogError($"{typeof(T).Name} is not an interface");
                return null;
            }
            var realisations = Object.FindObjectsByType<Component>(FindObjectsSortMode.None).Where(o => interfaceType.IsAssignableFrom(o.GetType()));

            return realisations.FirstOrDefault(r =>
                r.GetType().IsDefined(typeof(RegisterByInterface), true) &&
                r.GetType().GetCustomAttribute<RegisterByInterface>().InterfaceType == interfaceType &&
                r.GetType().GetCustomAttribute<RegisterByInterface>().RealisationTag == realisationTag) as T;
        }
        
        /// <summary>
        /// Accesses the object's container and adds a component to the object and its container
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T RegisterComponentToContainer<T>(this GameObject gameObject) where T : Component
        {
            if (typeof(T).IsInterface)
            {
                Debug.LogError($"Cannot register {typeof(T)} in the container because it is an interface");
                return null;
            }
            
            Component component = gameObject.AddComponent<T>();

            var injectObject = gameObject.GetComponent<InjectObject>();
            
            if (injectObject is null) throw new Exception($"Failed to add component of type '{typeof(T).Name}' to GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            
            injectObject.Container.Register(typeof(Component), component);
            return injectObject.Container.Resolve<T>();
        }
    }
}
