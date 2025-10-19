namespace NocInjector
{
    public abstract class Installer : IInstaller
    {
        public abstract void Install(ContainerView container);
    }
}