using System;

namespace NocInjector.Calls
{

    public class CallMissingException : Exception
    {
        public CallMissingException(Type callType) : base($"{callType.Name} call is not registered or disposed")
        {
            
        }
    }
}