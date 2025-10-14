using System;
using NocInjector.Calls;

namespace NocInjector
{
    public class ContainerView
    {
        private readonly GameContainer _gameContainer;

        internal ContainerView(CallView systemView)
        {
            _gameContainer = new GameContainer(systemView);
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
    }
}
