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
    }
}