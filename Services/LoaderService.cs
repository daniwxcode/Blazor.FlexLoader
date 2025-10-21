namespace Blazor.FlexLoader.Services;

public class LoaderService
{
    public event EventHandler? OnChange;
    private int _requestCount;

    public bool IsLoading => _requestCount > 0;

    /// <summary>
    /// Affiche le loader en incrémentant le compteur
    /// </summary>
    public void Show()
    {
        Increment();
    }

    /// <summary>
    /// Masque le loader en décrémentant le compteur
    /// </summary>
    public void Close()
    {
        Decrement();
    }

    /// <summary>
    /// Incrémente le compteur de requêtes de chargement
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment(ref _requestCount);
        OnChange?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Décrémente le compteur de requêtes de chargement
    /// </summary>
    public void Decrement()
    {
        if (_requestCount > 0)
        {
            Interlocked.Decrement(ref _requestCount);
            OnChange?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Remet à zéro le compteur de requêtes (force la fermeture)
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _requestCount, 0);
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
