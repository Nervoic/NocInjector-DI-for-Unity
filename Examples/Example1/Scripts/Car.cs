using UnityEngine;

namespace NocInjector
{
    internal class Car
    {
        public void Create(string name)
        {
            Debug.Log($"Created car with name {name}");
        }
    }
}
