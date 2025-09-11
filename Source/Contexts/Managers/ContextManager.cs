
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {
        private DependencyInjector _injector;
        private void Awake()
        {
            InstallContexts();
        }

        private void InstallContexts()
        {
            var contexts = FindObjectsByType<Context>(FindObjectsSortMode.None).OrderBy(c => c.GetType() == typeof(GameContext) ? 0 : 1);

            foreach (var context in contexts)
            {
                context.InstallContext();
            }
            InjectToComponents();
        }

        private void InjectToComponents()
        {
            _injector ??= new DependencyInjector();
            var gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in gameObjects)
            {
                _injector.Inject(obj);
            }
        }
    }
}
