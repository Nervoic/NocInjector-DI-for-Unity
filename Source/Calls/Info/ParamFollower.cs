using System;

namespace NocInjector.Calls
{
    internal class ParamFollower<T> : DisposableFollower<T>
    {
        public Action<T> Method { get; }

        public ParamFollower(Action<T> method)
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