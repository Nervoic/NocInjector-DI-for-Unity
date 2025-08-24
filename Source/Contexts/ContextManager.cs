using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    public class ContextManager : MonoBehaviour
    {
        [SerializeField] private List<Context> contexts;

        public List<Context> Contexts => contexts;

        private void Awake()
        {
            foreach (var context in contexts.Where(c => c is not null))
            {
                context.Install();
            }
        }
    }
}
