## Description
Registration of dependencies.

## The main thing
- Registration using implementation tags is available.

## Registration of interfaces

```csharp
public class MyInstaller : Installer
{
    public void Install(ContainerView container) 
    {
        container.Register<MyService>.AsImplementation<IMyService>.WithId("MainImp")
    }
}
```

To register a dependency as an implementation of an interface, use the AsImplementation method for the registered dependency. To add an ID, use the withId method, which is available both for registering regular dependencies and for interface implementations.


## Standard registration
When registering components, you must explicitly specify the GameObject on which the component is located.
```csharp
public class MyServiceInstaller : Installer
{
    [SerializeField] private GameObject behaviourObject;
    
    public override void Install(ContainerView container) 
    {
        container.Register<MyBehaviour>().AsComponentOn(behaviourObject)
    }
}
```
It can be registered in any context, as long as the BehaviourObject actually has a MyBehaviour component.


## Notes
- Dynamic registration is available if you access the container in Runtime and register a dependency in it.


---
## Описание
Регистрация зависимостей.

## Главное
- Доступна регистрация с использованием тегов реализации.

## Регистрация интерфейсов

```csharp
public class MyInstaller : Installer
{
    public void Install(ContainerView container) 
    {
        container.Register<MyService>.AsImplementation<IMyService>.WithId("MainImp")
    }
}
```

Для регистрации зависимости как реализации какого либо интерфейса, используйте метод AsImplementation к регистрируемой зависимости. Для добавления ID используйте метод WithId, который доступен как для регистрации обычных зависимостей так и для реализаций интерфейсов.


## Стандартная регистрация
При регистрации компонентов необходимо явно указать GameObject, на котором данный компонент находится.
```csharp
public class MyServiceInstaller : Installer
{
    [SerializeField] private GameObject behaviourObject;
    
    public override void Install(ContainerView container) 
    {
        container.Register<MyBehaviour>().AsComponentOn(behaviourObject)
    }
}
```
Можно регистрировать в любом контексте, если BehaviourObject действительно имеет компонент MyBehaviour.


## Примечания
- Доступна динамическая регистрация, если вы обратитесь к контейнеру в Runtime и зарегистрируете в него зависимость.