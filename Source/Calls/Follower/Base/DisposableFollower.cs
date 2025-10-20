using System;
using System.Collections.Generic;

namespace NocInjector.Calls
{
    internal abstract class DisposableFollower<T> : IDisposable
    {
        public bool Disposed { get; private set; }
        private List<CallInfo<T>> Calls { get; } = new();

        public void AddCall(CallInfo<T> call) => Calls.Add(call);

        public void RemoveCall(CallInfo<T> call) => Calls.Remove(call);

        public bool ContainsCall(CallInfo<T> call) => Calls.Contains(call);

        public void Dispose()
        {
            if (Disposed) return;
            foreach (var call in Calls)
            {
                call.RemoveFollower(this);
            }

            Disposed = true;
        }
    }
}