using NocInjector;

public class ClassInstaller : Installer
{
    public override void Install(ServiceContainer container)
    {
        container.Register<Car>(ServiceLifetime.Transient);
        container.Register<CarManager>(ServiceLifetime.Singleton);
    }
}
