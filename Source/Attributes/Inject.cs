using System;

namespace NocInjector
{
    /// <summary>
    /// Attribute for marking fields properties for dependency injection.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property | AttributeTargets.Field)]
    public class Inject : Attribute
    {
        
    }
}
