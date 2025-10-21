# Documentation LoaderService

## Fran�ais

### Vue d'ensemble
Le `LoaderService` est un service de gestion d'indicateurs de chargement pour les applications Blazor. Il utilise un syst�me de compteur thread-safe qui permet de g�rer plusieurs op�rations de chargement simultan�es.

### Principe de fonctionnement
- **Compteur interne** : Le service maintient un compteur (`_requestCount`) qui suit le nombre d'op�rations en cours
- **Thread-safe** : Utilise `Interlocked` pour garantir la s�curit� dans les environnements multi-thread�s
- **�v�nements** : D�clenche `OnChange` � chaque modification d'�tat

### M�thodes disponibles

#### M�thodes simples
```csharp
LoaderService.Show();   // Affiche le loader
LoaderService.Close();  // Masque le loader
```

#### M�thodes avanc�es
```csharp
LoaderService.Increment(); // Incr�mente le compteur (+1)
LoaderService.Decrement(); // D�cr�mente le compteur (-1)
LoaderService.Reset();     // Remet � z�ro (fermeture forc�e)
```

### Exemples d'utilisation

#### Utilisation basique
```csharp
@inject LoaderService LoaderService

private async Task ChargerDonnees()
{
    LoaderService.Show();
    try
    {
        var donnees = await ApiService.GetDataAsync();
        // Traitement des donn�es...
    }
    finally
    {
        LoaderService.Close();
    }
}
```

#### Gestion de plusieurs op�rations
```csharp
// D�marrer 3 op�rations en parall�le
LoaderService.Increment(); // Compteur = 1 ? Loader affich�
LoaderService.Increment(); // Compteur = 2 ? Loader reste affich�
LoaderService.Increment(); // Compteur = 3 ? Loader reste affich�

// Terminer les op�rations une par une
LoaderService.Decrement(); // Compteur = 2 ? Loader reste affich�
LoaderService.Decrement(); // Compteur = 1 ? Loader reste affich�
LoaderService.Decrement(); // Compteur = 0 ? Loader se masque
```

#### Gestion d'erreur avec Reset
```csharp
try
{
    LoaderService.Show();
    await OperationRisquee();
}
catch (Exception ex)
{
    LoaderService.Reset(); // Force la fermeture en cas d'erreur
    // Gestion d'erreur...
}
```

#### Utilisation dans un service
```csharp
public class DataService
{
    private readonly LoaderService _loaderService;
    private readonly HttpClient _httpClient;

    public async Task<List<User>> GetUsersAsync()
    {
        _loaderService.Show();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("api/users");
        }
        finally
        {
            _loaderService.Close();
        }
    }
}
```

### Bonnes pratiques

1. **Utilisez toujours try/finally** pour garantir la fermeture du loader
2. **Pr�f�rez Show()/Close()** pour les cas simples
3. **Utilisez Increment()/Decrement()** pour les op�rations complexes
4. **Reset() uniquement en cas d'urgence** (erreurs critiques)

---

## English

### Overview
The `LoaderService` is a loading indicator management service for Blazor applications. It uses a thread-safe counter system that allows managing multiple simultaneous loading operations.

### How it works
- **Internal counter**: The service maintains a counter (`_requestCount`) that tracks the number of ongoing operations
- **Thread-safe**: Uses `Interlocked` to ensure safety in multi-threaded environments
- **Events**: Triggers `OnChange` on every state modification

### Available methods

#### Simple methods
```csharp
LoaderService.Show();   // Display the loader
LoaderService.Close();  // Hide the loader
```

#### Advanced methods
```csharp
LoaderService.Increment(); // Increment counter (+1)
LoaderService.Decrement(); // Decrement counter (-1)
LoaderService.Reset();     // Reset to zero (forced closure)
```

### Usage examples

#### Basic usage
```csharp
@inject LoaderService LoaderService

private async Task LoadData()
{
    LoaderService.Show();
    try
    {
        var data = await ApiService.GetDataAsync();
        // Process data...
    }
    finally
    {
        LoaderService.Close();
    }
}
```

#### Managing multiple operations
```csharp
// Start 3 parallel operations
LoaderService.Increment(); // Counter = 1 ? Loader displayed
LoaderService.Increment(); // Counter = 2 ? Loader remains displayed
LoaderService.Increment(); // Counter = 3 ? Loader remains displayed

// Finish operations one by one
LoaderService.Decrement(); // Counter = 2 ? Loader remains displayed
LoaderService.Decrement(); // Counter = 1 ? Loader remains displayed
LoaderService.Decrement(); // Counter = 0 ? Loader hides
```

#### Error handling with Reset
```csharp
try
{
    LoaderService.Show();
    await RiskyOperation();
}
catch (Exception ex)
{
    LoaderService.Reset(); // Force closure on error
    // Error handling...
}
```

#### Usage in a service
```csharp
public class DataService
{
    private readonly LoaderService _loaderService;
    private readonly HttpClient _httpClient;

    public async Task<List<User>> GetUsersAsync()
    {
        _loaderService.Show();
        try
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("api/users");
        }
        finally
        {
            _loaderService.Close();
        }
    }
}
```

### Best practices

1. **Always use try/finally** to ensure loader closure
2. **Prefer Show()/Close()** for simple cases
3. **Use Increment()/Decrement()** for complex operations
4. **Reset() only in emergencies** (critical errors)