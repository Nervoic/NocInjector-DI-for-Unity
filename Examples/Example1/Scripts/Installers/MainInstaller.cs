using NocInjector.Calls;

namespace NocInjector.Examples
{
    internal class MainInstaller : Installer
    {
        protected override void Install()
        {
            Register<CallView>();
        }
    }
}
