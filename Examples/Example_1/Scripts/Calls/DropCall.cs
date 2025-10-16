using UnityEngine;

namespace NocInjector.Examples
{
    internal class DropCall
    {
        public GameObject TrashPrefab { get; }

        public DropCall(GameObject trashPrefab)
        {
            TrashPrefab = trashPrefab;
        }
    }
}
