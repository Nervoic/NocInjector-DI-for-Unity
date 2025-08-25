## Description
Allows you to register a service or interface implementation.

## Usage example

```csharp
public class MyBehaviour : MonoBehaviour 
{
    
}
```

In this example, the type does not require registration, since it is a component, and can be obtained from any component located on the same GameObject.

```csharp
public class MyService
{
    
}
```
In this case, the service requires registration, as it is not a component. For more information, see below

## Registration
Only those types that are not components require registration. Components require registration only if they implement an interface.

```csharp
public class MyService : MonoBehaviour
{
    
}
```

```csharp
public class MyServiceInstaller : Installer
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton)
    }
}
```
In this example, we manually register the service in a separate class. This Installer will need to be moved to the Installers field of the context in which you want to register the service.

## Registration of interfaces
Any types that can be requested as an interface implementation require registration as an interface implementation.
```csharp
[RegisterComponentAsImplementation(typeof(IBehaviour), "Behaviour")]
public class MyBehaviour : MonoBehaviour : IBehaviour
{
    
}
```
```csharp
public class MyService : IBehaviour
{
    
}
```
```csharp
public class MyServiceInstaller : Installer
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton).AsRealisation<IBehaviour>("Service")
    }
}
```
In these examples, we register the component and the service as an interface implementation. You can get the implementation by tag using the [InjectImplementation] attribute. For more information, see Inject.md The [RegisterComponentAsImplementation] attribute only works for registering components as an interface implementation, services need to be registered completely manually.


## Notes
- There is no need for Installers for components.
- The [RegisterCoponentAsImplementation] attribute only works for components.


---
## Описание
Позволяет зарегистрировать сервис или реализацию интерфейса.

## Пример использования

```csharp
public class MyBehaviour : MonoBehaviour 
{
    
}
```

В этом примере тип не требует регистрации, так как является компонентом, и может быть получен из любого компонента, находящимся на том же GameObject-е.

```csharp
public class MyService
{
    
}
```
В этом сервис требует регистрации, так как не является компонентом. Подробнее см. дальше

## Регистрация
Регистрацию требуют только те типы, которые не являются компонентами. Компоненты требует регистрацию только, если реализуют интерфейс.

```csharp
public class MyService : MonoBehaviour
{
    
}
```

```csharp
public class MyServiceInstaller : Installer
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton)
    }
}
```
В этом примере мы вручную регистрируем сервис в отдельном классе. Данный Installer необходимо будет перенести в поле Installers того контекста, в котором вы хотите зарегистрировать сервис.

## Регистрация интерфейсов
Любые типы, которые могут быть запрошены как реализация интерфейса, требуют регистрацию как реализация интерфейса.
```csharp
[RegisterComponentAsImplementation(typeof(IBehaviour), "Behaviour")]
public class MyBehaviour : MonoBehaviour : IBehaviour
{
    
}
```
```csharp
public class MyService : IBehaviour
{
    
}
```
```csharp
public class MyServiceInstaller : Installer
{
    public override void Install(ServiceContainer container) 
    {
        container.Register<MyService>(ServiceLifeTime.Singleton).AsRealisation<IBehaviour>("Service")
    }
}
```
В данных примерах мы регистрируем компонент и сервис как реализацию интерфейса. Получить реализацию можно будет по тегу с помощью атрибута [InjectImplementation], подробнее см. в Inject.md. Атрибут [RegisterComponentAsImplementation] работает только для регистрации компонентов как реализацию интерфейса, сервисы нужно регистрировать полностью вручную.


## Примечания
- Нет необходимости Installer-ы для компонентов.
- Атрибут [RegisterCoponentAsImplementation] работает только для компонентов.