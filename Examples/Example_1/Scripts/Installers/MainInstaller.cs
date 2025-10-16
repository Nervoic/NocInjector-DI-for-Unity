using NocInjector.Calls;
using UnityEngine;

namespace NocInjector.Examples
{
    internal class MainInstaller : Installer
    {
        [SerializeField] private Trash trash;
        protected override void Install()
        {
            Register<CallView>();
        }
    }
}
