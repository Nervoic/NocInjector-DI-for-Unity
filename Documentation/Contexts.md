## Description
The context is used to separate dependency containers.

## The main thing
- To use the context, you need to add it to the scene via the Tools menu
- For the library to work, you also need to add a ContextManager via the Tools menu

The context is the GameContext component, inside which you can install Installers or select Lifetime.

## Lifetimes
- Scene - lives only on the current scene
- Object - lives only on this object
- Project - lives in the entire project.

## Installers
When adding an Installer to the Installers array on a context, this Installer will register dependencies exclusively into the container of this context.

## Usage example
```csharp
public class MyBehaviour : MonoBehaviour 
{
    [SerializeField] private GameContext _sceneContext;
    
    public void Start() 
    {
        var myService = _sceneContext.Container.Resolve<MyService>()
        
        myService.Print("Hello World")
    }
}
```
In this example, we access the context that we set manually through the inspector, access its container, and get the MyService dependency.

## ContextManager
The main context manager. It installs all dependencies and also implements dependencies into components. DI won't work without it.
By accessing ContextManager.InjectionManager, you can manually inject dependencies into any GameObject, component, or object.

## Notes
- Containers from each context are independent of each other.