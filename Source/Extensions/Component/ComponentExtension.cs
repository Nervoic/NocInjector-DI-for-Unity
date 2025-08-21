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
        public static T ContainerGetComponent<T>(this GameObject gameObject) where T : Component
        {
            if (typeof(T).IsInterface) return null;
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
        public static T ContainerGetComponentByInterface<T>(this GameObject gameObject, string realisationTag) where T : Component
        {
            var injectObject = gameObject.GetComponent<InjectObject>();
            
            if (injectObject is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            return (T)injectObject.Container.ResolveByInterface(typeof(T), realisationTag);
        }
        
        /// <summary>
        /// Accesses the object's container and adds a component to the object and its container
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T AddComponentToContainer<T>(this GameObject gameObject) where T : Component
        {
            if (typeof(T).IsInterface) return null;
            Component component = gameObject.AddComponent<T>();

            var injectObject = gameObject.GetComponent<InjectObject>();
            
            if (injectObject is null) throw new Exception($"Failed to add component of type '{typeof(T).Name}' to GameObject '{gameObject.name}': InjectObject component is missing. Please add InjectObject before using dependency injection.");
            
            injectObject.Container.Register(typeof(Component), component);
            return injectObject.Container.Resolve<T>();
        }
    }
}
