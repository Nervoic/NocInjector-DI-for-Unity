
using UnityEngine;

namespace NocInjector
{
    internal class CarManager : MonoBehaviour
    {
        [Inject("MainFabric")] private ICarFabric _fabric;

        [SerializeField] private string carName;

        public void CreateCar()
        {
            _fabric.CreateCar(carName, gameObject.GetContainer());
        }
    }
}
