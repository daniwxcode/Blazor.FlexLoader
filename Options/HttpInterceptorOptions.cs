using System.Net;

namespace Blazor.FlexLoader.Options;

/// <summary>
/// Options de configuration pour l'intercepteur HTTP.
/// </summary>
/// <remarks>
/// Configuration options for the HTTP interceptor.
/// </remarks>
public class HttpInterceptorOptions
{
    /// <summary>
    /// Nombre maximum de tentatives de retry.
    /// Valeur par défaut : 3.
    /// </summary>
    /// <remarks>
    /// Maximum number of retry attempts.
    /// Default value: 3.
  /// </remarks>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Délai de base entre les tentatives de retry.
    /// Utilisé avec la stratégie de retry pour calculer le délai réel.
    /// Valeur par défaut : 1 seconde.
    /// </summary>
    /// <remarks>
    /// Base delay between retry attempts.
    /// Used with retry strategy to calculate actual delay.
    /// Default value: 1 second.
    /// </remarks>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Indique si l'exponential backoff est activé.
    /// Si true, le délai augmente exponentiellement : 1s, 2s, 4s, 8s...
    /// Si false, le délai est constant.
    /// Valeur par défaut : true.
    /// </summary>
    /// <remarks>
    /// Indicates whether exponential backoff is enabled.
    /// If true, delay increases exponentially: 1s, 2s, 4s, 8s...
    /// If false, delay is constant.
    /// Default value: true.
    /// </remarks>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Codes de statut HTTP qui déclenchent un retry.
    /// Valeur par défaut : [500, 502, 503, 504, 408].
    /// </summary>
    /// <remarks>
    /// HTTP status codes that trigger a retry.
    /// Default value: [500, 502, 503, 504, 408].
    /// </remarks>
    public HashSet<HttpStatusCode> RetryOnStatusCodes { get; set; } = new()
    {
        HttpStatusCode.InternalServerError,   // 500
        HttpStatusCode.BadGateway,    // 502
     HttpStatusCode.ServiceUnavailable,// 503
        HttpStatusCode.GatewayTimeout,         // 504
        HttpStatusCode.RequestTimeout            // 408
    };

    /// <summary>
  /// Indique si le retry doit être activé en cas de timeout.
    /// Valeur par défaut : true.
    /// </summary>
    /// <remarks>
    /// Indicates whether retry should be enabled on timeout.
    /// Default value: true.
    /// </remarks>
    public bool RetryOnTimeout { get; set; } = true;

    /// <summary>
    /// Prédicat pour déterminer si une requête doit être interceptée.
    /// Si null, toutes les requêtes sont interceptées.
    /// </summary>
    /// <example>
    /// <code>
    /// options.InterceptPredicate = request => 
    ///     request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    /// </code>
/// </example>
    /// <remarks>
    /// Predicate to determine if a request should be intercepted.
    /// If null, all requests are intercepted.
    /// </remarks>
 public Func<HttpRequestMessage, bool>? InterceptPredicate { get; set; }

    /// <summary>
  /// Prédicat pour déterminer si le loader doit être affiché pour une requête.
    /// Si null, le loader est affiché pour toutes les requêtes interceptées.
    /// </summary>
    /// <example>
    /// <code>
    /// options.ShowLoaderPredicate = request => 
    ///     request.Method != HttpMethod.Get;
    /// </code>
    /// </example>
    /// <remarks>
    /// Predicate to determine if the loader should be shown for a request.
    /// If null, loader is shown for all intercepted requests.
    /// </remarks>
    public Func<HttpRequestMessage, bool>? ShowLoaderPredicate { get; set; }

    /// <summary>
    /// Callback appelé avant chaque tentative de retry.
    /// Permet d'exécuter une logique personnalisée (notification, log, etc.).
    /// </summary>
    /// <example>
    /// <code>
    /// options.OnRetry = async (attempt, exception, delay) =>
    /// {
 ///     await NotificationService.ShowWarning($"Tentative {attempt}...");
    /// };
    /// </code>
    /// </example>
  /// <remarks>
    /// Callback invoked before each retry attempt.
    /// Allows executing custom logic (notification, logging, etc.).
    /// </remarks>
    public Func<int, Exception?, TimeSpan, Task>? OnRetry { get; set; }

    /// <summary>
    /// Indique si le logging détaillé est activé.
    /// Valeur par défaut : false.
    /// </summary>
    /// <remarks>
    /// Indicates whether detailed logging is enabled.
    /// Default value: false.
    /// </remarks>
    public bool EnableDetailedLogging { get; set; } = false;
}
