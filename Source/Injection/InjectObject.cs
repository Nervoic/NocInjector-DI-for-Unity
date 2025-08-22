using System.Linq;
using UnityEngine;

namespace NocInjector
{
    /// <summary>
    /// MonoBehaviour for injecting dependencies into attached components.
    /// </summary>
    public class InjectObject : MonoBehaviour
    {
        /// <summary>
        /// Object container holding injected components.
        /// </summary>
        public ObjectContainer Container { get; private set; }
        private void Awake()
        {
            InjectToComponents();
        }

        private void InjectToComponents()
        {
            var components = GetComponents<Component>().Where(c => c is not null).ToArray();
            Container = new ObjectContainer(gameObject);
            
            new ObjectInjector().Inject(components, Container);
        }
    }
}
