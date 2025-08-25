
using UnityEngine;

namespace NocInjector
{
        [RegisterComponentAsImplementation(typeof(IGameLogger), "StepLogger")]
        public class GameLoggerStep : MonoBehaviour, IGameLogger
        {
            [Inject] private Car _car;
            [Inject] private Bus _bus;

            private void Start()
            {
                _car.Drive();
            }

            public void Log(string message)
            {
                Debug.Log($"New message in StepLogger - {message}");
            }
        }
    }

