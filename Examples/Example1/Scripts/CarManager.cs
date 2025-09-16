
using UnityEngine;

namespace NocInjector
{
    internal class CarManager : MonoBehaviour
    {
         private ICarFabric _fabric;

        [SerializeField] private string carName;

        public void CreateCar()
        {
            _fabric.CreateCar(carName, gameObject.GetContext().Container);
        }
        
        [Inject("MainFabric")]
        public void Initialize(ICarFabric fabric)
        {
            _fabric = fabric;
        }
    }
}
