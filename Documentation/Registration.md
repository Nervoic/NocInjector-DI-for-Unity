## Description
Registration of dependencies.

## The main thing
- Registration using implementation tags is available.

## Registration of interfaces

```csharp
public class MyInstaller : Installer
{
    public void Install(DependencyContainer container) 
    {
        container.Register<MyService>.AsImplementation<IMyService>.WithId("MainImp")
    }
}
```

To register a dependency as an implementation of an interface, use the AsImplementation method for the registered dependency. To add an ID, use the withId method, which is available both for registering regular dependencies and for interface implementations.


## Standard registration
Components can be registered both on the current GameObject, where they are located (if they have an ObjectContext), and as a component on another GameObject.

If you register a component in a context located on a GameObject on which this component does not exist, and do not specify the GameObject of the component to be registered during registration, an attempt to obtain a dependency will cause an error.

```csharp
public class MyInstaller : MonoBehaviour
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyBehaviour>()
    }
}
```
An error occurs if you drag this Installer into a context that does not have this component on its GameObject.
```csharp
public class MyServiceInstaller : Installer
{
    [SerializeField] private GameObject behaviourObject;
    
    public override void Install(ServiceContainer container) 
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
    public void Install(DependencyContainer container) 
    {
        container.Register<MyService>.AsImplementation<IMyService>.WithId("MainImp")
    }
}
```

Для регистрации зависимости как реализации какого либо интерфейса, используйте метод AsImplementation к регистрируемой зависимости. Для добавления ID используйте метод WithId, который доступен как для регистрации обычных зависимостей так и для реализаций интерфейсов.


## Стандартная регистрация
Компоненты могут быть зарегистрированы как на текущем GameObject, где они находятся(если они имеет ObjectContext), так и как компонент на другом GameObject.

Если зарегистрировать компонент в контексте, находящимся на GameObject, на котором данного компонента нет, и не указать GameObject регистрируемого компонента при регистрации, попытка получить зависимость вызовет ошибку.

```csharp
public class MyInstaller : MonoBehaviour
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyBehaviour>()
    }
}
```
Ошибка, если вы перетащите данный Installer в контекст, на GameObject-е которого нет данного компонента.
```csharp
public class MyServiceInstaller : Installer
{
    [SerializeField] private GameObject behaviourObject;
    
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyBehaviour>().AsComponentOn(behaviourObject)
    }
}
```
Можно регистрировать в любом контексте, если BehaviourObject действительно имеет компонент MyBehaviour.


## Примечания
- Доступна динамическая регистрация, если вы обратитесь к контейнеру в Runtime и зарегистрируете в него зависимость.