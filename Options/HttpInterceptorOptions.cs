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
    /// Valeur par d�faut : 3.
    /// </summary>
    /// <remarks>
    /// Maximum number of retry attempts.
    /// Default value: 3.
  /// </remarks>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// D�lai de base entre les tentatives de retry.
    /// Utilis� avec la strat�gie de retry pour calculer le d�lai r�el.
    /// Valeur par d�faut : 1 seconde.
    /// </summary>
    /// <remarks>
    /// Base delay between retry attempts.
    /// Used with retry strategy to calculate actual delay.
    /// Default value: 1 second.
    /// </remarks>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Indique si l'exponential backoff est activ�.
    /// Si true, le d�lai augmente exponentiellement : 1s, 2s, 4s, 8s...
    /// Si false, le d�lai est constant.
    /// Valeur par d�faut : true.
    /// </summary>
    /// <remarks>
    /// Indicates whether exponential backoff is enabled.
    /// If true, delay increases exponentially: 1s, 2s, 4s, 8s...
    /// If false, delay is constant.
    /// Default value: true.
    /// </remarks>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Codes de statut HTTP qui d�clenchent un retry.
    /// Valeur par d�faut : [500, 502, 503, 504, 408].
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
  /// Indique si le retry doit �tre activ� en cas de timeout.
    /// Valeur par d�faut : true.
    /// </summary>
    /// <remarks>
    /// Indicates whether retry should be enabled on timeout.
    /// Default value: true.
    /// </remarks>
    public bool RetryOnTimeout { get; set; } = true;

    /// <summary>
    /// Pr�dicat pour d�terminer si une requ�te doit �tre intercept�e.
    /// Si null, toutes les requ�tes sont intercept�es.
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
  /// Pr�dicat pour d�terminer si le loader doit �tre affich� pour une requ�te.
    /// Si null, le loader est affich� pour toutes les requ�tes intercept�es.
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
    /// Callback appel� avant chaque tentative de retry.
    /// Permet d'ex�cuter une logique personnalis�e (notification, log, etc.).
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
    /// Indique si le logging d�taill� est activ�.
    /// Valeur par d�faut : false.
    /// </summary>
    /// <remarks>
    /// Indicates whether detailed logging is enabled.
    /// Default value: false.
    /// </remarks>
    public bool EnableDetailedLogging { get; set; } = false;
}
