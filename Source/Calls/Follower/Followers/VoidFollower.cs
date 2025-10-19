using System;

namespace NocInjector.Calls
{
    internal sealed class VoidFollower<T> : DisposableFollower<T>
    {
        public Action Method { get; }

        public VoidFollower(Action method)
        {
            Method = method;
        }

        public override void Dispose()
        {
            foreach (var call in Calls)
            {
                call.RemoveFollower(Method);
            }
        }
    }
}