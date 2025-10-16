
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
                follower = new NonParamFollower<T>(method);
                _disposableFollowers.Add(follower);
            }

            var currentCall = GetCall<T>();
            
            if (follower.Calls.Contains(currentCall))
                throw new Exception($"You are trying set method {method.Method.Name} on {method.Target.GetType().Name} as follower to call {typeof(T)}, but this method already follow this call");
            
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
                follower = new ParamFollower<T>(method);
                _disposableFollowers.Add(follower);
            }
            
            var currentCall = GetCall<T>();
            
            if (follower.Calls.Contains(currentCall))
                throw new Exception($"You are trying set method {method.Method.Name} on {method.Target.GetType().Name} as follower to call {typeof(T)}, but this method already follow this call");
            
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
                foreach (var disposableFollower in _disposableFollowers.Where(f => ToFollower<T>(f).Calls.Contains(currentCall)))
                {
                    var follower = ToFollower<T>(disposableFollower);
                    follower.RemoveCall(currentCall);
                }
                
                _callsContainer.Remove(currentCall);
                return;
            }
            
            currentCall.InvokeMethods(value);
        }

        private bool GetFollower<T>(Action method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<NonParamFollower<T>>().FirstOrDefault(f => f.Method == method);
            return follower is not null;
        }

        private bool GetFollower<T>(Action<T> method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<ParamFollower<T>>().FirstOrDefault(f => f.Method == method);
            return follower is not null;
        }

        private DisposableFollower<T> ToFollower<T>(object follower) => follower as DisposableFollower<T>;

        private bool HasCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T)) is not null;

        private CallInfo<T> GetCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T));
        private CallInfo<T> ToCall<T>(object call) => call as CallInfo<T>;


    }
}