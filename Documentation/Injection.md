## Description
Allows you to automatically fill in the fields with the necessary services registered in the container or components.

## The main thing

The `[Inject]` attribute is used to automatically inject dependencies into the fields of Unity components. Component injection only works if the requested component has been registered and is located on the same GameObject where it was requested, or is registered in the ProjectContext or SceneContext.

If the component is registered in the container on the 1st GameObject, and you request it from the 2nd GameObject, automatic injection will not work. You can manually access the container of the 1st GameObject (if it has an ObjectContext), or register this component in the global context.

- The Inject attribute can take 2 parameters - the implementation tag that you specified during registration, and the context from which the dependency will be injected.
- If you specified the implementation tag during registration, then during implementation you also need to specify the tag of the implementation you want to receive, otherwise the dependency will not be detected in the container.,

By default, the context from which the dependency will be injected is All (of all contexts, except ObjectContext, if they are not the current GameObject)

You can also set a context - Object (only from the current context of the object), Scene (only from the context of the scene), Project (only from the context of the project)
## Usage example

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```

In this example, the `_service` field will be automatically filled in by the `MyService` instance if it is registered in the container object or in the global context.

## Manual injection
To manually resolve dependencies, we need to get the context from which we will resolve the dependency.
```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private GameContext sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
We can also refer to the container of the current object (if it has an Context)

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
To inject interfaces, we first need to register a component or service as an interface implementation. For more information about registration, see Registration.md .

```csharp
public class MyBehaviour : MonoBehaviour
{
    [Inject("Behaviour")] private IBehaviour _behaviour
}
```

You can also add multiple tags in the parameters of the Inject attribute.


## Notes
- The attribute only works with fields and properties
- Injection of all dependencies on an object takes place in Awake
- Recursive injection is supported
- - The OnInjected attribute is used as a label for methods that will need to be called after dependency injection.


---

## Описание
Позволяет автоматически заполнять поля нужными сервисами, зарегистрированными в контейнере или компонентами

## Главное

Атрибут `[Inject]` используется для автоматической инъекции зависимостей в поля компонентов Unity. Иньекция компонентов работает только в случае, если запрашиваемый компонент был зарегистрирован, и находится на том же GameObject-е, где его запросили, или же зарегистрирован в ProjectContext или SceneContext.

Если компонент зарегистрирован в контейнере на 1-м GameObject, а вы запрашиваете его со 2-го GameObject, автоматическая иньекция работать не будет. Вы можете вручную обратиться к контейнеру 1-го GameObject (если он имеет ObjectContext), или же зарегистрировать этот компонент в глобальном контексте.

- Атрибут Inject может принимать 2 параметра - тег реализации, который вы указали при регистрации, и контекст, из которого будет внедряться зависимость.
- Если вы при регистрации указали тег реализации, то при внедрении вам также нужно указать тег реализации, которую вы хотите получить, иначе зависимость не будет обнаружена в контейнере,

По умолчанию контекст, из которого будет внедряться зависимость является All (из всех контекстов, за исключением ObjectContext, если они не являются текущим GameObject)

Вы также можете поставить контекст - Object (только из текущего контекста обьекта), Scene (только из контекста сцены), Project (только из контекста проекта)
## Пример использования

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject] private MyService _service;
}
```

В этом примере поле `_service` будет автоматически заполнено экземпляром `MyService`, если он зарегистрирован в контейнере обьекте или в глобальном контексте.

## Ручная иньекция
Для ручного разрешения зависимостей нам нужно получить контекст, из которого мы будем разрешать зависимость
```csharp
public class MyBehaviour : MonoBehaviour
{
    [SerializeField] private GameContext sceneContext;
    
    private MyService _service
    
    public void Start() 
    {
        _service = sceneContext.Container.Resolve<MyService>();
    }
}
```
Также мы можем обратиться к контейнеру текущего обьекта (если он имеет Context)

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

## Иньекция интерфейсов
Для иньекции интерфейсов нам сначала необходимо зарегистрировать компонент или сервис как реализацию интерфейса. Подробнее про регистрацию см. в Registration.md.

```csharp
public class MyBehaviour : MonoBehaviour
{
    [Inject("Behaviour")] private IBehaviour _behaviour
}
```

Вы также можете добавить множественные теги в параметрах атрибута Inject.


## Примечания
- Атрибут работает только с полями и свойствами
- Инъекция всех зависимостей на обьекте происходит в Awake 
- Поддерживается рекурсивная иньекция
- Атрибут OnInjected используется для как метка для методов, которые должны будут быть вызваны после иньекции зависимостей.
