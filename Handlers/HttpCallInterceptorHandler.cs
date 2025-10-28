using Blazor.FlexLoader.Services;
using Blazor.FlexLoader.Options;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Blazor.FlexLoader.Handlers
{
    /// <summary>
    /// Handler HTTP qui intercepte les requêtes pour afficher un indicateur de chargement
    /// et implémenter une logique de retry en cas d'erreur.
    /// </summary>
    /// <remarks>
    /// HTTP handler that intercepts requests to display a loading indicator
    /// and implements retry logic on errors.
    /// </remarks>
    public class HttpCallInterceptorHandler : DelegatingHandler
    {
    private readonly LoaderService _loaderService;
        private readonly HttpInterceptorOptions _options;
   private readonly ILogger<HttpCallInterceptorHandler>? _logger;

        /// <summary>
        /// Initialise une nouvelle instance du handler avec les options par défaut.
        /// </summary>
        /// <param name="loaderService">Service de gestion du loader.</param>
        /// <param name="logger">Logger optionnel pour la traçabilité.</param>
        public HttpCallInterceptorHandler(
      LoaderService loaderService,
            ILogger<HttpCallInterceptorHandler>? logger = null)
            : this(loaderService, new HttpInterceptorOptions(), logger)
 {
        }

        /// <summary>
        /// Initialise une nouvelle instance du handler avec des options personnalisées.
        /// </summary>
        /// <param name="loaderService">Service de gestion du loader.</param>
        /// <param name="options">Options de configuration de l'intercepteur.</param>
   /// <param name="logger">Logger optionnel pour la traçabilité.</param>
     public HttpCallInterceptorHandler(
            LoaderService loaderService,
   HttpInterceptorOptions options,
      ILogger<HttpCallInterceptorHandler>? logger = null)
    {
       _loaderService = loaderService ?? throw new ArgumentNullException(nameof(loaderService));
     _options = options ?? throw new ArgumentNullException(nameof(options));
      _logger = logger;
     }

  /// <summary>
        /// Intercepte et exécute les requêtes HTTP avec gestion du loader et retry automatique.
        /// Le loader s'affiche automatiquement pendant toute la durée de la requête.
 /// En cas d'erreur, la requête est réessayée selon la configuration.
        /// </summary>
        /// <param name="request">La requête HTTP à envoyer.</param>
        /// <param name="cancellationToken">Token d'annulation de l'opération.</param>
      /// <returns>La réponse HTTP reçue du serveur.</returns>
  /// <remarks>
        /// Intercepts and executes HTTP requests with loader management and automatic retry.
  /// The loader displays automatically for the entire duration of the request.
    /// On error, the request is retried according to configuration.
        /// </remarks>
   protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Vérifie si la requête doit être interceptée
  if (_options.InterceptPredicate != null && !_options.InterceptPredicate(request))
        {
    _logger?.LogDebug("Request to {Url} skipped by InterceptPredicate", request.RequestUri);
  return await base.SendAsync(request, cancellationToken);
          }

       // Vérifie si le loader doit être affiché
      var showLoader = _options.ShowLoaderPredicate?.Invoke(request) ?? true;

        if (showLoader)
    {
       _loaderService.Increment();
            }

      var stopwatch = Stopwatch.StartNew();

     try
    {
         _logger?.LogInformation("Starting request to {Url} (Method: {Method})",
     request.RequestUri, request.Method);

        // Combine le CancellationToken global avec celui de la requête
           using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
    cancellationToken, 
              _loaderService.GlobalCancellationToken);

 var response = await ExecuteWithRetryAsync(request, linkedCts.Token);

   stopwatch.Stop();

      // Enregistre les métriques
    if (response.IsSuccessStatusCode)
            {
  _loaderService.Metrics.RecordSuccess((int)response.StatusCode, stopwatch.Elapsed);
      }
     else
    {
    _loaderService.Metrics.RecordFailure((int)response.StatusCode, stopwatch.Elapsed);
    }

     _logger?.LogInformation("Request to {Url} completed in {Duration}ms with status {StatusCode}",
    request.RequestUri, stopwatch.ElapsedMilliseconds, response.StatusCode);

         return response;
     }
            catch (OperationCanceledException) when (_loaderService.GlobalCancellationToken.IsCancellationRequested)
        {
      stopwatch.Stop();
         _logger?.LogWarning("Request to {Url} was cancelled globally", request.RequestUri);
        throw;
  }
      catch (Exception ex)
{
      stopwatch.Stop();
        _loaderService.Metrics.RecordFailure(500, stopwatch.Elapsed);
      
     _logger?.LogError(ex, "Request to {Url} failed after {Duration}ms",
  request.RequestUri, stopwatch.ElapsedMilliseconds);
              throw;
          }
            finally
            {
    if (showLoader)
    {
   _loaderService.Decrement();
          }
    }
        }

        /// <summary>
        /// Exécute la requête avec une logique de retry en cas d'erreur.
        /// Réessaie automatiquement la requête selon la configuration des options.
        /// </summary>
        /// <param name="request">La requête HTTP à exécuter.</param>
   /// <param name="cancellationToken">Token d'annulation de l'opération.</param>
        /// <returns>La réponse HTTP réussie ou une réponse d'erreur après épuisement des tentatives.</returns>
        private async Task<HttpResponseMessage> ExecuteWithRetryAsync(
       HttpRequestMessage request,
      CancellationToken cancellationToken)
   {
            HttpResponseMessage? response = null;
      Exception? lastException = null;
         bool hasRetried = false;

            for (int attempt = 0; attempt < _options.MaxRetryAttempts; attempt++)
            {
   try
                {
    // Clone la requête pour les tentatives suivantes si nécessaire
  var requestToSend = attempt == 0 ? request : await CloneHttpRequestMessageAsync(request);

           response = await base.SendAsync(requestToSend, cancellationToken);

                 // Vérifie si le code de statut nécessite un retry
  if (!_options.RetryOnStatusCodes.Contains(response.StatusCode))
     {
 return response;
            }

                    _logger?.LogWarning("Request to {Url} returned {StatusCode}, attempt {Attempt}/{MaxAttempts}",
       request.RequestUri, response.StatusCode, attempt + 1, _options.MaxRetryAttempts);

            // Dispose la réponse d'erreur avant de réessayer
            response.Dispose();

         // Si c'est la dernière tentative, on sort de la boucle
                  if (attempt == _options.MaxRetryAttempts - 1)
        {
    break;
            }

             // Enregistre le retry dans les métriques
         _loaderService.Metrics.RecordRetry(!hasRetried);
  hasRetried = true;

    // Attend avant de réessayer
         await DelayBeforeRetryAsync(attempt, null, cancellationToken);
      }
         catch (HttpRequestException ex) when (_options.RetryOnTimeout)
  {
       lastException = ex;

      _logger?.LogWarning(ex, "Request to {Url} failed with HttpRequestException, attempt {Attempt}/{MaxAttempts}",
         request.RequestUri, attempt + 1, _options.MaxRetryAttempts);

 // Si c'est la dernière tentative, on va sortir de la boucle
      if (attempt == _options.MaxRetryAttempts - 1)
{
     break;
      }

// Enregistre le retry dans les métriques
           _loaderService.Metrics.RecordRetry(!hasRetried);
   hasRetried = true;

                // Attend avant de réessayer
              await DelayBeforeRetryAsync(attempt, ex, cancellationToken);
     }
 catch (TaskCanceledException ex) when (_options.RetryOnTimeout && !cancellationToken.IsCancellationRequested)
    {
       lastException = ex;

           _logger?.LogWarning(ex, "Request to {Url} timed out, attempt {Attempt}/{MaxAttempts}",
                request.RequestUri, attempt + 1, _options.MaxRetryAttempts);

           // Si c'est la dernière tentative, on va sortir de la boucle
      if (attempt == _options.MaxRetryAttempts - 1)
 {
    break;
           }

   // Enregistre le retry dans les métriques
              _loaderService.Metrics.RecordRetry(!hasRetried);
    hasRetried = true;

          // Attend avant de réessayer
     await DelayBeforeRetryAsync(attempt, ex, cancellationToken);
       }
       }

         // Toutes les tentatives ont échoué
        _logger?.LogError("All retry attempts exhausted for request to {Url}", request.RequestUri);

            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
         {
                RequestMessage = request,
        ReasonPhrase = lastException?.Message ?? "Max retry attempts reached"
            };
 }

        /// <summary>
  /// Calcule et applique le délai avant la prochaine tentative de retry.
   /// Utilise l'exponential backoff si configuré.
      /// </summary>
   /// <param name="attemptNumber">Numéro de la tentative (0-based).</param>
      /// <param name="exception">Exception ayant causé le retry.</param>
        /// <param name="cancellationToken">Token d'annulation.</param>
        private async Task DelayBeforeRetryAsync(int attemptNumber, Exception? exception, CancellationToken cancellationToken)
        {
          var delay = _options.UseExponentialBackoff
       ? TimeSpan.FromMilliseconds(_options.RetryDelay.TotalMilliseconds * Math.Pow(2, attemptNumber))
          : _options.RetryDelay;

         _logger?.LogDebug("Waiting {Delay}ms before retry attempt {Attempt}",
          delay.TotalMilliseconds, attemptNumber + 2);

   // Appelle le callback OnRetry si défini
            if (_options.OnRetry != null)
        {
     await _options.OnRetry(attemptNumber + 1, exception, delay);
       }

            await Task.Delay(delay, cancellationToken);
        }

        /// <summary>
        /// Clone une requête HTTP pour permettre les tentatives multiples.
        /// Nécessaire car une <see cref="HttpRequestMessage"/> ne peut être envoyée qu'une seule fois.
  /// Clone le contenu, les headers et les propriétés de la requête originale.
        /// </summary>
        /// <param name="request">La requête HTTP à cloner.</param>
        /// <returns>Une nouvelle instance de <see cref="HttpRequestMessage"/> identique à l'originale.</returns>
/// <remarks>
        /// Clones an HTTP request to allow multiple attempts.
    /// Required because an <see cref="HttpRequestMessage"/> can only be sent once.
        /// Clones the content, headers, and properties of the original request.
        /// </remarks>
      private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
   var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
        Version = request.Version
  };

      // Copie le contenu si présent
   if (request.Content != null)
       {
    var contentBytes = await request.Content.ReadAsByteArrayAsync();
      clone.Content = new ByteArrayContent(contentBytes);

     // Copie les headers du contenu
     foreach (var header in request.Content.Headers)
          {
   clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
    }
            }

            // Copie les headers de la requête
        foreach (var header in request.Headers)
      {
   clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

     // Copie les propriétés
         foreach (var property in request.Options)
      {
      clone.Options.TryAdd(property.Key, property.Value);
            }

            if (_options.EnableDetailedLogging)
      {
 _logger?.LogDebug("Cloned request for {Url}", request.RequestUri);
            }

            return clone;
        }
    }
}
