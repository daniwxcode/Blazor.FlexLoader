using System.Collections.Concurrent;

namespace Blazor.FlexLoader.Models;

/// <summary>
/// Métriques et statistiques en temps réel du LoaderService.
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
    /// Nombre total de requêtes effectuées.
  /// </summary>
    /// <remarks>
    /// Total number of requests made.
    /// </remarks>
    public int TotalRequests { get; private set; }

 /// <summary>
    /// Nombre de requêtes réussies (codes 2xx).
    /// </summary>
    /// <remarks>
    /// Number of successful requests (2xx codes).
    /// </remarks>
    public int SuccessfulRequests { get; private set; }

    /// <summary>
    /// Nombre de requêtes échouées (codes 4xx et 5xx).
    /// </summary>
    /// <remarks>
    /// Number of failed requests (4xx and 5xx codes).
    /// </remarks>
    public int FailedRequests { get; private set; }

    /// <summary>
    /// Nombre de requêtes ayant nécessité un retry.
    /// </summary>
    /// <remarks>
    /// Number of requests that required a retry.
    /// </remarks>
    public int RetriedRequests { get; private set; }

    /// <summary>
/// Nombre total de tentatives de retry effectuées.
    /// </summary>
    /// <remarks>
    /// Total number of retry attempts made.
    /// </remarks>
    public int TotalRetryAttempts { get; private set; }

    /// <summary>
    /// Temps de réponse moyen.
    /// </summary>
    /// <remarks>
    /// Average response time.
    /// </remarks>
    public TimeSpan AverageResponseTime { get; private set; }

    /// <summary>
  /// Temps de réponse maximum observé.
    /// </summary>
    /// <remarks>
    /// Maximum response time observed.
    /// </remarks>
    public TimeSpan MaxResponseTime { get; private set; }

    /// <summary>
    /// Temps de réponse minimum observé.
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
    /// Date et heure de la dernière requête.
  /// </summary>
    /// <remarks>
  /// Date and time of the last request.
    /// </remarks>
    public DateTime? LastRequestTime { get; private set; }

    /// <summary>
    /// Taux de réussite en pourcentage.
    /// </summary>
    /// <remarks>
    /// Success rate as a percentage.
    /// </remarks>
    public double SuccessRate => TotalRequests > 0 
        ? (double)SuccessfulRequests / TotalRequests * 100 
  : 0;

    /// <summary>
    /// Taux d'échec en pourcentage.
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
    /// Nombre moyen de retries par requête nécessitant un retry.
    /// </summary>
    /// <remarks>
    /// Average number of retries per request requiring retry.
    /// </remarks>
    public double AverageRetriesPerFailedRequest => RetriedRequests > 0 
 ? (double)TotalRetryAttempts / RetriedRequests 
        : 0;

    /// <summary>
    /// Enregistre une requête réussie.
    /// </summary>
    /// <param name="statusCode">Code de statut HTTP.</param>
    /// <param name="responseTime">Temps de réponse.</param>
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
    /// Enregistre une requête échouée.
    /// </summary>
    /// <param name="statusCode">Code de statut HTTP.</param>
    /// <param name="responseTime">Temps de réponse.</param>
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
    /// <param name="isFirstRetry">Indique si c'est le premier retry pour cette requête.</param>
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
    /// Réinitialise toutes les métriques.
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
  /// Obtient un résumé formaté des métriques.
    /// </summary>
    /// <returns>Chaîne formatée avec les métriques principales.</returns>
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
        
        // Limite le nombre d'échantillons pour éviter une consommation mémoire excessive
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
