
using System;
using System.Collections.Generic;
using System.Linq;

namespace NocInjector.Calls
{
    internal sealed class CallContainer
    {
        private readonly List<object> _disposableFollowers = new();
        private readonly List<object> _callsContainer = new();
        public IDisposable Follow<T>(Action method, bool autoRegisterCall)
        {
            if (!HasCall<T>() && autoRegisterCall)
                RegisterCall<T>();
            else
                throw new CallMissingException(typeof(T));

            var currentCall = GetCall<T>();
            var follower = currentCall.AddFollower(method);
            
            if (!_disposableFollowers.Contains(follower))
                _disposableFollowers.Add(follower);
            
            return follower;
        }

        public IDisposable Follow<T>(Action<T> method, bool autoRegisterCall)
        {
            if (!HasCall<T>() && autoRegisterCall)
                RegisterCall<T>();
            else
                throw new CallMissingException(typeof(T));

            var currentCall = GetCall<T>();
            var follower = currentCall.AddFollower(method);
            
            if (!_disposableFollowers.Contains(follower))
                _disposableFollowers.Add(follower);
            
            return follower;
        }

        public void Unfollow<T>(Action method)
        {
            if (!TryGetCall<T>(out var currentCall))
                throw new CallMissingException(typeof(T));
            
            if (!TryGetFollower<T>(method, out var follower)) 
                return;
            
            follower.RemoveCall(currentCall);
            currentCall.RemoveFollower(follower);
        }
        
        public void Unfollow<T>(Action<T> method)
        {
            if (!TryGetCall<T>(out var currentCall))
                throw new CallMissingException(typeof(T));
            
            if (!TryGetFollower(method, out var follower)) 
                return;
            
            follower.RemoveCall(currentCall);
            currentCall.RemoveFollower(follower);
        }

        public IDisposable RegisterCall<T>()
        {
            if (HasCall<T>())
                return GetCall<T>();
            
            var newCall = new CallInfo<T>();
            _callsContainer.Add(newCall);

            return newCall;
        }


        public void Call<T>(T value)
        {
            var callType = typeof(T);

            if (!TryGetCall<T>(out var currentCall))
                throw new CallMissingException(callType);

            if (currentCall.Disposed)
            {
                ClearDisposed(currentCall);
                return;
            }

            currentCall.InvokeFollowers(value);
        }

        private void ClearDisposed<T>(CallInfo<T> currentCall)
        {
            foreach (var disposableFollower in _disposableFollowers.Where(follower => ToFollower<T>(follower).ContainsCall(currentCall)))
            {
                var follower = ToFollower<T>(disposableFollower);
                follower.RemoveCall(currentCall);
            }
                
            _callsContainer.Remove(currentCall);
        }

        private bool TryGetFollower<T>(Action method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<VoidFollower<T>>().FirstOrDefault(f => f.Method == method);
            
            if (follower is not null && follower.Disposed)
            {
                _disposableFollowers.Remove(follower);
                return TryGetFollower(method, out follower);
            }
            return follower is not null;
        }

        private bool TryGetFollower<T>(Action<T> method, out DisposableFollower<T> follower)
        {
            follower = _disposableFollowers.OfType<Follower<T>>().FirstOrDefault(f => f.Method == method);
            if (follower is not null && follower.Disposed)
            {
                _disposableFollowers.Remove(follower);
                return TryGetFollower(method, out follower);
            }
            return follower is not null;
        }
        private DisposableFollower<T> ToFollower<T>(object follower) => follower as DisposableFollower<T>;

        private bool HasCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T)) is not null;
        
        private CallInfo<T> GetCall<T>() => _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T));
        private bool TryGetCall<T>(out CallInfo<T> call)
        {
            call = _callsContainer.OfType<CallInfo<T>>().FirstOrDefault(call => call.CallType == typeof(T));
            return call is not null;
        }


    }
}