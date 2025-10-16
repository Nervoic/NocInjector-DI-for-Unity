using System;

namespace NocInjector
{
    public class DependencyInfo
    {
        public Type DependencyType { get; }
        public Type ImplementsInterface { get; }
        public string DependencyTag { get; }
        public object Instance { get; }

        public DependencyInfo(Type dependencyType, Type implementsInterface = null, string dependencyTag = null, object instance = null)
        {
            DependencyType = dependencyType;
            ImplementsInterface = implementsInterface;
            DependencyTag = dependencyTag;
            Instance = instance;
        }
    }
}