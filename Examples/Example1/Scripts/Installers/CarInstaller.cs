
namespace NocInjector
{
    internal class CarInstaller : Installer
    {
        public override void Install(ContainerView container)
        {
            container.Register<Car>(Lifetime.Transient);
        }
    }
}
