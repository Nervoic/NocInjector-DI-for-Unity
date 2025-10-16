
using System.Collections.Generic;
using UnityEngine;

namespace NocInjector
{
    internal class DependencyObject : MonoBehaviour
    {
        private readonly List<GameContainer> _gameContainers = new();
        
        public void Initialize(GameContainer container)
        {
            _gameContainers.Add(container);
        }

        private void OnDestroy()
        {
            foreach (var gameContainer in _gameContainers)
            {
                gameContainer.DisposeObject(this);
            }
        }
    }
}