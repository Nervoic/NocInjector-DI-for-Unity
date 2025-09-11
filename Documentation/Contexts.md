## Description
The context is used to separate dependency containers into an object, a scene, and a project.

## The main thing
- To use the context, you need to add it to the scene via the Tools menu
- Be sure to add **ContextManager** to the stage also via the Tools menu
- ObjectContext is added to each GameObject manually.

There are 2 types of context - GameContext and ObjectContext,

- GameContext has a Lifetime parameter, configurable in the inspector, which indicates the lifetime of the context.

- ObjectContext lives on a single GameObject.
## Usage example

The Installers field in the context is used to add installers to this context. All installers that you add to this context register the dependency in the container of this context.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [SerializeField] private GameContext _sceneContext;
    
    public void Start() 
    {
        var myService = _sceneContext.Container.Resolve<MyService>()
        
        myService.Print("Hello World")
    }
}
```
In this example, we access the context, which we set manually through the inspector, access its container and get the MyService dependency. For more information about registration, see Registration.md.

The ContextManager must exist on the stage. Dependency injection on the stage will not work without it.

## Notes
- There are 3 types of context - for the object, for the stage and for the project.
- Containers from each context are independent of each other.


---

## Описание
Контекст используется для разделения контейнеров зависимостей на обьект, сценe и проект.

## Главное
- Для использования контекста необходимо добавить его на сцену через меню Tools
- Обязательно добавить на сцену **ContextManager** также через меню Tools
- ObjectContext добавляется на каждый GameObject вручную.

Существует 2 вида контекста - GameContext и ObjectContext, 

- GameContext имеет параметр Lifetime, настраиваемый в инспекторе, который указывает на время жизни контекста.

- ObjectContext живет на единственном GameObject.
## Пример использования

Поле Installers в контексте используется для добавления инсталлеров в этот контекст. Все инсталлеры, которые вы добавляете в данный контекст, регистрируют зависимость в контейнер этого контекста.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [SerializeField] private GameContext _sceneContext;
    
    public void Start() 
    {
        var myService = _sceneContext.Container.Resolve<MyService>()
        
        myService.Print("Hello World")
    }
}
```
В этом примере мы обращаемся к контексту, который устанавливаем вручную через инспектор, обращаемся к его контейнеру и получаем зависимость MyService, подробнее про регистрацию см. в Register.md.

ContextManager обязательно должен существовать на сцене. Без него внедрение зависимостей на сцене работать не будет.

## Примечания
- Существует 3 вида контекста - на обьект, на сцену и на проект.
- Контейнеры из каждого контекста независимы друг от друга.
