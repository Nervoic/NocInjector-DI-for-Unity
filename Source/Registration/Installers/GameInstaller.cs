
using UnityEngine;

namespace NocInjector
{

    public abstract class GameInstaller : MonoBehaviour, IInstaller
    {
        public abstract void Install(ContainerView container);
        
    }
}
