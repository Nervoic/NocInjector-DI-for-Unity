
namespace NocInjector
{
    internal class DependencyCreatedCall
    {
        public object DependencyInstance { get; }
        public DependencyCreatedCall(object dependencyInstance)
        {
            DependencyInstance = dependencyInstance;
        }
    }
}