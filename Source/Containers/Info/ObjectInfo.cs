using System;

namespace NocInjector
{
    public class ObjectInfo
    {
        public Type ObjectType { get; }
        public Type ImplementsInterface { get; }
        public string ObjectTag { get; }
        public object Instance { get; }

        public ObjectInfo(Type objectType, Type implementsInterface = null, string objectTag = null, object instance = null)
        {
            ObjectType = objectType;
            ImplementsInterface = implementsInterface;
            ObjectTag = objectTag;
            Instance = instance;
        }
    }
}