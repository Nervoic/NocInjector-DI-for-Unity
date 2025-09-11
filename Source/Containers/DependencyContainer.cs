using System;
using UnityEngine;

namespace NocInjector
{
    public class DependencyContainer
    {
        private readonly ServiceContainer _serviceContainer;
        private readonly ComponentContainer _componentContainer;

        internal DependencyContainer(GameObject containerObject)
        {
            _serviceContainer = new ServiceContainer();
            _componentContainer = new ComponentContainer(containerObject);
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

            var container = SelectContainerToRegister(typeToRegister);
            container.Register(typeToRegister, lifetime);
            
            return new ContainerRegister(container, typeToRegister, this);

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
        /// <param name="id">The ID of the dependency to resolve</param>
        /// <returns></returns>
        public object Resolve(Type objectToResolve, string id = null)
        {
            if (!_serviceContainer.TryResolve(objectToResolve, id, out var instance))
                _componentContainer.TryResolve(objectToResolve, id, out instance);

            if (instance is null)
                throw new Exception($"Dependency {objectToResolve.Name} with id {id} is not found.");

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
        /// <param name="id">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve(Type objectToResolve, string id, out object instance)
        {
            if (!_serviceContainer.TryResolve(objectToResolve, id, out instance))
                _componentContainer.TryResolve(objectToResolve, id, out instance);
            
            return instance is not null;
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="id">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve<T>(string id, out T instance)
        {
            if (!_serviceContainer.TryResolve<T>(id, out instance))
                _componentContainer.TryResolve<T>(id, out instance);
            
            return instance is not null;
        }

        private Container SelectContainerToRegister(Type typeToSelect)
        {
            return typeToSelect.IsSubclassOf(typeof(Component)) ? _componentContainer : _serviceContainer;
        }
        
        public class ContainerRegister
        {
            private readonly Container _container;
            private readonly Type _currentType;
            private readonly DependencyContainer _dependencyContainer;

            internal ContainerRegister(Container container, Type currentType, DependencyContainer dependencyContainer)
            {
                _container = container;
                _currentType = currentType;
                _dependencyContainer = dependencyContainer;
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

                _dependencyContainer._componentContainer.RegisterComponent(_currentType, gameObject);
                return this;
            }

            /// <summary>
            /// Adds an ID to the registered dependency.
            /// </summary>
            /// <param name="id">ID for the dependency.</param>

            public void WithId(string id)
            {
                _container.AddId(_currentType, id);
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
                
                _container.AddImplementation(_currentType, interfaceType);
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
