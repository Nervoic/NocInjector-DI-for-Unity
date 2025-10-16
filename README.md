
<img width="4200" height="1500" alt="Banner" src="https://github.com/user-attachments/assets/b20de985-8594-4682-a6de-63b946088e04" />


## NocInjector - Dependency Injection Framework for Unity. Lightweight. Powerful. Elegant.
## Unity version - 2022+, tested on IL2CPP, WebGL, Mobile, PC

## NocInjector is a dependency injection system specifically designed for Unity, combining enterprise-level architecture with exceptional performance.

## Why NocInjector?
- Performance First
- Zero garbage allocation in runtime resolution
- A flexible dependency management system and an event system

## Optimized for high-frequency injection scenarios

### Powerful Features
```csharp
// Fluent API for elegant registration
container.Register<PlayerService>()
         .AsImplementation<IPlayerService>()
          .AsComponentOn(playerObject);
         .WithTag("Main")

// Advanced event-driven injection
[Inject("Main")]
private IAudioService _audioService;

// Hierarchical context system
[Inject(InjectContextLifetime.Project)]
private IGameConfig _config;
```
## Enterprise Ready
- Call system - Fully typed event system
- Method & Field Injection - Flexible dependency injection
- Context Management - Project/Scene/Object lifetime scopes and custom contexts
- Unity support - Deep Unity integration

## Installation
Install NocInjector.unitypackage, and add to your Unity-project

💡 ### Quick Start
```csharp
public class GameInstaller : MonoBehaviour
{
    private void Start()
    {
        var container = new ContainerView();
        
        container.Register<GameManager>(Lifetime.Singleton);
        container.Register<AudioService>().AsImplementation<IAudioService>();
    }
}

public class GameManager : MonoBehaviour
{
    [Inject] private IAudioService _audioService;
    
    private void Start()
    {
        _audioService.PlayMusic(); // Injected automatically!
    }
}
```
## Features Deep Dive
- Call system
```csharp
// Subscribe to events
systemView.Follow<PlayerDiedEvent>(OnPlayerDied);

// Publish events  
systemView.Call(new PlayerDiedEvent(player));
```
## Context Hierarchy
- Project Context - Global dependencies
- Scene Context - Scene-specific dependencies
- Object Context - GameObject-level scope

## Advanced Injection
```csharp
// Method injection
[Inject]
private void InitializeServices(IAudioService audio, IInputService input)
{
    // Dependencies injected automatically
}

// Post-injection callbacks
[OnInjected]
private void OnDependenciesInjected()
{
    // Called after all injections complete
}
```

## Perfect For
- Games of any complexity
- Mobile games requiring maximum performance
- Teams needing clean, maintainable and simple API
- Projects with extensive Unity integration
