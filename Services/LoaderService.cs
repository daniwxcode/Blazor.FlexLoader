namespace Blazor.FlexLoader.Services;

/// <summary>
/// Service de gestion d'indicateurs de chargement pour applications Blazor.
/// Utilise un système de compteur thread-safe pour gérer plusieurs opérations simultanées.
/// </summary>
/// <remarks>
/// Loading indicator management service for Blazor applications.
/// Uses a thread-safe counter system to handle multiple simultaneous operations.
/// </remarks>
public class LoaderService
{
    /// <summary>
    /// Événement déclenché lors du changement d'état du loader.
    /// </summary>
    /// <remarks>
    /// Event triggered when the loader state changes.
    /// </remarks>
    public event EventHandler? OnChange;
    
    private int _requestCount;

    /// <summary>
    /// Indique si le loader est actuellement affiché.
    /// Retourne true si au moins une requête de chargement est en cours.
    /// </summary>
    /// <value>
    /// true si le loader est visible, false sinon.
    /// </value>
    /// <remarks>
    /// Indicates whether the loader is currently displayed.
    /// Returns true if at least one loading request is in progress.
    /// </remarks>
    public bool IsLoading => _requestCount > 0;

    /// <summary>
    /// Affiche le loader en incrémentant le compteur de requêtes.
    /// Méthode simplifiée équivalente à <see cref="Increment"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Français
    /// LoaderService.Show();
    /// try 
    /// {
    ///     await OperationAsync();
    /// }
    /// finally 
    /// {
    ///     LoaderService.Close();
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// Displays the loader by incrementing the request counter.
    /// Simplified method equivalent to <see cref="Increment"/>.
    /// </remarks>
    public void Show()
    {
        Increment();
    }

    /// <summary>
    /// Masque le loader en décrémentant le compteur de requêtes.
    /// Méthode simplifiée équivalente à <see cref="Decrement"/>.
    /// Le loader ne se ferme que lorsque toutes les requêtes sont terminées (compteur = 0).
    /// </summary>
    /// <remarks>
    /// Hides the loader by decrementing the request counter.
    /// Simplified method equivalent to <see cref="Decrement"/>.
    /// The loader only closes when all requests are finished (counter = 0).
    /// </remarks>
    public void Close()
    {
        Decrement();
    }

    /// <summary>
    /// Incrémente le compteur de requêtes de chargement de manière thread-safe.
    /// Chaque appel augmente le compteur de 1. Le loader reste affiché tant que le compteur > 0.
    /// </summary>
    /// <example>
    /// <code>
    /// // Français - Gestion de plusieurs opérations
    /// LoaderService.Increment(); // Compteur = 1, loader affiché
    /// LoaderService.Increment(); // Compteur = 2, loader toujours affiché
    /// LoaderService.Decrement(); // Compteur = 1, loader toujours affiché
    /// LoaderService.Decrement(); // Compteur = 0, loader masqué
    /// </code>
    /// </example>
    /// <remarks>
    /// Increments the loading request counter in a thread-safe manner.
    /// Each call increases the counter by 1. The loader remains displayed as long as counter > 0.
    /// </remarks>
    public void Increment()
    {
        Interlocked.Increment(ref _requestCount);
        OnChange?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Décrémente le compteur de requêtes de chargement de manière thread-safe.
    /// Le compteur ne peut pas descendre en dessous de 0.
    /// Déclenche l'événement <see cref="OnChange"/> pour notifier les composants du changement d'état.
    /// </summary>
    /// <remarks>
    /// Decrements the loading request counter in a thread-safe manner.
    /// The counter cannot go below 0.
    /// Triggers the <see cref="OnChange"/> event to notify components of state changes.
    /// </remarks>
    public void Decrement()
    {
        if (_requestCount > 0)
        {
            Interlocked.Decrement(ref _requestCount);
            OnChange?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Force la fermeture immédiate du loader en remettant le compteur à zéro.
    /// Utilise cette méthode en cas d'erreur critique ou pour réinitialiser l'état.
    /// ⚠️ Attention : Cette méthode ignore toutes les requêtes en cours.
    /// </summary>
    /// <example>
    /// <code>
    /// // Français - En cas d'erreur critique
    /// try 
    /// {
    ///     await MultipleOperationsAsync();
    /// }
    /// catch (CriticalException ex)
    /// {
    ///     LoaderService.Reset(); // Force la fermeture immédiate
    ///     // Gestion d'erreur...
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// Forces immediate closure of the loader by resetting the counter to zero.
    /// Use this method in case of critical error or to reset the state.
    /// ⚠️ Warning: This method ignores all pending requests.
    /// </remarks>
    public void Reset()
    {
        Interlocked.Exchange(ref _requestCount, 0);
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
