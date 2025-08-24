The `[Inject]` attribute is used to automatically inject dependencies into the fields of Unity components. Component injection only works if the requested component is located on the same GameObject where it was requested.

## Description
Allows you to automatically fill in the fields with the necessary services registered in the container or components.

## Usage example

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```

In this example, the `_service` field will be automatically filled in by the `MyService` instance if it is registered in the container.

```csharp
public class MyBehaviour : MonoBehaviour
    { 
        [Inject] private MyBehaviour2 _behaviour
    }
```
In this example, the `_behaviour` field will be automatically populated with an instance of `MyBehaviour2` if it is on the same GameObject.

## Manual injection
To inject types that are not components, we need to get the context from which we will get the dependency.
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
To inject components, we need to get the InjectObject component, access its container and request a dependency.

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

## Interface injection
To inject interfaces, we first need to register a component or service as an interface implementation. For more information about registration, see Register.md
```csharp
[RegisterAsRealisation(typeof(IBehaviour), "Behaviour")]
public class MyBehaviour : MonoBehaviour : IBehaviour
{
    [SerializeField] private Context sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
After that, when requesting injection via the interface, specify the tag of the implementation that we want to receive. This works for both components and services.

```csharp
public class MyBehaviour : MonoBehaviour
{
    [InjectByRealisation("Behaviour")] private IBehaviour _behaviour
}
```


## Notes
- The attribute only works with fields and properties
- The attribute does not work for interfaces. To embed the interface, use the InjectByInterface attribute.
- Injection of all dependencies on an object takes place in Awake
- In non-component types, injection occurs after you request a dependency. NocInjector also supports recursive injection.


---
Атрибут `[Inject]` используется для автоматической инъекции зависимостей в поля компонентов Unity. Иньекция компонентов работает только в случае, если запрашиваемый компонент находится на том же GameObject-е, где его запросили

## Описание
Позволяет автоматически заполнять поля нужными сервисами, зарегистрированными в контейнере или компонентами

## Пример использования

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```

В этом примере поле `_service` будет автоматически заполнено экземпляром `MyService`, если он зарегистрирован в контейнере.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyBehaviour2 _behaviour
}
```
В этом примере поле `_behaviour` будет автоматически заполнено экземпляром `MyBehaviour2`, если он находится на том же GameObject-е.

## Ручная иньекция
Для иньекции типов, не являющихся компонентами, нам необходимо получить контекст, из которого мы будем получать зависимость.
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
Для иньекции компонентов нам необходимо получить компонент InjectObject, обратиться к его контейнеру и запросить зависимость.

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

## Иньекция интерфейсов
Для иньекции интерфейсов нам сначала необходимо зарегистрировать компонент или сервис как реализацию интерфейса. Подробнее про регистрацию см. в Register.md
```csharp
[RegisterAsRealisation(typeof(IBehaviour), "Behaviour")]
public class MyBehaviour : MonoBehaviour : IBehaviour
{
    [SerializeField] private Context sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
После чего при запросе иньекции по интерфейсу указать тег реализации, которую мы хотим получить. Это работает как для компонентов так и для сервисов.

```csharp
public class MyBehaviour : MonoBehaviour
{
    [InjectByRealisation("Behaviour")] private IBehaviour _behaviour
}
```


## Примечания
- Атрибут работает только с полями и свойствами
- Атрибут не работает для интерфейсов. Для внедрения интерфейса используйте атрибут InjectByInterface
- Инъекция всех зависимостей на обьекте происходит в Awake
- В типы, не являющиеся компонентами иньекция происходит после того, как вы запросите зависимость. NocInjector также поддерживает рекурсивную иньекцию.
