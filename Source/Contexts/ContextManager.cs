
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {

        private void Awake()
        {
            InstallToContexts();
        }

        private void InstallToContexts()
        {
            var contexts = FindObjectsByType<Context>(FindObjectsSortMode.None).ToList();
            var sortedContexts = contexts.OrderBy(c => c.GetType() == typeof(GameContext) ? 0 : 1);
            foreach (var context in sortedContexts)
            {
                context.Install();
            }
        }
    }
}
