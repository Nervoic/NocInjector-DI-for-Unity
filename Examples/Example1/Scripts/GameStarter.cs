
using UnityEngine;

namespace NocInjector
{
    public class GameStarter : MonoBehaviour
    {
        [InjectImplementation("MainLogger")] private IGameLogger _logger;

        private void Start()
        {
            _logger.Log("Hello World");
        }

        public void Find()
        {
            gameObject.GetContext().ComponentContainer.Resolve<GameLoggerStep>().Log("Step logger");
        }
    }
}
