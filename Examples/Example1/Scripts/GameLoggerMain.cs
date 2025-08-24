
using UnityEngine;

namespace NocInjector
{
    [RegisterAsRealisation(typeof(IGameLogger), "MainLogger")]
    public class GameLoggerMain : MonoBehaviour, IGameLogger
    {
        [SerializeField] private GameContext context;
        [InjectByRealisation("CarManager")] private ICarManager _carManager;

        private void Start()
        {
            _carManager.CreateCar(context.Container);
        }

        public void Log(string message)
        {
            Debug.Log($"New message in MainLogger - {message}");
        }
    }
}
