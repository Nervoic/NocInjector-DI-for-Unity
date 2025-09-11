
namespace NocInjector
{
    internal class CarInstaller : Installer
    {
        public override void Install(DependencyContainer container)
        {
            container.Register<Car>(Lifetime.Transient);
        }
    }
}
