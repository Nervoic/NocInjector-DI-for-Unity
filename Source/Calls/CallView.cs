
using System;

namespace NocInjector.Calls
{
    public sealed class CallView
    {
        private readonly CallContainer _container = new();

        private readonly object _callLock = new();
        
        /// <summary>
        /// Registers the call in the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDisposable RegisterCall<T>()
        {
            lock (_callLock)
            {
                return _container.RegisterCall<T>();
            }
        }
        
        /// <summary>
        /// Assigns a call to a method that will be called when the call is made.
        /// </summary>
        /// <param name="method">The method that subscribes to the call</param>
        /// <param name="autoRegisterCall">Indicates whether to automatically register a call if it is not already registered</param>
        /// <typeparam name="T">The type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public IDisposable Follow<T>(Action method, bool autoRegisterCall = true)
        {
            lock (_callLock)
            {
                return method is null 
                    ? throw new ArgumentNullException(nameof(method)) 
                    : _container.Follow<T>(method, autoRegisterCall);
            }
        }
        
        /// <summary>
        /// Assigns a call to a method that will be called when the call is made.
        /// </summary>
        /// <param name="method">The method that subscribes to the call</param>
        /// <param name="autoRegisterCall">Indicates whether to automatically register a call if it is not already registered</param>
        /// <typeparam name="T">The type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public IDisposable Follow<T>(Action<T> method, bool autoRegisterCall = true)
        {
            lock (_callLock)
            {
                return method is null 
                    ? throw new ArgumentNullException(nameof(method)) 
                    : _container.Follow(method, autoRegisterCall);
            }
        }
        
        /// <summary>
        /// Ends call tracking
        /// </summary>
        /// <param name="method">Method that ends call tracking</param>
        /// <typeparam name="T">Type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Unfollow<T>(Action method)
        {
            lock (_callLock)
            {
                if (method is null)
                    throw new ArgumentNullException(nameof(method));

                _container.Unfollow<T>(method);
            }
        }
        
        
        /// <summary>
        /// Ends call tracking
        /// </summary>
        /// <param name="method">Method that ends call tracking</param>
        /// <typeparam name="T">Type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Unfollow<T>(Action<T> method)
        {
            lock (_callLock)
            {
                if (method is null)
                    throw new ArgumentNullException(nameof(method));

                _container.Unfollow(method);
            }
        }
        
        /// <summary>
        /// Calls all followers of this call
        /// </summary>
        /// <param name="value">The value that will be passed to the methods that accept the parameter</param>
        /// <typeparam name="T">The type of the call</typeparam>
        public void Call<T>(T value = default)
        {
            lock (_callLock)
            {
                _container.Call(value);
            }
        }
    }
}
