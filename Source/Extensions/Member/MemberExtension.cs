using System;
using System.Reflection;

namespace NocInjector
{
    public static class MemberExtension
    {
        /// <summary>
        /// Allows you to set a value in a member if it is a field or property
        /// </summary>
        /// <param name="member"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public static void SetValue(this MemberInfo member, object obj, object value)
        {
            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(obj, value);
                    break;
                case PropertyInfo property:
                    property.SetValue(obj, value);
                    break;
                default: throw new Exception("SetValue can only be used on fields or properties. Please check the MemberInfo type before calling this method.");
            }
        }
        
        /// <summary>
        /// Allows you to get the member-a. Return type, which is a field or property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Type GetMemberType(this MemberInfo member)
        {
            var typeToInjection = member switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new InvalidOperationException("GetMemberType can only be used on fields or properties. Please check the MemberInfo type before calling this method.")
            };

            return typeToInjection;
        }
    }
}
