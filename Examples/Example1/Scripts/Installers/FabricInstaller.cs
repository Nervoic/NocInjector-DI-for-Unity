

using UnityEngine;

namespace NocInjector
{
    internal class FabricInstaller : Installer
    {
        [SerializeField] private GameObject carManagerObject;
        public override void Install(ContainerView container)
        {
            container.Register<CarFabric>().AsImplementation<ICarFabric>().WithTag("MainFabric");
            container.Register<CarManager>().AsComponentOn(carManagerObject);
        }
    }
}
