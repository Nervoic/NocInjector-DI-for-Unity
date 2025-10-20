## Description
An event system with internal unsubscription control.

## The main thing
- Creation of custom "calls", analogous to events in C#
- Automatic event monitoring
- Connection with contexts, the ability to link subscriptions from different parts of the game
- CallField - represents a field or property, when changing the value of which all subscribers will be called.

## Registration of calls
We need to register the CallView in the container you need via the Installer.

```csharp
public class MyInstaller : Installer
{
    public void Install(ContainerView container) 
    {
        container.Register<CallView>;
    }
}
```


## Usage
We need to implement CallView;
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
To subscribe to a call, use the Follow method of the CallView class, and to unsubscribe, use the Unfollow method.

- The Follow method
  Returns IDisposable. When Disposing, the method will unsubscribe from all calls that it is subscribed to.

We can also manually register the call and get an IDisposable, when we call Dispose, all methods will be unsubscribed from the call.

## CallField
It represents a field, when updating the value of which all subscribers monitoring this field will be called.

```csharp
public class CallsUser : MonoBehaviour
{
    [Inject] private CallField<int> _balance = new(10);
    
    [OnInjected] private void Follow() 
    {
        _balance.Follow(BalanceChanged);
    }
    
    private void BalanceChanged(int newBalance) 
    {
        Debug.Log(newBalance);
    }
}
```
The same methods are used for subscriptions and unsubscriptions as in CallView.