
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NocInjector.Calls
{
    internal sealed class CallContainer
    {
        private readonly List<object> _disposableFollowers = new();
        private readonly List<object> _callsContainer = new();
        public IDisposable Follow<T>(Action method)
        {
            if (!HasCall<T>())
                RegisterCall<T>();
            
            if (!GetFollower<T>(method, out var follower))
            {
                follower = new VoidFollower<T>(method);
                _disposableFollowers.Add(follower);
            }

            var currentCall = GetCall<T>();
            
            if (!follower.Calls.Contains(currentCall)) 
                follower.AddCall(currentCall);
            
            currentCall.AddFollower(method);
            
            return follower;
        }

        public IDisposable Follow<T>(Action<T> method)
        {
            if (!HasCall<T>())
                RegisterCall<T>();
            
            if (!GetFollower(method, out var follower))
            {
                follower = new Follower<T>(method);
                _disposableFollowers.Add(follower);
            }
            
            var currentCall = GetCall<T>();
            
            if (!follower.Calls.Contains(currentCall)) 
                follower.AddCall(currentCall);
            
            currentCall.AddFollower(method);
            
            return follower;
        }

        public void Unfollow<T>(Action method)
        {
            if (!HasCall<T>())
                throw new Exception($"Call {typeof(T).Name} is not registered or disposed");
            
            var currentCall = GetCall<T>();
            
            if (!GetFollower<T>(method, out var follower)) return;
            
            follower.RemoveCall(currentCall);
            currentCall.RemoveFollower(method);
        }
        
        public void Unfollow<T>(Action<T> method)
        {
            if (!HasCall<T>())
                throw new Exception($"Call {typeof(T).Name} is not registered or disposed");
            
            var currentCall = GetCall<T>();
            
            if (!GetFollower(method, out var follower)) return;
            
            follower.RemoveCall(currentCall);
            currentCall.RemoveFollower(method);
        }

        public IDisposable RegisterCall<T>()
        {
            if (HasCall<T>())
                throw new Exception($"{typeof(T)} call already registered");
            
            var newCall = new CallInfo<T>();
            _callsContainer.Add(newCall);

            return newCall;
        }


        public void Call<T>(T value)
        {
            var callType = typeof(T);

            if (!HasCall<T>())
                throw new Exception($"Call {callType.Name} is not registered or disposed");

            var currentCall = GetCall<T>();

            if (currentCall.Disposed)
            {
                ClearDisposed(currentCall);
                return;
            }

            currentCall.InvokeMethods(value);
        }

        private void ClearDisposed<T>(CallInfo<T> currentCall)
        {
            foreach (var disposableFollower in _disposableFollowers.Where(f => ToFollower<T>(f).Calls.Contains(currentCall)))
            {
                var follower = ToFollower<T>(disposableFollower);
                follower.RemoveCall(currentCall);
            }
                
            _callsContainer.Remove(currentCall);
        }

        private bool GetFollower<T>(Action method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<VoidFollower<T>>().FirstOrDefault(f => f.Method == method);
            return follower is not null;
        }

        private bool GetFollower<T>(Action<T> method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<Follower<T>>().FirstOrDefault(f => f.Method == method);
            return follower is not null;
        }

        private DisposableFollower<T> ToFollower<T>(object follower) => follower as DisposableFollower<T>;

        private bool HasCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T)) is not null;

        private CallInfo<T> GetCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T));
        private CallInfo<T> ToCall<T>(object call) => call as CallInfo<T>;


    }
}