using System;

namespace NocInjector.Calls
{
    internal class NonParamFollower<T> : DisposableFollower<T>
    {
        public Action Method { get; }

        public NonParamFollower(Action method)
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