using System;
using UnityEngine;

namespace NocInjector
{
    public class FluentRegistration<TDependencyType>
        {
            private readonly GameContainer _container;
            private readonly DependencyInfo _currentDependency;

            private Type DependencyType => typeof(TDependencyType);

            internal FluentRegistration(GameContainer container, DependencyInfo currentDependency)
            {
                _container = container;
                _currentDependency = currentDependency;
            }
            
            /// <summary>
            /// Registers a component from a specific GameObject
            /// </summary>
            /// <param name="gameObject"></param>
            /// <returns></returns>
            public FluentRegistration<TDependencyType> AsComponentOn(GameObject gameObject)
            {
                if (!DependencyType.IsSubclassOf(typeof(Component)))
                    throw new InvalidOperationException( $"Cannot register the service {DependencyType.Name} as a component on {gameObject.name}");
                
                if (gameObject is null)
                    throw new Exception($"Cannot register {DependencyType.Name} component on null-GameObject");
                
                _container.RegisterComponent(_currentDependency, gameObject);
                return this;
            }

            /// <summary>
            /// Adds an Tag to the registered dependency.
            /// </summary>
            /// <param name="tag">Tag for the dependency.</param>

            public FluentRegistration<TDependencyType> WithTag(string tag)
            {
                _container.SetTag(_currentDependency, tag);
                return this;
            }
            
            /// <summary>
            /// Set instance to this dependency
            /// </summary>
            /// <param name="instance">Dependency instance</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public FluentRegistration<TDependencyType> WithInstance(TDependencyType instance)
            {
                if (instance is null)
                    throw new Exception($"Cannot set an instance for {_currentDependency.DependencyType.Name} as null");

                if (DependencyType.IsSubclassOf(typeof(Component)))
                    throw new Exception($"Cannot set an instance for the component {DependencyType.Name} when registering. Use {nameof(AsComponentOn)}");

                var instanceType = typeof(TDependencyType);
                _container.SetInstance(_currentDependency, instance);
                
                return this;
            }
            
            /// <summary>
            /// Cancels registration if the condition is not true
            /// </summary>
            /// <param name="condition"></param>
            public bool If(bool condition)
            {
                if (!condition) 
                    _container.CancelRegister(_currentDependency);

                return condition;
            }
            
            /// <summary>
            /// Uses the type as an interface implementation
            /// </summary>
            /// <typeparam name="TInterfaceType">The type of the interface to be implemented</typeparam>
            /// <returns></returns>
            public FluentRegistration<TDependencyType> AsImplementation<TInterfaceType>()
            {
                var interfaceType = typeof(TInterfaceType);
                
                if (!interfaceType.IsInterface)
                    throw new InvalidOperationException($"You are trying to register {DependencyType.Name} as an implementation of {interfaceType.Name}, but {interfaceType.Name} is not an interface.");

                if (!interfaceType.IsAssignableFrom(DependencyType))
                    throw new InvalidOperationException($"{DependencyType.Name} service does not implement the {interfaceType.Name} interface, and cannot be registered.");
                
                _container.SetImplementation<TInterfaceType>(_currentDependency);
                return this;
            }
        }
}