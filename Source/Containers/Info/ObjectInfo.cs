using System;

namespace NocInjector
{
    public class ObjectInfo
    {
        public Type ObjectType { get; }
        public Type ImplementsInterface { get; }
        public string ObjectId { get; }

        public ObjectInfo(Type objectType, Type implementsInterface = null, string objectId = null)
        {
            ObjectType = objectType;
            ImplementsInterface = implementsInterface;
            ObjectId = objectId;
        }
    }
}