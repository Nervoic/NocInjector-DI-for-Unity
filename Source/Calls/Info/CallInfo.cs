using System;
using System.Collections.Generic;
using UnityEngine;

namespace NocInjector.Calls
{
    internal sealed class CallInfo<T> : IDisposable
    {
        public Type CallType { get; private set; } = typeof(T);

        private readonly List<Action> _nonParamFollowers = new();
        private readonly List<Action<T>> _paramFollowers = new();
        
        public bool Disposed { get; private set; }

        public void AddFollower(Action nonParamFollower)
        {
            if (_nonParamFollowers.Contains(nonParamFollower)) return;
            
            _nonParamFollowers.Add(nonParamFollower);
        }

        public void AddFollower(Action<T> paramFollower)
        {
            if (_paramFollowers.Contains(paramFollower)) return;
            
            _paramFollowers.Add(paramFollower);
        }

        public void RemoveFollower(Action nonParamFollower)
        {
            if (!_nonParamFollowers.Contains(nonParamFollower)) return;

            _nonParamFollowers.Remove(nonParamFollower);
        }

        public void RemoveFollower(Action<T> paramFollower)
        {
            if (!_paramFollowers.Contains(paramFollower)) return;

            _paramFollowers.Remove(paramFollower);
        }

        public void InvokeMethods(T value)
        {
            foreach (var nonParamFollower in _nonParamFollowers)
            {
                if (nonParamFollower.Target is null)
                {
                    RemoveFollower(nonParamFollower);
                    continue;
                }
                
                nonParamFollower.Invoke();
            }
            
            
            foreach (var paramFollower in _paramFollowers)
            {
                if (paramFollower.Target is null)
                {
                    RemoveFollower(paramFollower);
                    continue;
                }
                
                paramFollower.Invoke(value);
            }
            
        }

        public Action[] GetNonParamMethods() => _nonParamFollowers.ToArray();
        public Action<T>[] GetParamMethods() => _paramFollowers.ToArray();
        
        
        /// <summary>
        /// Clears all follows of all methods in this class.
        /// </summary>
        public void Dispose()
        {
            _nonParamFollowers.Clear();
            _paramFollowers.Clear();

            Disposed = true;
        }
    }
}