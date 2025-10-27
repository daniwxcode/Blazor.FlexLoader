using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Blazor.FlexLoader.Services;
using Blazor.FlexLoader.Handlers;

namespace Blazor.FlexLoader.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services Blazor.FlexLoader.
/// </summary>
/// <remarks>
/// Extensions for registering Blazor.FlexLoader services.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute les services Blazor.FlexLoader à la collection de services.
    /// Enregistre le <see cref="LoaderService"/> comme service scoped.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <returns>Collection de services pour chaînage.</returns>
    /// <remarks>
    /// Adds Blazor.FlexLoader services to the service collection.
    /// Registers the <see cref="LoaderService"/> as a scoped service.
    /// </remarks>
    public static IServiceCollection AddBlazorFlexLoader(this IServiceCollection services)
    {
        services.AddScoped<LoaderService>();
        return services;
    }

    /// <summary>
    /// Ajoute les services Blazor.FlexLoader avec interception automatique des requêtes HTTP.
    /// Configure un <see cref="HttpClient"/> avec le <see cref="HttpCallInterceptorHandler"/> 
    /// pour afficher automatiquement le loader lors des appels HTTP.
    /// </summary>
    /// <param name="services">Collection de services.</param>
    /// <param name="configureClient">Action optionnelle pour configurer le HttpClient.</param>
    /// <returns>Collection de services pour chaînage.</returns>
    /// <example>
    /// <code>
    /// // Dans Program.cs
    /// builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
    /// {
    ///     client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    /// });
    /// </code>
    /// </example>
    /// <remarks>
    /// Adds Blazor.FlexLoader services with automatic HTTP request interception.
    /// Configures an <see cref=\"HttpClient\"/> with the <see cref=\"HttpCallInterceptorHandler\"/> 
    /// to automatically display the loader during HTTP calls.
    /// </remarks>
    public static IServiceCollection AddBlazorFlexLoaderWithHttpInterceptor(
        this IServiceCollection services,
      Action<HttpClient>? configureClient = null)
    {
        services.AddScoped<LoaderService>();
      services.AddTransient<HttpCallInterceptorHandler>();

     services.AddHttpClient("BlazorFlexLoader", client =>
        {
  configureClient?.Invoke(client);
        })
        .AddHttpMessageHandler<HttpCallInterceptorHandler>();

return services;
    }
}