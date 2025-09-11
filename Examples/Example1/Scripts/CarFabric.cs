

namespace NocInjector
{
    internal class CarFabric : ICarFabric
    {
        public void CreateCar(string name, DependencyContainer container)
        {
            container.Resolve<Car>().Create(name);
        }
    }
}
