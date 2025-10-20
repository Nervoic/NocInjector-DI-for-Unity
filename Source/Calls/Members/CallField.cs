using System;

namespace NocInjector.Calls
{
    public class CallField<T>
    {
        private readonly CallInfo<T> _call = new();
        
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _call.InvokeFollowers(value);
            }
        }
        
        
        public CallField(T value = default)
        {
            Value = value;
        }

        public IDisposable Follow(Action method) => _call.AddFollower(method);
        public IDisposable Follow(Action<T> method) => _call.AddFollower(method);

        public void Unfollow(Action method) => _call.RemoveFollower(method);
        public void Unfollow(Action<T> method) => _call.RemoveFollower(method);
    }
}