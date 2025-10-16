
using System;
using System.Collections.Generic;
using System.Linq;

namespace NocInjector.Calls
{
    internal sealed class CallContainer
    {
        private readonly List<object> _registeredFollowers = new();
        private readonly Dictionary<Type, List<object>> _callsContainer = new();

        private void RegisterFollower<T>(object followerObject)
        {
            var newFollower = new CallFollower<T>(followerObject);
            _registeredFollowers.Add(newFollower);
        }
        public IDisposable Follow<T>(Action method)
        {
            var followerObject = method.Target;

            var follower = InitFollow<T>(followerObject);
            
            if (follower.GetNonParamMethods().Contains(method))
                throw new Exception($"You are trying set method {method.Method.Name} on {followerObject.GetType().Name} as follower to call {typeof(T)}, but this method already follow this call");
            
            follower.AddMethod(method);
            return follower;
        }

        public IDisposable Follow<T>(Action<T> method)
        {
            var followerObject = method.Target;

            var follower = InitFollow<T>(followerObject);

            if (follower.GetParamMethods().Contains(method))
                throw new Exception($"You are trying set method {method.Method.Name} on {followerObject.GetType().Name} as follower to call {typeof(T)}, but this method already follow this call");
            
            follower.AddMethod(method);
            return follower;
        }

        private CallFollower<T> InitFollow<T>(object followerObject)
        {
            var callType = typeof(T);
            
            if (!HasFollower<T>(followerObject)) 
                RegisterFollower<T>(followerObject);

            if (!HasCall(callType))
                InitCall(callType);

            var follower = GetFollower<T>(followerObject);
            var currentCall = _callsContainer[callType];
            
            AddFollower(follower, currentCall);

            return follower;
        }

        public void Unfollow<T>(Action method)
        {
            var callType = typeof(T);
            
            if (!HasCall(callType))
                throw new Exception($"The call {callType} is not monitored by anyone.");

            var targetCall = _callsContainer[callType];
            var targetFollower = ToFollower<T>(targetCall.FirstOrDefault(o => ToFollower<T>(o).GetNonParamMethods().Contains(method)));
            
            if (targetFollower is null)
                throw new Exception($"Method {method.Method.Name} does not track the call {callType}, and cannot stop tracking it");
            
            targetFollower.RemoveMethod(method);
        }
        
        public void Unfollow<T>(Action<T> method)
        {
            var callType = typeof(T);

            if (!HasCall(callType))
                throw new Exception($"The call {callType} is not monitored by anyone.");

            var targetCall = _callsContainer[callType];
            var targetFollower = ToFollower<T>(targetCall.FirstOrDefault(o => ToFollower<T>(o).GetParamMethods().Contains(method)));
            
            if (targetFollower is null)
                throw new Exception($"Method {method.Method.Name} does not track the call {typeof(T)}, and cannot stop tracking it");
            
            targetFollower.RemoveMethod(method);
        }

        private void AddFollower<T>(CallFollower<T> follower, List<object> currentCall)
        {
            if (currentCall.Contains(follower)) return;
            
            currentCall.Add(follower);
        }

        private void InitCall(Type callType)
        {
            _callsContainer.Add(callType, new List<object>());
        }


        public void Call<T>(T value)
        {
            var callType = typeof(T);

            if (!HasCall(callType))
                throw new Exception($"Call {callType.Name} doesn't have not a single follower");
            
            var nullFollowers = new List<object>();
            var callFollowers = _callsContainer[callType];
            
            foreach (var callFollower in callFollowers)
            {
                var follower = ToFollower<T>(callFollower);

                if (follower.FollowerObject is null) 
                {
                    nullFollowers.Add(callFollower); 
                    continue; 
                }
                    
                follower.InvokeMethods(value);
            }
            
            RemoveNullFollowers(ref nullFollowers, ref callFollowers);
        }

        private void RemoveNullFollowers(ref List<object> nullFollowers, ref List<object> callFollowers)
        {
            if (nullFollowers.Count <= 0) return;
                
            foreach (var nullFollower in nullFollowers)
            { 
                ToFollower<object>(nullFollower).Dispose();
                
                _registeredFollowers.Remove(nullFollower);
                callFollowers.Remove(nullFollower);
            }
            
            nullFollowers.Clear();
        }

        private bool HasFollower<T>(object followerObject) => GetFollower<T>(followerObject) is not null;
        private bool HasCall(Type callType) => _callsContainer.FirstOrDefault(c => c.Key == callType).Key is not null;

        private CallFollower<T> GetFollower<T>(object followerObject) => (CallFollower<T>)_registeredFollowers.FirstOrDefault(f => ((CallFollower<T>)f).FollowerObject == followerObject);
        private CallFollower<T> ToFollower<T>(object obj) => (CallFollower<T>)obj;


    }
}