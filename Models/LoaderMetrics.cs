using System.Collections.Concurrent;

namespace Blazor.FlexLoader.Models;

/// <summary>
/// M�triques et statistiques en temps r�el du LoaderService.
/// </summary>
/// <remarks>
/// Real-time metrics and statistics for the LoaderService.
/// </remarks>
public class LoaderMetrics
{
    private readonly ConcurrentDictionary<int, int> _statusCodeDistribution = new();
    private readonly ConcurrentQueue<TimeSpan> _responseTimes = new();
  private readonly object _lock = new();
    private const int MaxResponseTimeSamples = 100;

    /// <summary>
    /// Nombre total de requ�tes effectu�es.
  /// </summary>
    /// <remarks>
    /// Total number of requests made.
    /// </remarks>
    public int TotalRequests { get; private set; }

 /// <summary>
    /// Nombre de requ�tes r�ussies (codes 2xx).
    /// </summary>
    /// <remarks>
    /// Number of successful requests (2xx codes).
    /// </remarks>
    public int SuccessfulRequests { get; private set; }

    /// <summary>
    /// Nombre de requ�tes �chou�es (codes 4xx et 5xx).
    /// </summary>
    /// <remarks>
    /// Number of failed requests (4xx and 5xx codes).
    /// </remarks>
    public int FailedRequests { get; private set; }

    /// <summary>
    /// Nombre de requ�tes ayant n�cessit� un retry.
    /// </summary>
    /// <remarks>
    /// Number of requests that required a retry.
    /// </remarks>
    public int RetriedRequests { get; private set; }

    /// <summary>
/// Nombre total de tentatives de retry effectu�es.
    /// </summary>
    /// <remarks>
    /// Total number of retry attempts made.
    /// </remarks>
    public int TotalRetryAttempts { get; private set; }

    /// <summary>
    /// Temps de r�ponse moyen.
    /// </summary>
    /// <remarks>
    /// Average response time.
    /// </remarks>
    public TimeSpan AverageResponseTime { get; private set; }

    /// <summary>
  /// Temps de r�ponse maximum observ�.
    /// </summary>
    /// <remarks>
    /// Maximum response time observed.
    /// </remarks>
    public TimeSpan MaxResponseTime { get; private set; }

    /// <summary>
    /// Temps de r�ponse minimum observ�.
    /// </summary>
    /// <remarks>
    /// Minimum response time observed.
    /// </remarks>
    public TimeSpan MinResponseTime { get; private set; } = TimeSpan.MaxValue;

    /// <summary>
  /// Distribution des codes de statut HTTP.
    /// </summary>
    /// <remarks>
    /// Distribution of HTTP status codes.
    /// </remarks>
    public IReadOnlyDictionary<int, int> StatusCodeDistribution => _statusCodeDistribution;

    /// <summary>
    /// Date et heure de la derni�re requ�te.
  /// </summary>
    /// <remarks>
  /// Date and time of the last request.
    /// </remarks>
    public DateTime? LastRequestTime { get; private set; }

    /// <summary>
    /// Taux de r�ussite en pourcentage.
    /// </summary>
    /// <remarks>
    /// Success rate as a percentage.
    /// </remarks>
    public double SuccessRate => TotalRequests > 0 
        ? (double)SuccessfulRequests / TotalRequests * 100 
  : 0;

    /// <summary>
    /// Taux d'�chec en pourcentage.
    /// </summary>
    /// <remarks>
    /// Failure rate as a percentage.
    /// </remarks>
    public double FailureRate => TotalRequests > 0 
 ? (double)FailedRequests / TotalRequests * 100 
        : 0;

    /// <summary>
  /// Taux de retry en pourcentage.
  /// </summary>
    /// <remarks>
  /// Retry rate as a percentage.
    /// </remarks>
    public double RetryRate => TotalRequests > 0 
     ? (double)RetriedRequests / TotalRequests * 100 
        : 0;

    /// <summary>
    /// Nombre moyen de retries par requ�te n�cessitant un retry.
    /// </summary>
    /// <remarks>
    /// Average number of retries per request requiring retry.
    /// </remarks>
    public double AverageRetriesPerFailedRequest => RetriedRequests > 0 
 ? (double)TotalRetryAttempts / RetriedRequests 
        : 0;

    /// <summary>
    /// Enregistre une requ�te r�ussie.
    /// </summary>
    /// <param name="statusCode">Code de statut HTTP.</param>
    /// <param name="responseTime">Temps de r�ponse.</param>
    /// <remarks>
    /// Records a successful request.
    /// </remarks>
    public void RecordSuccess(int statusCode, TimeSpan responseTime)
    {
        lock (_lock)
        {
  TotalRequests++;
            SuccessfulRequests++;
   LastRequestTime = DateTime.UtcNow;
            
 RecordStatusCode(statusCode);
       RecordResponseTime(responseTime);
      }
    }

    /// <summary>
    /// Enregistre une requ�te �chou�e.
    /// </summary>
    /// <param name="statusCode">Code de statut HTTP.</param>
    /// <param name="responseTime">Temps de r�ponse.</param>
 /// <remarks>
    /// Records a failed request.
    /// </remarks>
    public void RecordFailure(int statusCode, TimeSpan responseTime)
    {
      lock (_lock)
        {
      TotalRequests++;
            FailedRequests++;
    LastRequestTime = DateTime.UtcNow;
      
            RecordStatusCode(statusCode);
    RecordResponseTime(responseTime);
        }
    }

    /// <summary>
    /// Enregistre une tentative de retry.
    /// </summary>
    /// <param name="isFirstRetry">Indique si c'est le premier retry pour cette requ�te.</param>
    /// <remarks>
    /// Records a retry attempt.
    /// </remarks>
    public void RecordRetry(bool isFirstRetry)
    {
        lock (_lock)
        {
            TotalRetryAttempts++;
    if (isFirstRetry)
{
                RetriedRequests++;
      }
        }
    }

    /// <summary>
    /// R�initialise toutes les m�triques.
    /// </summary>
    /// <remarks>
    /// Resets all metrics.
    /// </remarks>
    public void Reset()
    {
      lock (_lock)
{
    TotalRequests = 0;
            SuccessfulRequests = 0;
       FailedRequests = 0;
 RetriedRequests = 0;
            TotalRetryAttempts = 0;
   AverageResponseTime = TimeSpan.Zero;
            MaxResponseTime = TimeSpan.Zero;
  MinResponseTime = TimeSpan.MaxValue;
         LastRequestTime = null;
            
        _statusCodeDistribution.Clear();
       _responseTimes.Clear();
        }
    }

    /// <summary>
  /// Obtient un r�sum� format� des m�triques.
    /// </summary>
    /// <returns>Cha�ne format�e avec les m�triques principales.</returns>
    /// <remarks>
    /// Gets a formatted summary of the metrics.
    /// </remarks>
    public string GetSummary()
    {
        return $@"Blazor.FlexLoader Metrics:
Total Requests: {TotalRequests}
Success Rate: {SuccessRate:F2}%
Failure Rate: {FailureRate:F2}%
Retry Rate: {RetryRate:F2}%
Avg Response Time: {AverageResponseTime.TotalMilliseconds:F2}ms
Max Response Time: {MaxResponseTime.TotalMilliseconds:F2}ms
Min Response Time: {(MinResponseTime == TimeSpan.MaxValue ? 0 : MinResponseTime.TotalMilliseconds):F2}ms
Total Retries: {TotalRetryAttempts}
Last Request: {LastRequestTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}";
    }

    private void RecordStatusCode(int statusCode)
    {
_statusCodeDistribution.AddOrUpdate(statusCode, 1, (_, count) => count + 1);
    }

    private void RecordResponseTime(TimeSpan responseTime)
    {
        _responseTimes.Enqueue(responseTime);
        
        // Limite le nombre d'�chantillons pour �viter une consommation m�moire excessive
        while (_responseTimes.Count > MaxResponseTimeSamples)
     {
            _responseTimes.TryDequeue(out _);
        }

    // Calcul des statistiques
        if (_responseTimes.Any())
        {
          var times = _responseTimes.ToArray();
 AverageResponseTime = TimeSpan.FromMilliseconds(times.Average(t => t.TotalMilliseconds));
     MaxResponseTime = times.Max();
            MinResponseTime = times.Min();
        }
    }
}
