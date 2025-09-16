

namespace NocInjector
{
    internal class CarFabric : ICarFabric
    {
        public void CreateCar(string name, ContainerView containerView)
        {
            containerView.Resolve<Car>().Create(name);
        }
    }
}
