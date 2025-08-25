

namespace NocInjector
{
    public class BusInstaller : Installer
    {
        public override void Install(ServiceContainer container)
        {
            container.Register<Bus>(ServiceLifetime.Transient);
        }
    }
}
