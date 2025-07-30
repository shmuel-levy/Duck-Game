# Duck Game Clone - Script Architecture

This document describes the organized architecture of the Duck Game Clone scripts.

## Architecture Overview

The project uses a **clean, event-driven architecture** with clear separation of concerns:

```
Assets/Scripts/
├── Core/           # Data structures and game state
├── Managers/       # Singleton managers for different systems
├── Controllers/    # Player and enemy controllers
├── Systems/        # Weapon systems, movement systems, etc.
├── UI/            # User interface components
└── Utils/         # Utility classes and helpers
```

## Namespace Structure

All scripts are organized under the `DuckGame` namespace with sub-namespaces:

- `DuckGame.Core` - Core data structures and events
- `DuckGame.Managers` - Singleton managers
- `DuckGame.Controllers` - Player and enemy controllers
- `DuckGame.Systems` - Game systems
- `DuckGame.UI` - User interface components
- `DuckGame.Utils` - Utility classes

## Core Components

### Core/GameState.cs
Contains the main data structures:
- `GameState` - Overall game state (score, health, game status)
- `PlayerData` - Player-specific data (position, health, jumps)
- `WeaponData` - Weapon information (ammo, fire rate, etc.)

### Core/Events.cs
Centralized event system using Unity's event system:
- Player events (health, score, death, jump, land)
- Game state events (start, over, pause, resume)
- Weapon events (fire, switch, reload)
- Audio and particle events

## Managers

### Managers/GameManager.cs
- Manages overall game state
- Handles scoring and UI updates
- Uses event-driven architecture
- Singleton pattern for easy access

### Managers/AudioManager.cs
- Centralized audio management
- Event-driven sound system
- Supports music and SFX
- Object pooling for performance

### Managers/ParticleEffectsManager.cs
- Manages particle effects
- Object pooling for particles
- Event-driven particle system

## Controllers

### Controllers/DuckController.cs
- Player character controller
- Movement and jumping mechanics
- Health and damage system
- Event-driven interactions

### Controllers/Enemy.cs
- Enemy AI and behavior
- State machine for patrol/chase/attack
- Damage and death handling

## Systems

### Systems/WeaponSystem.cs
- Weapon management
- Ammo and reload system
- Weapon switching

### Systems/MovementSystem.cs
- Movement mechanics
- Ground detection
- Physics-based movement

## Event-Driven Communication

The architecture uses Unity's event system for loose coupling:

```csharp
// Subscribe to events
GameEvents.OnPlayerHealthChanged.AddListener(OnPlayerHealthChanged);

// Trigger events
GameEvents.OnPlayerHealthChanged.Invoke(newHealth);
```

## Benefits of This Architecture

1. **Separation of Concerns** - Each component has a single responsibility
2. **Loose Coupling** - Components communicate through events
3. **Testability** - Easy to unit test individual components
4. **Maintainability** - Clear organization makes code easy to understand
5. **Scalability** - Easy to add new features without breaking existing code
6. **Performance** - Object pooling and optimized event handling

## Usage Examples

### Adding a New Manager
```csharp
namespace DuckGame.Managers
{
    public class NewManager : MonoBehaviour
    {
        public static NewManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
```

### Creating a New Event
```csharp
// In Core/Events.cs
public static UnityEvent OnNewEvent = new UnityEvent();

// Trigger the event
GameEvents.OnNewEvent.Invoke();

// Subscribe to the event
GameEvents.OnNewEvent.AddListener(OnNewEventHandler);
```

### Adding Player Data
```csharp
// In Core/GameState.cs
[Serializable]
public class PlayerData
{
    public float newProperty;
    // ... existing properties
}
```

## Troubleshooting Common Issues

### Namespace Errors
If you get errors like "The type or namespace name 'X' could not be found":

1. **Add the correct using directive**:
   ```csharp
   using DuckGame.Controllers;  // For DuckController
   using DuckGame.Managers;     // For managers
   using DuckGame.Core;         // For events and data
   ```

2. **Check namespace declarations**:
   ```csharp
   namespace DuckGame.Controllers
   {
       public class MyController : MonoBehaviour
       {
           // Your code here
       }
   }
   ```

3. **Common namespace mappings**:
   - `DuckController` → `DuckGame.Controllers`
   - `GameManager` → `DuckGame.Managers`
   - `AudioManager` → `DuckGame.Managers`
   - `Enemy` → `DuckGame.Controllers`

### Script Organization
- **Managers** go in `Assets/Scripts/Managers/`
- **Controllers** go in `Assets/Scripts/Controllers/`
- **Systems** go in `Assets/Scripts/Systems/`
- **UI** components go in `Assets/Scripts/UI/`
- **Utilities** go in `Assets/Scripts/Utils/`

### Unity Inspector Issues
After reorganizing scripts, you may need to:
1. Reassign script references in the Inspector
2. Check that all prefabs have the correct script references
3. Verify that event listeners are properly connected

## Migration Guide

If you have existing scripts that need to be updated:

1. **Add namespace** - Wrap class in appropriate namespace
2. **Use events** - Replace direct manager calls with events
3. **Update references** - Update any hardcoded references to use the new structure
4. **Test thoroughly** - Ensure all functionality still works

## Best Practices

1. **Always use events** for communication between components
2. **Keep managers as singletons** for easy access
3. **Use the namespace structure** to organize code
4. **Document your events** in the Events.cs file
5. **Test your components** in isolation
6. **Follow Unity naming conventions** for consistency

## File Organization

### Core/
- `GameState.cs` - Data structures
- `Events.cs` - Event system

### Managers/
- `GameManager.cs` - Game state management
- `AudioManager.cs` - Audio system
- `ParticleEffectsManager.cs` - Particle effects

### Controllers/
- `DuckController.cs` - Player controller
- `Enemy.cs` - Enemy AI

### Systems/
- `WeaponSystem.cs` - Weapon management
- `MovementSystem.cs` - Movement mechanics

### UI/
- UI-related components (to be added)

### Utils/
- `ScriptNamespaceHelper.cs` - Namespace management helper
- Utility classes and helpers

## Script Location Note

The Scripts folder is located inside the Assets folder (`Assets/Scripts/`). This is a common Unity practice and is perfectly fine. Some projects place Scripts outside Assets, but both approaches work well. The current organization follows Unity's recommended structure.

This architecture provides a solid foundation for a scalable, maintainable game project. 