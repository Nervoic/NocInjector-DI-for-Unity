using System;
using UnityEngine;

namespace NocInjector
{
    public static class ComponentExtension
    {
        
        /// <summary>
        /// Returns a component from the ObjectContext container
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="id">Implementation ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        
        public static T ResolveComponent<T>(this GameObject gameObject, string id = null) where T : Component
        {
            var container = GetContainer(gameObject);
            
            if (container is null) throw new Exception($"Failed to get component of type '{typeof(T).Name}' from GameObject '{gameObject.name}': Context is missing. Please add Context before using dependency injection.");
            return container.Resolve<T>(id);
        }
        
        /// <summary>
        /// Returns a component from the ObjectContext container
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="component"></param>
        /// <param name="id">Implementation ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        
        public static Component ResolveComponent(this GameObject gameObject, Component component, string id = null)
        {
            var container = GetContainer(gameObject);
            
            if (container is null) throw new Exception($"Failed to get component of type '{component.GetType().Name}' from GameObject '{gameObject.name}': Context component is missing. Please add Context before using dependency injection.");
            return (Component)container.Resolve(component.GetType(), id);
        }

        public static DependencyContainer GetContainer(this GameObject gameObject)
        {
            return gameObject.GetComponent<ObjectContext>()?.Container;
        }
    }
}
