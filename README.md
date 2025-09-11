NocInjector is a lightweight DI (Dependency Injection) container for Unity that allows you to conveniently manage dependencies.

## Works in Unity version 2022+

## Main features
- Registration and resolution of any dependencies
- Attributes for automatic dependency injection
- Contexts for separating scopes
- Implementation using implementation tags
- Flexibility in registering and resolving dependencies

## Quick start

1. Register the dependency using the Installer and drag it to the Installers field of the context you need.

```csharp
public class MyServiceInstaller : Installer 
{
    public override void Install(DependencyContainer container) 
    {
        container.Register<MyService>(Lifetime.Transient).WithId("Main")
    }
}
```

2. Request the dependency manually:

```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private GameContext sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>("Main");
    }
}
```

3. Use the Inject attribute for automatic injection:

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject("Main")] private MyService _service;
}
```

## For examples
See other files in the Documentation folder for detailed examples.

---

NocInjector — это легковесный DI (Dependency Injection) контейнер для Unity, позволяющий удобно управлять зависимостями.


## Основные возможности
- Регистрация и разрешение любых зависимостей
- Атрибуты для автоматической инъекции зависимостей
- Контексты для разделения областей видимости
- Внедрение с использованием тегов реализации
- Гибкость в регистрации и разрешении зависимостей

## Быстрый старт

1. Зарегистрируйте зависимость с помощью Installer-а и перетащите его в поле Installers нужного вам контекста.

```csharp
public class MyServiceInstaller : Installer 
{
    public override void Install(DependencyContainer container) 
    {
        container.Register<MyService>(Lifetime.Transient).WithId("Main")
    }
}
```

2. Запросите зависимость вручную:

```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private GameContext sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>("Main");
    }
}
```

3. Используйте атрибут Inject для автоматической инъекции:

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject("Main")] private MyService _service;
}
```

## Примеры
См. другие файлы в папке Documentation для подробных примеров.

