
using UnityEngine;

namespace NocInjector
{
        [RegisterComponentAsImplementation(typeof(IGameLogger), "MainLogger")]
        public class GameLoggerMain : MonoBehaviour, IGameLogger
        {
            [SerializeField] private GameContext context;
            [InjectImplementation("CarManager")] private ICarManager _carManager;
            
            private void Start()
            {
                _carManager.CreateCar(context.Container);
                GameStarter starter = gameObject.GetContext().ComponentContainer.Resolve<GameStarter>();
                starter.Find();
            }

            public void Log(string message)
            {
                Debug.Log($"New message in MainLogger - {message}");
            }
        }
    }

