using System;

namespace NocInjector.Calls
{
    internal sealed class Follower<T> : DisposableFollower<T>
    {
        public Action<T> Method { get; }

        public Follower(Action<T> method)
        {
            Method = method;
        }
    }
}