using System;
using System.Collections.Generic;

namespace NocInjector.Calls
{
    internal abstract class DisposableFollower<T> : IDisposable
    {
        public List<CallInfo<T>> Calls { get; } = new();

        public void AddCall(CallInfo<T> call)
        {
            Calls.Add(call);
        }

        public void RemoveCall(CallInfo<T> call)
        {
            Calls.Remove(call);
        }

        public abstract void Dispose();
    }
}