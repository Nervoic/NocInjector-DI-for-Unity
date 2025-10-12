using System;
using UnityEngine;

namespace NocInjector
{
    public class ContainerRegister
        {
            private readonly GameContainer _container;
            private readonly Type _currentType;

            internal ContainerRegister(GameContainer container, Type currentType)
            {
                _container = container;
                _currentType = currentType;
            }
            
            /// <summary>
            /// Registers a component from a specific GameObject
            /// </summary>
            /// <param name="gameObject"></param>
            /// <returns></returns>
            public ContainerRegister AsComponentOn(GameObject gameObject)
            {
                if (!_currentType.IsSubclassOf(typeof(Component)))
                    throw new InvalidOperationException( $"Cannot register the service {_currentType.Name} as a component on {gameObject.name}");

                _container.RegisterComponent(_currentType, gameObject);
                return this;
            }

            /// <summary>
            /// Adds an Tag to the registered dependency.
            /// </summary>
            /// <param name="tag">Tag for the dependency.</param>

            public void WithTag(string tag)
            {
                _container.ChangeTag(_currentType, tag);
            }
            
            /// <summary>
            /// Uses the type as an interface implementation
            /// </summary>
            /// <typeparam name="T">The type of the interface to be implemented</typeparam>
            /// <returns></returns>
            public ContainerRegister AsImplementation<T>()
            {
                var interfaceType = typeof(T);
                
                if (!interfaceType.IsInterface)
                    throw new InvalidOperationException($"You are trying to register {_currentType.Name} as an implementation of {interfaceType.Name}, but {interfaceType.Name} is not an interface.");

                if (!interfaceType.IsAssignableFrom(_currentType))
                    throw new InvalidOperationException($"{_currentType.Name} service does not implement the {interfaceType.Name} interface, and cannot be registered.");
                
                _container.ChangeImplementation(_currentType, interfaceType);
                return this;
            }
        }
}