## Description
The context is used to separate dependency containers into stage and project containers.

## Usage example

To use the context, you need to add it to the scene via the Tools -> Create Context menu, or manually add the Context component to the GameObject.

The Installers field in the context is used to add installers to this context. All installers that you add to this context register the dependency in the container of this context.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [SerializeField] private Context _sceneContext;
    
    public void Start() 
    {
        var myService = _sceneContext.Resolve<MyService>()
        
        myService.Print("Hello World")
    }
}
```

In this example, we access the context, which we set manually through the inspector, or we can find it on the scene through FindAnyObjectByType, access its container and get the MyService dependency, for more information about registration, see Register.md

ObjectContext is a key component of dependency injection. It is added manually to each object on the scene that uses dependency injection. It stores a container of all the components of that object, as well as a container of services registered with that object, which can also be registered using Installers.

ContextManager is the main object for installing containers. Without it, dependency injection will not work on the stage.

## Notes
- There are 3 types of context - for object, for the stage and for the project.
- The container from each context is independent of each other.


---

## Описание
Контекст используется для разделения контейнеров зависимостей на сценовый и проектный.

## Пример использования

Для использования контекста необходимо добавить его на сцену через меню Tools -> Create Context, или вручную добавить на GameObject компонент Context.

Поле Installers в контексте используется для добавления инсталлеров в этот контекст. Все инсталлеры, которые вы добавляете в данный контекст, регистрируют зависимость в контейнер этого контекста.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [SerializeField] private Context _sceneContext;
    
    public void Start() 
    {
        var myService = _sceneContext.Resolve<MyService>()
        
        myService.Print("Hello World")
    }
}
```
В этом примере мы обращаемся к контексту, который устанавливаем вручную через инспектор, обращаемся к его контейнеру и получаем зависимость MyService, подробнее про регистрацию см. в Register.md.

ObjectContext - ключевой компонент внедрения зависимостей. Он добавляется вручную на каждый обьект на сцене, использующий внедрение зависимостей. Он хранит в себе контейнер всех компонентов этого обьекта, и контейнер зарегистрированных на данный обьект сервисов, которые также можно регистрировать с помощью Installer-ов.

ContextManager - главный обьект инсталляции контейнеров. Без него внедрение зависимостей на сцене работать не будет.

## Примечания
- Существует 3 вида контекста - на обьект, на сцену и на проект.
- Контейнер из каждого контекста независимы друг от друга.
