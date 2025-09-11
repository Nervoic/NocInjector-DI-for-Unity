

using UnityEngine;

namespace NocInjector
{
    internal class FabricInstaller : Installer
    {
        [SerializeField] private GameObject carManagerObject;
        public override void Install(DependencyContainer container)
        {
            container.Register<CarFabric>().AsImplementation<ICarFabric>().WithId("MainFabric");
            container.Register<CarManager>().AsComponentOn(carManagerObject);
        }
    }
}
