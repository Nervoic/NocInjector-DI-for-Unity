using NocInjector.Calls;
using UnityEngine;

namespace NocInjector.Examples
{
    internal class DropButton : MonoBehaviour
    {
        [SerializeField] private GameObject trashPrefab;
        
        [Inject(ContextType.Scene)] private CallView _view;

        public void Trash()
        {
            _view.Call(new DropCall(trashPrefab));
        }
    }
}
