## Description
Allows you to automatically fill in the fields with the necessary dependencies registered in the container.

## The main thing

- The `[Inject]` attribute
  It is used for automatic dependency injection into the fields of Unity components.

- The Inject attribute can take 2 parameters - the tag that you specified during registration, and the context from which the dependency will be injected.
- For fields and properties - if you specified an tag during registration, then during implementation you also need to specify the tag of the dependency you want to receive, otherwise the dependency will not be detected in the container.,
- For arrays, if the tag was not specified, all registered dependencies of this type will be implemented.

By default, the context from which the dependency will be injected is All (of all contexts)

You can also set a context - Object (only from the current context of the object), Scene (only from the context of the scene), Project (only from the context of the project)
## Usage example

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```

In this example, the `_service` field will be automatically filled in by the `MyService` instance if it is registered in the container.

## Manual injection
To manually resolve dependencies, we need to get the context from which we will resolve the dependency.
```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private GameContext sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>("MyTag");
    }
}
```
We can also refer to the container of the current object (if it has a Context)

```csharp
public class MyBehaviour : MonoBehaviour
{
    private MyBehaviour2 _behaviour
    
    public void Start() 
    {
        _behaviour = gameObject.GetContext.Container.Resolve<MyBehaviour2>();
    }
}
```

## Interface injection
To inject interfaces, we first need to register the dependency as an interface implementation. For more information about registration, see Registration.md .

```csharp
public class MyBehaviour : MonoBehaviour
{
    [Inject("Behaviour")] private IBehaviour _behaviour
}
```

## The [OnInjected] attribute
You can mark the methods that will be called after the implementation of all dependencies on the object. It cannot be combined with the Inject attribute.


## Notes
- The attribute works with fields, properties, and methods.
- Injection of all dependencies on an object occurs later than Awake, but ends at Start
- Recursive injection is supported