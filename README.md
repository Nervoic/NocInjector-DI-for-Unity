
<img width="4200" height="1500" alt="Banner" src="https://github.com/user-attachments/assets/b20de985-8594-4682-a6de-63b946088e04" />


NocInjector is a lightweight DI (Dependency Injection) container for Unity that allows you to conveniently manage dependencies. It combines simplicity and great power, covering almost any requirements when developing your projects on Unity. If you don't have enough library functionality, the door is open for you to write your own extensions.

## Works in Unity version 2022+

## Main features
- Registration and resolution of any dependencies
- Attributes for automatic dependency injection
- Contexts for separating scopes
- Implementation using implementation tags
- Flexibility in registering and resolving dependencies
- Own event system

## Quick start

1. To use the library, you need to manually register the dependencies.
   
```csharp
public class MyInstaller : Installer 
{
    public void Install()
    {
       Register<MyBehaviour>()
    }
}
```
This Installer needs to be dragged into the Installers field of the context where you want to register it.


2. Dependency injection.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject("Main")] private MyService _service;
}
```

## Examples
In the Documentation folder you can find detailed examples and complete documentation.
