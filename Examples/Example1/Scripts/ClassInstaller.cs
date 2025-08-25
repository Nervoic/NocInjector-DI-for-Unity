

namespace NocInjector
{
    public class ClassInstaller : Installer
    {
        public override void Install(ServiceContainer container)
        {
            container.Register<CarManager>(ServiceLifetime.Singleton).AsImplementation<ICarManager>("CarManager");
            container.Register<Car>(ServiceLifetime.Transient);
        }
    }
}
