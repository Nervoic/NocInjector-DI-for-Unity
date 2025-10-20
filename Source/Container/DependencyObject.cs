
using System;
using NocInjector.Calls;
using UnityEngine;

namespace NocInjector
{
    internal class DependencyObject : MonoBehaviour
    {
        private readonly CallView _objectView = new();
        

        public void FollowDestroy<T>(Action<T> containerFollower) => _objectView.Follow(containerFollower);
        private void OnDestroy() => _objectView.Call(new DependencyObjectDestroyedCall(this));
    }
}