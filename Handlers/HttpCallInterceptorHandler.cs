using Blazor.FlexLoader.Services;

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
    public class HttpCallInterceptorHandler(LoaderService loaderService) : DelegatingHandler
    {
   private const int MaxRetryAttempts = 3;

     /// <summary>
        /// Intercepte et exécute les requêtes HTTP avec gestion du loader et retry automatique.
        /// Le loader s'affiche automatiquement pendant toute la durée de la requête.
        /// En cas d'erreur, la requête est réessayée jusqu'à 3 fois avant de retourner une erreur.
      /// </summary>
        /// <param name="request">La requête HTTP à envoyer.</param>
        /// <param name="cancellationToken">Token d'annulation de l'opération.</param>
        /// <returns>La réponse HTTP reçue du serveur.</returns>
   /// <remarks>
        /// Intercepts and executes HTTP requests with loader management and automatic retry.
 /// The loader displays automatically for the entire duration of the request.
        /// On error, the request is retried up to 3 times before returning an error.
      /// </remarks>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            loaderService.Increment();
 try
   {
                return await ExecuteWithRetryAsync(request, cancellationToken);
        }
            finally
            {
            loaderService.Decrement();
       }
  }

      /// <summary>
        /// Exécute la requête avec une logique de retry en cas d'erreur.
        /// Réessaie automatiquement la requête en cas de <see cref="HttpRequestException"/> 
        /// ou de réponse <see cref="System.Net.HttpStatusCode.InternalServerError"/> (HTTP 500).
  /// </summary>
        /// <param name="request">La requête HTTP à exécuter.</param>
        /// <param name="cancellationToken">Token d'annulation de l'opération.</param>
        /// <returns>La réponse HTTP réussie ou une réponse d'erreur après épuisement des tentatives.</returns>
        /// <remarks>
        /// Executes the request with retry logic on error.
        /// Automatically retries the request on <see cref="HttpRequestException"/> 
/// or <see cref="System.Net.HttpStatusCode.InternalServerError"/> (HTTP 500) response.
        /// </remarks>
        private async Task<HttpResponseMessage> ExecuteWithRetryAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
   HttpResponseMessage? response = null;
            Exception? lastException = null;

     for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
   {
      try
             {
                    // Clone la requête pour les tentatives suivantes si nécessaire
            var requestToSend = attempt == 0 ? request : await CloneHttpRequestMessageAsync(request);

           response = await base.SendAsync(requestToSend, cancellationToken);

     // Si la réponse n'est pas une erreur serveur, on retourne
   if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
      {
   return response;
   }

   // Dispose la réponse d'erreur avant de réessayer
  response.Dispose();
  }
       catch (HttpRequestException ex)
     {
          lastException = ex;

              // Si c'est la dernière tentative, on va sortir de la boucle
    if (attempt == MaxRetryAttempts - 1)
          {
       break;
            }
          }
            }

  // Toutes les tentatives ont échoué
            return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
 {
                RequestMessage = request,
   ReasonPhrase = lastException?.Message ?? "Max retry attempts reached"
            };
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

  return clone;
     }
    }
}
