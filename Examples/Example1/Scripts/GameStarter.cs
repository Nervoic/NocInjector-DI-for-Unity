
using NocInjector;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [InjectByRealisation("MainLogger")] private IGameLogger _logger;

    private void Start()
    {
        _logger.Log("Hello World");
    }
}
