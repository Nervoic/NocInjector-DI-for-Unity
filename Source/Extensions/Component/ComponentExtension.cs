using System;
using UnityEngine;

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

            var injectObject = gameObject.GetContext();
            
            if (injectObject is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            return injectObject.ComponentContainer.Resolve<T>();
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

            var injectObject = gameObject.GetContext();
            
            if (injectObject is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            return injectObject.ComponentContainer.ResolveImplementation(realisationTag) as T;
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
            Component component = gameObject.AddComponent<T>();

            var injectObject = gameObject.GetContext();
            
            if (injectObject is null) throw new Exception($"Failed to add component of type '{typeof(T).Name}' to GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            
            injectObject.ComponentContainer.Register(typeof(Component), component);
            return injectObject.ComponentContainer.Resolve<T>();
        }

        public static ObjectContext GetContext(this GameObject gameObject)
        {
            return gameObject.GetComponent<ObjectContext>();
        }
    }
}
