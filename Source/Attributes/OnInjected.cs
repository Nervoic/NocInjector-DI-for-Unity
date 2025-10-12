using System;

namespace NocInjector
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnInjected : Attribute
    {
        public object[] Parameters { get; }
        
        /// <summary>
        /// Parameters that will be passed to this method when it is called
        /// </summary>
        /// <param name="parameters"></param>
        public OnInjected(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}