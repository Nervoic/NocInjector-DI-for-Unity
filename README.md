
<img width="4200" height="1500" alt="Banner" src="https://github.com/user-attachments/assets/b20de985-8594-4682-a6de-63b946088e04" />

NocInjector is a lightweight DI (Dependency Injection) container for Unity that allows you to conveniently manage dependencies.
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
2. You can embed a dependency via an attribute or request it manually from the container.
3. You have full flexibility in registering and querying dependencies, allowing you to select contexts and tags.
4. Use the Inject attribute for automatic injection:
5. Use CallView to conveniently organize events within the project.

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

## Работает в Unity версии 2022+

## Основные возможности
- Регистрация и разрешение любых зависимостей
- Атрибуты для автоматической инъекции зависимостей
- Контексты для разделения областей видимости
- Внедрение с использованием тегов реализации
- Гибкость в регистрации и разрешении зависимостей
- Собственная система событий

## Быстрый старт

1. Для использования библиотеки вам необходимо вручную зарегистрировать зависимости.
2. Вы можете внедрить зависимость через атрибут или запросить ее вручную у контейнера.
3. У вас есть полная гибкость в регистрации и запросе зависимостей, позволяя выбирать контексты и теги.
4. Используйте атрибут Inject для автоматической инъекции:
5. Используйте CallView для удобной организации событий внутри проекта.

```csharp
public class MyBehaviour : MonoBehaviour 
{
    [Inject("Main")] private MyService _service;
}
```

## Примеры
См. другие файлы в папке Documentation для подробных примеров.

