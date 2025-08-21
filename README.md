NocInjector is a lightweight DI (Dependency Injection) container for Unity that allows you to conveniently manage dependencies between components and services.


## Main features
- Registration and authorization of components and services
- Attributes for automatic dependency injection and registration
- Contexts for separating scopes
- No need to register components.
- Implementation by interfaces using implementation tags

## Quick start

1. Add the InjectObject component to your GameObject and register the class:

Using the attribute
```csharp
[Register(ServiceLifeTime.Singleton, ContextLifeTime.Scene)]
public class MyService 
{
    
}
```
Or using the Installer
```csharp
public class MyServiceInstaller : Installer 
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton)
    }
}
```
For components, registration is automatic in the local ObjectContainer.
2. Request the service manually:

```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private Context sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
Example with a component request

```csharp
public class MyBehaviour : MonoBehaviour
{
    private MyBehaviour2 _behaviour
    
    public void Start() 
    {
        _behaviour = GetComponent<InjectObject>.Container.Resolve<MyBehaviour2>();
    }
}
```

3. Use the Inject attribute for automatic injection:

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```
An example of implementing a component
```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyBehaviour2 _behaviour
}
```

## For examples
See other files in the documentation folder for detailed examples.

---

NocInjector — это легковесный DI (Dependency Injection) контейнер для Unity, позволяющий удобно управлять зависимостями между компонентами и сервисами.


## Основные возможности
- Регистрация и разрешение компонентов и сервисов
- Атрибуты для автоматической инъекции и регистрации зависимостей
- Контексты для разделения областей видимости
- Отсутствие необходимости регистрировать компоненты.
- Внедрение по интерфейсам с использованием тегов реализации

## Быстрый старт

1. Добавьте компонент InjectObject на ваш GameObject и зарегистрируйте класс:

С помощью атрибута
```csharp
[Register(ServiceLifeTime.Singleton, ContextLifeTime.Scene)]
public class MyService 
{
    
}
```
Или с помощью Installer-а
```csharp
public class MyServiceInstaller : Installer 
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton)
    }
}
```
Для компонентов регистрация автоматическая в локальном ObjectContainer-е
2. Запросите сервис вручную:

```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private Context sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
Пример с запросом компонента

```csharp
public class MyBehaviour : MonoBehaviour
{
    private MyBehaviour2 _behaviour
    
    public void Start() 
    {
        _behaviour = GetComponent<InjectObject>.Container.Resolve<MyBehaviour2>();
    }
}
```

3. Используйте атрибут Inject для автоматической инъекции:

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```
Пример с внедрением компонента
```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyBehaviour2 _behaviour
}
```

## Примеры
См. другие файлы в папке documentation для подробных примеров.

