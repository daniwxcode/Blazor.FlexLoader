namespace Blazor.FlexLoader.Services;

public class LoaderService
{
    public event EventHandler? OnChange;
    private int _requestCount;

    public bool IsLoading => _requestCount > 0;

    public void Increment()
    {
        Interlocked.Increment(ref _requestCount);
        OnChange?.Invoke(this, EventArgs.Empty);
    }

    public void Decrement()
    {
        if (_requestCount > 0)
        {
            Interlocked.Decrement(ref _requestCount);
            OnChange?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _requestCount, 0);
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
