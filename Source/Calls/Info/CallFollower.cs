using System;
using System.Collections.Generic;

namespace NocInjector.Calls
{
    internal class CallFollower<T>
    {
        public object FollowerObject { get; }
        public Type FollowerType { get; }

        private readonly List<Action> _nonParamMethods = new();
        private readonly List<Action<T>> _paramMethods = new ();

        public CallFollower(object followerObject)
        {
            FollowerObject = followerObject;
            FollowerType = FollowerObject.GetType();
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

    }
}