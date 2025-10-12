## Description
The call system

## The main thing
- Creation of custom calls, analogous to events in C#
- Automatic event monitoring
- Connection with contexts, the ability to link subscriptions from different parts of the game
- Create and register Call Views in the areas you need

## Registration of calls

```csharp
public class MyInstaller : Installer
{
    public void Install() 
    {
        Register<CallView>;
    }
}
```

Here we register the CallView, later we need to move this Installer to the context we need.


## Usage
We need to implement CallView. To subscribe or unsubscribe, we use Follow or Unfollow, and to call Call
```csharp
public class CallsUser : MonoBehaviour
{
    [Inject] private CallView _view;
    
    [OnInjected] private void Follow() 
    {
        _view.Follow<CallInfo>(OnCall)
        _view.Call<CallInfo>()
        
        _view.Unfollow<CallInfo>(OnCall)
    }
    
    private void OnCall() 
    {
        
    }
}
```

## Notes
- A method called with a Call can accept only one type of parameter.


---
## Описание
Система звонков

## Главное
- Создание кастомных звонков, аналог событиям в C#
- Автоматический контроль событий
- Связь с контекстами, возможность связывать подписки из разных частей игры
- Создание и регистрация View-ов на звонки в нужных вам областях

## Регистрация звонков

```csharp
public class MyInstaller : Installer
{
    public void Install() 
    {
        Register<CallView>;
    }
}
```

Тут мы регистрируем CallView, позже этот Installer нам нужно перенести в необходимый нам контекст.


## Использование
Нам необходимо внедрить CallView. Для подписки или отписки используем Follow или Unfollow, а для вызова Call
```csharp
public class CallsUser : MonoBehaviour
{
    [Inject] private CallView _view;
    
    [OnInjected] private void Follow() 
    {
        _view.Follow<CallInfo>(OnCall)
        _view.Call<CallInfo>()
        
        _view.Unfollow<CallInfo>(OnCall)
    }
    
    private void OnCall() 
    {
        
    }
}
```

## Примечания
- Метод, вызываемый с помощью Call может принимать параметр только одного типа.