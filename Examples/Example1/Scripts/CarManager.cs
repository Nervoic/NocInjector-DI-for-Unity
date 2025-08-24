using NocInjector;
using UnityEngine;

[RegisterAsRealisation(typeof(ICarManager), "CarManager")]
public class CarManager : ICarManager
{
    private Car _car;

    public void CreateCar(ServiceContainer container)
    {
        _car = container.Resolve<Car>();
    }
}
