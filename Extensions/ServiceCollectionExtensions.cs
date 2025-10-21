using Microsoft.Extensions.DependencyInjection;
using Blazor.FlexLoader.Services;

namespace Blazor.FlexLoader.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services Blazor.FlexLoader
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute les services Blazor.FlexLoader à la collection de services
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <returns>Collection de services pour chainage</returns>
    public static IServiceCollection AddBlazorFlexLoader(this IServiceCollection services)
    {
        services.AddScoped<LoaderService>();
        return services;
    }
}