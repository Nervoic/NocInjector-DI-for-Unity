using System;
using NocInjector.Calls;

namespace NocInjector
{
    public class ContainerView
    {
        private readonly GameContainer _gameContainer;

        private readonly object _containerLock = new();

        internal ContainerView(CallView systemView)
        {
            _gameContainer = new GameContainer(systemView);
        }
        
        /// <summary>
        /// Registers a type in the container.
        /// </summary>
        /// <param name="lifetime">Lifetime of the dependency. Default is Singleton</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public FluentRegistration<TDependencyType> Register<TDependencyType>(Lifetime lifetime = Lifetime.Singleton)
        {
            lock (_containerLock)
            {
                var dependencyType = typeof(TDependencyType);
                
                if (dependencyType.IsAbstract)
                    throw new Exception($"Cannot register abstract class {dependencyType.Name}");

                var newDependency = _gameContainer.Register<TDependencyType>(lifetime);

                return new FluentRegistration<TDependencyType>(_gameContainer, newDependency);
            }
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="dependencyType">The type of dependency to resolve</param>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <returns></returns>
        public object Resolve(Type dependencyType, string tag = null)
        {
            lock (_containerLock)
            {
                return dependencyType is null 
                    ? throw new ArgumentNullException(nameof(dependencyType)) 
                    : _gameContainer.Resolve(dependencyType, tag);
            }
        }

        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <returns></returns>
        public T Resolve<T>(string tag = null) => (T)Resolve(typeof(T), tag);
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="dependencyType">The type of dependency to resolve</param>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve(Type dependencyType, string tag, out object instance)
        {
            lock (_containerLock)
            {
                return dependencyType is null 
                    ? throw new ArgumentNullException(nameof(dependencyType)) 
                    : _gameContainer.TryResolve(dependencyType, tag, out instance);
            }
        }
        
        /// <summary>
        /// Returns a dependency of a specific type from the container.
        /// </summary>
        /// <param name="tag">The ID of the dependency to resolve</param>
        /// <param name="instance"></param>
        /// <returns></returns>

        public bool TryResolve<T>(string tag, out T instance)
        {
            lock (_containerLock)
            {
                return _gameContainer.TryResolve(tag, out instance);
            }
        }
    }
}
