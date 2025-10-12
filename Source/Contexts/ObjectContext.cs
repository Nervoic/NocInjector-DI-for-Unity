
using System;
using System.Linq;
using UnityEngine;

namespace NocInjector
{
    public class ObjectContext : Context
    {
        public override ContainerView Container { get; protected set; }

        protected override void Install()
        {
            try
            {
                Container = new ContainerView();

                foreach (var installer in installers.Where(i => i is not null))
                {
                    installer.Initialize(Container);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
