
using System;

namespace NocInjector.Calls
{
    public class CallView
    {
        private readonly CallContainer _container = new();
        
        /// <summary>
        /// Assigns a call to a method that will be called when the call is made.
        /// </summary>
        /// <param name="method">The method that subscribes to the call</param>
        /// <typeparam name="T">The type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Follow<T>(Action method)
        {
            if (method is null)
                throw new Exception($"Cannot pass a null method when follow");
            
            _container.Follow<T>(method);
        }
        
        /// <summary>
        /// Assigns a call to a method that will be called when the call is made.
        /// </summary>
        /// <param name="method">The method that subscribes to the call</param>
        /// <typeparam name="T">The type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Follow<T>(Action<T> method)
        {
            if (method is null)
                throw new Exception($"Cannot pass a null method when follow");
            
            _container.Follow(method);
        }
        
        /// <summary>
        /// Ends call tracking
        /// </summary>
        /// <param name="method">Method that ends call tracking</param>
        /// <typeparam name="T">Type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Unfollow<T>(Action method)
        {
            if (method is null)
                throw new Exception($"Unable to pass a null method when follow ends");
            
            _container.Unfollow<T>(method);
        }
        
        
        /// <summary>
        /// Ends call tracking
        /// </summary>
        /// <param name="method">Method that ends call tracking</param>
        /// <typeparam name="T">Type of the call</typeparam>
        /// <exception cref="Exception"></exception>
        public void Unfollow<T>(Action<T> method)
        {
            if (method is null)
                throw new Exception($"Unable to pass a null method when follow ends");
            
            _container.Unfollow(method);
        }
        
        /// <summary>
        /// Calls all followers of this call
        /// </summary>
        /// <param name="value">The value that will be passed to the methods that accept the parameter</param>
        /// <typeparam name="T">The type of the call</typeparam>
        public void Call<T>(T value = default)
        {
            _container.Call(value);
        }
    }
}
