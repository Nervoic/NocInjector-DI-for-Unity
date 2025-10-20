using System;
using System.Reflection;

namespace NocInjector
{
    internal static class MemberExtension
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
        /// Allows you to get the member-a. Return type, which is a field, property or method
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Type GetMemberType(this MemberInfo member)
        {
            var typeToInjection = member switch
            {
                FieldInfo field => typeof(FieldInfo),
                PropertyInfo property => typeof(PropertyInfo),
                MethodInfo method => typeof(MethodInfo),
                _ => throw new InvalidOperationException("GetMemberType can only be used on fields, properties or methods. Please check the MemberInfo type before calling this method.")
            };
            
            return typeToInjection;
        }
        public static InjectorType GetInjectorType(this MemberInfo member)
        {
            var injectorType = member switch
            {
                FieldInfo field => InjectorType.Field,
                PropertyInfo property => InjectorType.Property,
                MethodInfo method => InjectorType.Method,
                _ => throw new InvalidOperationException($"GetInjectorType does not support the {member.Name} type")
            };
            
            return injectorType;
        }
        
        /// <summary>
        /// Allows you to get element. The return type, which is the type of a field or property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Type GetFieldType(this MemberInfo member)
        {
            var typeToInjection = member switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new InvalidOperationException("GetDependencyType can only be used on fields or properties. Please check the MemberInfo type before calling this method.")
            };
            
            return typeToInjection;
        }
        
        /// <summary>
        /// Parse MemberInfo to MethodInfo
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static MethodInfo ToMethod(this MemberInfo member)
        {
            if (member is MethodInfo method)
                return method;

            throw new Exception($"Member {member.GetType().Name} is not a method");
        }
        
        /// <summary>
        /// Parse MemberInfo to FieldInfo
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static FieldInfo ToField(this MemberInfo member)
        {
            if (member is FieldInfo field)
                return field;

            throw new Exception($"Member {member.GetType().Name} is not a field");
        }
        
        /// <summary>
        /// Parse MemberInfo to PropertyInfo
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static PropertyInfo ToProperty(this MemberInfo member)
        {
            if (member is PropertyInfo property)
                return property;

            throw new Exception($"Member {member.GetType().Name} is not a property");
        }
    }
}
