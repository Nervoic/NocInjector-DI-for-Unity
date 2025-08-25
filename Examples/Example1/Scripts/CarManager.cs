using NocInjector;

    public class CarManager : ICarManager
    {
        private Car _car;

        public void CreateCar(ServiceContainer container)
        {
            _car = container.Resolve<Car>();
        }
    }
