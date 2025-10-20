## Description
Registering dependencies in containers.

## The main thing
- Registration using implementation tags is available.
- When registering, you can also specify an tag, an instance, an interface that implements a dependency, a GameObject on which the component is located, and a condition.

## AsImplementation
Registers the dependency as an implementation of the interface.
```csharp
public class MyInstaller : Installer
{
    public void Install(ContainerView container) 
    {
        container.Register<MyService>(Lifetime.Singleton).AsImplementation<IMyService>;
    }
}
```

## WithTag
Adds an tag to the dependency.
```csharp
public class MyInstaller : Installer
{
    public void Install(ContainerView container) 
    {
        container.Register<MyService>(Lifetime.Transient).WithTag("MainImp")
    }
}
```

## AsComponentOn
Be sure to use it when registering components. Specifies which GameObject the component is located on.
```csharp
public class MyInstaller : GameInstaller
{
    [SerializeField] private GameObject _gameObj;
    public void Install(ContainerView container) 
    {
        container.Register<MyService>().AsComponentOn(_gameObj);
    }
}
```
If the component is registered with Lifetime Transient, then each time it is requested, an instance of this GameObject will be created on the stage.

## SetInstance
Adds an instance to the registered dependency. It cannot be used when registering components.

```csharp
public class MyInstaller : GameInstaller
{
    [SerializeField] private GameObject _gameObj;
    public void Install(ContainerView container) 
    {
        container.Register<MyService>().SetInstance(new MyService("Tag"))
    }
}
```

## If
Cancels registration if the condition has not been met.

```csharp
public class MyInstaller : GameInstaller
{
    [SerializeField] private GameObject _gameObj;
    public void Install(ContainerView container) 
    {
        container.Register<MyService>().If(3 > 2)
    }
}
```


## Notes
- Dynamic registration is available if you access the container in Runtime and register a dependency in it.