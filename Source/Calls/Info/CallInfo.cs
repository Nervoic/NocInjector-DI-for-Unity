using System;
using System.Collections.Generic;
using System.Linq;

namespace NocInjector.Calls
{
    internal sealed class CallInfo<T> : IDisposable
    {
        public Type CallType { get; private set; } = typeof(T);

        private readonly List<DisposableFollower<T>> _followers = new();
        
        public bool Disposed { get; private set; }

        public VoidFollower<T> AddFollower(Action method)
        {
            if (TryGetFollower(method, out var follower))
                return follower;

            var newFollower = new VoidFollower<T>(method);
            _followers.Add(newFollower);
            
            newFollower.AddCall(this);
            return newFollower;
        }

        public Follower<T> AddFollower(Action<T> method)
        {
            if (TryGetFollower(method, out var follower))
                return follower;

            var newFollower = new Follower<T>(method);
            _followers.Add(newFollower);

            newFollower.AddCall(this);
            return newFollower;
        }

        public void RemoveFollower(DisposableFollower<T> follower)
        {
            if (!_followers.Contains(follower)) 
                return;

            _followers.Remove(follower);
        }
        
        public void RemoveFollower(Action method)
        {
            if (TryGetFollower(method, out var follower)) 
                _followers.Remove(follower);
        }
        
        public void RemoveFollower(Action<T> method)
        {
            if (TryGetFollower(method, out var follower)) 
                _followers.Remove(follower);
        }

        public void InvokeFollowers(T value)
        {
            foreach (var voidFollower in _followers.OfType<VoidFollower<T>>().ToList())
            {
                var method = voidFollower.Method;

                if (method.Target is null)
                {
                    voidFollower.Dispose();
                    
                    _followers.Remove(voidFollower);
                    continue;
                }
                
                method.Invoke();
            }
            
            foreach (var follower in _followers.OfType<Follower<T>>().ToList())
            {
                var method = follower.Method;

                if (method.Target is null)
                {
                    follower.Dispose();
                    
                    _followers.Remove(follower);
                    continue;
                }
                
                method.Invoke(value);
            }
        }

        private bool TryGetFollower(Action method, out VoidFollower<T> follower)
        {
            follower = _followers.OfType<VoidFollower<T>>().FirstOrDefault(follower => follower.Method == method);
            
            return follower is not null;
        }
        
        private bool TryGetFollower(Action<T> method, out Follower<T> follower)
        {
            follower = _followers.OfType<Follower<T>>().FirstOrDefault(follower => follower.Method == method);
            
            return follower is not null;
        }
        
        /// <summary>
        /// Clears all follows of all methods in this class.
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;
            
            foreach (var follower in _followers)
            {
                follower.RemoveCall(this);
            }
            
            _followers.Clear();
            Disposed = true;
        }
    }
}