
using UnityEngine;

namespace NocInjector
{
    internal class CarUser : MonoBehaviour
    {
        [Inject] private CarManager _carManager;

        private void Start()
        {
            _carManager.CreateCar();
        }
    }
}
