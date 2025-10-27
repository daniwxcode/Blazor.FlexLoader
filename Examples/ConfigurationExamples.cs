using Blazor.FlexLoader.Extensions;
using Blazor.FlexLoader.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Blazor.FlexLoader.Examples;

/// <summary>
/// Exemples de configuration de Blazor.FlexLoader avec différents scénarios d'utilisation.
/// </summary>
/// <remarks>
/// Configuration examples for Blazor.FlexLoader with various usage scenarios.
/// </remarks>
public static class ConfigurationExamples
{
    /// <summary>
    /// Configuration minimale - Utilise tous les paramètres par défaut.
    /// </summary>
    public static void MinimalConfiguration(IServiceCollection services)
    {
      services.AddBlazorFlexLoader();
    }

    /// <summary>
    /// Configuration avec interception HTTP basique.
    /// </summary>
    public static void BasicHttpInterception(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
  {
 client.BaseAddress = new Uri(baseAddress);
      });
    }

    /// <summary>
    /// Configuration pour une API instable nécessitant plus de retries.
    /// </summary>
    public static void UnstableApiConfiguration(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
  client =>
            {
     client.BaseAddress = new Uri(baseAddress);
                client.Timeout = TimeSpan.FromSeconds(30);
       },
         options =>
        {
       options.MaxRetryAttempts = 5;
   options.UseExponentialBackoff = true;
      options.RetryDelay = TimeSpan.FromSeconds(2);
        });
    }

    /// <summary>
    /// Configuration pour intercepter uniquement les routes API.
    /// </summary>
    public static void ApiOnlyInterception(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
            client =>
    {
              client.BaseAddress = new Uri(baseAddress);
    },
        options =>
    {
           options.InterceptPredicate = request =>
       request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    });
    }

    /// <summary>
    /// Configuration pour afficher le loader uniquement sur les mutations (POST/PUT/DELETE).
    /// </summary>
    public static void MutationsOnlyLoader(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
        client =>
        {
     client.BaseAddress = new Uri(baseAddress);
    },
   options =>
 {
            options.ShowLoaderPredicate = request =>
      request.Method == HttpMethod.Post ||
            request.Method == HttpMethod.Put ||
   request.Method == HttpMethod.Delete;
  });
    }

    /// <summary>
    /// Configuration avec retry personnalisé et codes HTTP spécifiques.
    /// </summary>
    public static void CustomRetryConfiguration(IServiceCollection services, string baseAddress)
    {
     services.AddBlazorFlexLoaderWithHttpInterceptor(
  client =>
       {
             client.BaseAddress = new Uri(baseAddress);
    },
 options =>
  {
    options.MaxRetryAttempts = 3;
                options.RetryDelay = TimeSpan.FromSeconds(1);
        options.UseExponentialBackoff = true;
                
 options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
                {
     HttpStatusCode.InternalServerError,      // 500
        HttpStatusCode.BadGateway,         // 502
     HttpStatusCode.ServiceUnavailable,       // 503
    HttpStatusCode.GatewayTimeout,           // 504
       HttpStatusCode.TooManyRequests           // 429
                };
            });
    }

    /// <summary>
    /// Configuration avec callback de retry pour notifications utilisateur.
    /// </summary>
    public static void WithRetryNotifications(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
          client =>
       {
   client.BaseAddress = new Uri(baseAddress);
  },
      options =>
   {
        options.OnRetry = async (attempt, exception, delay) =>
           {
           // Ici vous pouvez afficher une notification, logger, etc.
       Console.WriteLine($"Tentative {attempt}/3 après {delay.TotalSeconds}s");
      await Task.CompletedTask;
        };
            });
    }

    /// <summary>
    /// Configuration pour environnement de développement avec logging détaillé.
    /// </summary>
    public static void DevelopmentConfiguration(IServiceCollection services, string baseAddress, bool isDevelopment)
    {
      services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
       {
    client.BaseAddress = new Uri(baseAddress);
  },
            options =>
       {
    options.EnableDetailedLogging = isDevelopment;
    options.MaxRetryAttempts = isDevelopment ? 1 : 3;
         
             if (isDevelopment)
     {
        options.OnRetry = async (attempt, ex, delay) =>
 {
        Console.WriteLine($"[DEV] Retry: {ex?.Message}");
    await Task.CompletedTask;
   };
       }
       });
    }

    /// <summary>
    /// Configuration complète pour production avec toutes les options.
    /// </summary>
    public static void ProductionConfiguration(IServiceCollection services, string baseAddress)
    {
services.AddBlazorFlexLoaderWithHttpInterceptor(
            client =>
    {
       client.BaseAddress = new Uri(baseAddress);
      client.Timeout = TimeSpan.FromSeconds(30);
       },
   options =>
       {
         // Retry configuration
             options.MaxRetryAttempts = 3;
         options.UseExponentialBackoff = true;
          options.RetryDelay = TimeSpan.FromSeconds(1);
              options.RetryOnTimeout = true;
    
            // Status codes
   options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
             {
          HttpStatusCode.InternalServerError,
       HttpStatusCode.BadGateway,
      HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
      };
       
     // Intercept only API calls
           options.InterceptPredicate = request =>
 request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
  
  // Show loader for all intercepted requests
       options.ShowLoaderPredicate = null;
      
          // Logging
    options.EnableDetailedLogging = false;
            
    // Retry callback for monitoring
    options.OnRetry = async (attempt, exception, delay) =>
       {
      // Ici vous pourriez envoyer des métriques à votre système de monitoring
    await Task.CompletedTask;
           };
        });
    }

    /// <summary>
    /// Configuration pour une SPA avec navigation côté client.
    /// N'intercepte que les appels API, pas les navigations.
    /// </summary>
    public static void SpaConfiguration(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
            client =>
     {
            client.BaseAddress = new Uri(baseAddress);
   },
      options =>
     {
              // N'intercepte que les vraies requêtes API
      options.InterceptPredicate = request =>
           {
               var path = request.RequestUri?.AbsolutePath ?? string.Empty;
  return path.StartsWith("/api/") || 
          path.StartsWith("/odata/") ||
    path.StartsWith("/graphql/");
        };
 
       // Affiche le loader pour toutes les opérations sauf GET
    options.ShowLoaderPredicate = request =>
     request.Method != HttpMethod.Get;
      });
    }

    /// <summary>
    /// Configuration avec retry limité pour éviter les boucles infinies.
    /// </summary>
    public static void SafeRetryConfiguration(IServiceCollection services, string baseAddress)
    {
        services.AddBlazorFlexLoaderWithHttpInterceptor(
      client =>
            {
         client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(15);
       },
            options =>
            {
       options.MaxRetryAttempts = 2; // Limite à 2 retries
    options.UseExponentialBackoff = false; // Délai constant
                options.RetryDelay = TimeSpan.FromSeconds(1);
            options.RetryOnTimeout = false; // Pas de retry sur timeout
      
         // Ne retry que sur les erreurs serveur temporaires
   options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
        {
     HttpStatusCode.ServiceUnavailable, // 503
     HttpStatusCode.GatewayTimeout      // 504
     };
   });
    }
}
