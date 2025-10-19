using NocInjector.Calls;
using UnityEngine;

namespace NocInjector.Examples
{
    internal class MainGameInstaller : GameInstaller
    {
        [SerializeField] private Trash trash;
        public override void Install(ContainerView container)
        {
            container.Register<CallView>();
        }
    }
}
