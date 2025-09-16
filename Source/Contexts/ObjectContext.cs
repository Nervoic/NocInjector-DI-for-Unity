
using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    public class ObjectContext : Context
    {
        public override DependencyContainer Container { get; protected set; }

        protected override void Install()
        {
            try
            {
                Container = new DependencyContainer(gameObject);

                foreach (var installer in installers.Where(i => i is not null))
                {
                    installer.Install(Container);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
