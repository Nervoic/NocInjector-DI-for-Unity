namespace NocInjector
{
    internal class DependencyObjectDestroyedCall
    {
        public DependencyObject DependencyObject { get; }

        public DependencyObjectDestroyedCall(DependencyObject dependencyObject)
        {
            DependencyObject = dependencyObject;
        }
    }
}