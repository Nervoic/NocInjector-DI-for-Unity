using System;
using UnityEngine;

namespace NocInjector
{
    public class ContainerView
    {
        private readonly GameContainer _gameContainer;

        internal ContainerView(GameObject containerObject)
        {
            _gameContainer = new GameContainer(containerObject);
        }

        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <param name="typeToRegister">The type to register.</param>
        /// <param name="lifetime">Lifetime of the dependency. Default is Singleton</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ContainerRegister Register(Type typeToRegister, Lifetime lifetime = Lifetime.Singleton)
        {
            if (typeToRegister.IsAbstract)
                throw new Exception($"Cannot register abstract class {typeToRegister.Name}");
            
            _gameContainer.Register(typeToRegister, lifetime);
            
            return new ContainerRegister(_gameContainer, typeToRegister);

        }
        
        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <param name="lifetime">Lifetime of the dependency. Default is Singleton</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public ContainerRegister Register<T>(Lifetime lifetime = Lifetime.Singleton)
        {
            return Register(typeof(T), lifetime);
        }
        
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="objectToResolve">The type of dependency to resolve</param>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <returns></returns>
        public object Resolve(Type objectToResolve, string tag = null)
        {
            _gameContainer.TryResolve(objectToResolve, tag, out var instance);

            if (instance is null)
                throw new Exception($"Dependency {objectToResolve.Name} with id {tag} is not found.");

            return instance;
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="id">The ID of the dependency to resolve</param>
        /// <returns></returns>
        public T Resolve<T>(string id = null)
        {
            return (T)Resolve(typeof(T), id);
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="objectToResolve">The type of dependency to resolve</param>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve(Type objectToResolve, string tag, out object instance)
        {
            _gameContainer.TryResolve(objectToResolve, tag, out instance);
            
            return instance is not null;
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve<T>(string tag, out T instance)
        {
            _gameContainer.TryResolve<T>(tag, out instance);
            
            return instance is not null;
        }
        
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
            /// Adds an ID to the registered dependency.
            /// </summary>
            /// <param name="tag">ID for the dependency.</param>

            public void WithId(string tag)
            {
                _container.ChangeTag(_currentType, tag);
            }
            
            /// <summary>
            /// Uses the type as an interface implementation
            /// </summary>
            /// <param name="interfaceType">The type of the interface to be implemented</param>
            /// <returns></returns>
            public ContainerRegister AsImplementation(Type interfaceType)
            {
                if (!interfaceType.IsInterface)
                    throw new InvalidOperationException($"You are trying to register {_currentType.Name} as an implementation of {interfaceType.Name}, but {interfaceType.Name} is not an interface.");

                if (!interfaceType.IsAssignableFrom(_currentType))
                    throw new InvalidOperationException($"{_currentType.Name} service does not implement the {interfaceType.Name} interface, and cannot be registered.");
                
                _container.ChangeImplementation(_currentType, interfaceType);
                return this;
            }
            
            /// <summary>
            /// Uses the type as an interface implementation
            /// </summary>
            /// <returns></returns>
            
            public ContainerRegister AsImplementation<T>()
            {
                return AsImplementation(typeof(T));
            }
        }
        

    }
}
