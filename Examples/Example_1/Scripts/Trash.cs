using NocInjector.Calls;
using UnityEngine;

namespace NocInjector.Examples
{
    internal class Trash : MonoBehaviour
    {
        [SerializeField] private Transform spawnPosition;

        [Inject(ContextType.Scene)] private CallView _view;
        
        [OnInjected]
        private void Follow()
        {
            _view.Follow<DropCall>(Drop);
        }

        private void Drop(DropCall call)
        {
            Instantiate(call.TrashPrefab, spawnPosition.position, Quaternion.identity);
        }
    }
}
