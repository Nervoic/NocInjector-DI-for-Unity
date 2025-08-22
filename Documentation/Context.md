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

## Notes
- There are 2 types of context - for the stage and for the project.
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
В этом примере мы обращаемся к контексту, который устанавливаем вручную через инспектор, или можем найти его на сцене через FindAnyObjectByType, обращаемся к его контейнеру и получаем зависимость MyService, подробнее про регистрацию см. в Register.md

## Примечания
- Существует 2 вида контекста - на сцену и на проект.
- Контейнер из каждого контекста независимы друг от друга.
