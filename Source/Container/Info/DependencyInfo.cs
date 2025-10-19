using System;
using UnityEngine;

namespace NocInjector
{
    internal class DependencyInfo
    {
        public Type DependencyType { get; }
        public Type ImplementsInterface { get; set; }
        public string DependencyTag { get; set; }
        public object Instance { get; set; }
        public DependencyObject GameObject { get; set; }

        public DependencyInfo(Type dependencyType, Type implementsInterface = null, string dependencyTag = null, object instance = null, DependencyObject gameObject = null)
        {
            DependencyType = dependencyType;
            ImplementsInterface = implementsInterface;
            DependencyTag = dependencyTag;
            Instance = instance;
            GameObject = gameObject;
        }
    }
}