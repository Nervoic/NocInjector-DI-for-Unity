using System;
using System.Reflection;

namespace NocInjector
{
    internal class PassedIncorrectMemberException : Exception
    {
        public PassedIncorrectMemberException(MemberInfo member, InjectorType injectorType) : base($"An incorrect {nameof(MemberInfo)} - {member.Name} was passed, and it cannot be converted to {injectorType}")
        {
            
        }
    }
}