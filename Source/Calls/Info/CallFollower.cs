using System;
using System.Collections.Generic;

namespace NocInjector.Calls
{
    internal sealed class CallFollower<T> : IDisposable
    {
        public object FollowerObject { get; private set; }

        private readonly List<Action> _nonParamMethods = new();
        private readonly List<Action<T>> _paramMethods = new ();

        public CallFollower(object followerObject)
        {
            FollowerObject = followerObject;
        }

        public void AddMethod(Action nonParamMethod) => _nonParamMethods.Add(nonParamMethod);

        public void AddMethod(Action<T> paramMethod) => _paramMethods.Add(paramMethod);

        public void RemoveMethod(Action nonParamMethod) => _nonParamMethods.Remove(nonParamMethod);

        public void RemoveMethod(Action<T> paramMethod) => _paramMethods.Remove(paramMethod);

        public void InvokeMethods(T value)
        {
            foreach (var nonParamMethod in _nonParamMethods)
            {
                nonParamMethod.Invoke();
            }

            foreach (var paramMethod in _paramMethods)
            {
                paramMethod.Invoke(value);
            }
        }

        public Action[] GetNonParamMethods() => _nonParamMethods.ToArray();
        public Action<T>[] GetParamMethods() => _paramMethods.ToArray();
        
        
        /// <summary>
        /// Clears all follows of all methods in this class.
        /// </summary>
        public void Dispose()
        {
            _nonParamMethods.Clear();
            _paramMethods.Clear();

            FollowerObject = null;
        }
    }
}