using Blazor.FlexLoader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.FlexLoader.Examples;

/// <summary>
/// Exemples d'utilisation des m�triques et du CancellationToken global.
/// </summary>
/// <remarks>
/// Usage examples for metrics and global CancellationToken.
/// </remarks>
public static class MetricsAndCancellationExamples
{
    /// <summary>
    /// Exemple 1 : Affichage des m�triques en temps r�el.
    /// </summary>
    public static void DisplayMetricsExample(LoaderService loaderService)
    {
  // Obtenir les m�triques
    var metrics = loaderService.Metrics;

        Console.WriteLine($"Total Requests: {metrics.TotalRequests}");
        Console.WriteLine($"Success Rate: {metrics.SuccessRate:F2}%");
        Console.WriteLine($"Failure Rate: {metrics.FailureRate:F2}%");
     Console.WriteLine($"Retry Rate: {metrics.RetryRate:F2}%");
   Console.WriteLine($"Average Response Time: {metrics.AverageResponseTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"Max Response Time: {metrics.MaxResponseTime.TotalMilliseconds:F2}ms");
        
  // Ou utiliser le r�sum� format�
        Console.WriteLine(loaderService.GetMetricsSummary());
    }

    /// <summary>
    /// Exemple 2 : Surveillance des m�triques en temps r�el dans une application Blazor.
    /// </summary>
    public static class MetricsMonitorComponent
    {
    public static string GetMetricsMarkup(LoaderService loaderService)
     {
 var metrics = loaderService.Metrics;
        
  return $@"
<div class=""metrics-panel"">
    <h3>?? Performance Metrics</h3>
    
    <div class=""metric-card"">
      <div class=""metric-title"">Total Requests</div>
        <div class=""metric-value"">{metrics.TotalRequests}</div>
    </div>
    
    <div class=""metric-card success"">
        <div class=""metric-title"">Success Rate</div>
        <div class=""metric-value"">{metrics.SuccessRate:F1}%</div>
        <div class=""metric-sub"">{metrics.SuccessfulRequests} successful</div>
    </div>
    
    <div class=""metric-card {(metrics.FailureRate > 10 ? "warning" : "")}"">
        <div class=""metric-title"">Failure Rate</div>
        <div class=""metric-value"">{metrics.FailureRate:F1}%</div>
   <div class=""metric-sub"">{metrics.FailedRequests} failed</div>
    </div>
    
    <div class=""metric-card"">
        <div class=""metric-title"">Retry Rate</div>
    <div class=""metric-value"">{metrics.RetryRate:F1}%</div>
     <div class=""metric-sub"">{metrics.RetriedRequests} retried</div>
    </div>
    
    <div class=""metric-card"">
        <div class=""metric-title"">Avg Response Time</div>
      <div class=""metric-value"">{metrics.AverageResponseTime.TotalMilliseconds:F0}ms</div>
        <div class=""metric-sub"">Max: {metrics.MaxResponseTime.TotalMilliseconds:F0}ms</div>
    </div>
    
    <div class=""metric-card"">
    <div class=""metric-title"">Last Request</div>
        <div class=""metric-value"">{metrics.LastRequestTime?.ToString("HH:mm:ss") ?? "N/A"}</div>
    </div>
</div>

<style>
    .metrics-panel {{
 display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
   gap: 1rem;
 padding: 1rem;
    }}
    
    .metric-card {{
        background: #f5f5f5;
        border-radius: 8px;
        padding: 1rem;
        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }}
    
    .metric-card.success {{
      background: #d4edda;
        border-left: 4px solid #28a745;
    }}
  
    .metric-card.warning {{
        background: #fff3cd;
        border-left: 4px solid #ffc107;
    }}
  
    .metric-title {{
        font-size: 0.875rem;
        color: #666;
        margin-bottom: 0.5rem;
    }}
    
    .metric-value {{
        font-size: 1.5rem;
        font-weight: bold;
        color: #333;
    }}
    
    .metric-sub {{
        font-size: 0.75rem;
        color: #999;
    margin-top: 0.25rem;
    }}
</style>
";
    }
    }

    /// <summary>
    /// Exemple 3 : R�initialisation des m�triques.
    /// </summary>
    public static void ResetMetricsExample(LoaderService loaderService)
    {
        // Afficher les m�triques actuelles
   Console.WriteLine("Before reset:");
   Console.WriteLine(loaderService.GetMetricsSummary());
        
        // R�initialiser
        loaderService.ResetMetrics();
        
 // V�rifier
        Console.WriteLine("\nAfter reset:");
        Console.WriteLine(loaderService.GetMetricsSummary());
    }

    /// <summary>
    /// Exemple 4 : Analyse des codes de statut HTTP.
    /// </summary>
    public static void AnalyzeStatusCodesExample(LoaderService loaderService)
    {
        var distribution = loaderService.Metrics.StatusCodeDistribution;
        
        Console.WriteLine("HTTP Status Code Distribution:");
        foreach (var (statusCode, count) in distribution.OrderByDescending(x => x.Value))
        {
   var percentage = loaderService.Metrics.TotalRequests > 0 
              ? (double)count / loaderService.Metrics.TotalRequests * 100 
     : 0;
         
      Console.WriteLine($"  {statusCode}: {count} ({percentage:F1}%)");
        }
    }

    /// <summary>
    /// Exemple 5 : Annulation de toutes les requ�tes en cours.
    /// </summary>
    public static async Task CancelAllRequestsExample(LoaderService loaderService, IHttpClientFactory httpClientFactory)
    {
    var client = httpClientFactory.CreateClient("BlazorFlexLoader");
     
        // D�marrer plusieurs requ�tes
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
  try
      {
         // Les requ�tes utiliseront le GlobalCancellationToken
          await client.GetAsync($"/api/data/{i}");
 }
      catch (OperationCanceledException)
     {
             Console.WriteLine($"Request {i} was cancelled");
     }
            }));
}
        
        // Attendre un peu
        await Task.Delay(100);
        
        // Annuler toutes les requ�tes
        loaderService.CancelAllRequests();
        
 // Attendre que toutes les t�ches se terminent
   await Task.WhenAll(tasks);
        
        Console.WriteLine("All requests cancelled");
    }

    /// <summary>
    /// Exemple 6 : Utilisation du CancellationToken global dans une requ�te manuelle.
    /// </summary>
    public static async Task ManualRequestWithGlobalCancellationExample(
        LoaderService loaderService, 
        HttpClient httpClient)
    {
  try
        {
        loaderService.Show();
    
        // Utiliser le GlobalCancellationToken
         var response = await httpClient.GetAsync(
   "/api/data", 
   loaderService.GlobalCancellationToken);
       
     Console.WriteLine($"Response: {response.StatusCode}");
        }
        catch (OperationCanceledException)
   {
            Console.WriteLine("Request was cancelled globally");
  }
        finally
      {
   loaderService.Close();
   }
    }

    /// <summary>
    /// Exemple 7 : Surveillance continue des m�triques avec alertes.
/// </summary>
    public static async Task MonitorMetricsWithAlertsExample(LoaderService loaderService)
    {
        while (true)
        {
    var metrics = loaderService.Metrics;
   
            // Alerte si taux d'�chec > 20%
 if (metrics.FailureRate > 20 && metrics.TotalRequests >= 10)
     {
                Console.WriteLine($"?? WARNING: High failure rate detected: {metrics.FailureRate:F1}%");
            }
    
         // Alerte si temps de r�ponse moyen > 5s
            if (metrics.AverageResponseTime.TotalSeconds > 5)
 {
    Console.WriteLine($"?? WARNING: Slow response time: {metrics.AverageResponseTime.TotalSeconds:F1}s");
            }
 
            // Alerte si trop de retries
       if (metrics.RetryRate > 50 && metrics.TotalRequests >= 10)
            {
           Console.WriteLine($"?? WARNING: High retry rate: {metrics.RetryRate:F1}%");
      }
            
       await Task.Delay(TimeSpan.FromSeconds(10));
   }
    }

    /// <summary>
    /// Exemple 8 : Export des m�triques pour monitoring externe.
    /// </summary>
    public static Dictionary<string, object> ExportMetricsForMonitoring(LoaderService loaderService)
    {
   var metrics = loaderService.Metrics;
        
        return new Dictionary<string, object>
        {
     ["timestamp"] = DateTime.UtcNow,
            ["total_requests"] = metrics.TotalRequests,
    ["successful_requests"] = metrics.SuccessfulRequests,
      ["failed_requests"] = metrics.FailedRequests,
  ["retried_requests"] = metrics.RetriedRequests,
        ["total_retry_attempts"] = metrics.TotalRetryAttempts,
        ["success_rate"] = metrics.SuccessRate,
    ["failure_rate"] = metrics.FailureRate,
     ["retry_rate"] = metrics.RetryRate,
         ["avg_response_time_ms"] = metrics.AverageResponseTime.TotalMilliseconds,
            ["max_response_time_ms"] = metrics.MaxResponseTime.TotalMilliseconds,
            ["min_response_time_ms"] = metrics.MinResponseTime == TimeSpan.MaxValue 
                ? 0 
      : metrics.MinResponseTime.TotalMilliseconds,
          ["last_request_time"] = metrics.LastRequestTime,
  ["status_code_distribution"] = metrics.StatusCodeDistribution
        };
  }

    /// <summary>
    /// Exemple 9 : Composant Blazor complet avec m�triques et annulation.
    /// </summary>
    public static string GetFullBlazorComponentExample()
    {
        return @"
@page ""/admin/metrics""
@inject LoaderService LoaderService
@inject IHttpClientFactory HttpClientFactory
@implements IDisposable

<h3>?? System Metrics Dashboard</h3>

<div class=""metrics-dashboard"">
    <div class=""metrics-summary"">
    @loaderService.GetMetricsSummary()
    </div>
    
    <div class=""metrics-charts"">
        <h4>Status Code Distribution</h4>
   @foreach (var (code, count) in LoaderService.Metrics.StatusCodeDistribution)
   {
            var percentage = LoaderService.Metrics.TotalRequests > 0 
            ? (double)count / LoaderService.Metrics.TotalRequests * 100 
        : 0;
                
            <div class=""status-bar"">
      <span>@code</span>
            <div class=""bar"" style=""width: @(percentage)%""></div>
                <span>@count (@percentage.ToString(""F1""))%</span>
    </div>
     }
    </div>
    
    <div class=""metrics-actions"">
        <button @onclick=""ResetMetrics"" class=""btn btn-warning"">Reset Metrics</button>
        <button @onclick=""CancelAllRequests"" class=""btn btn-danger"">Cancel All Requests</button>
        <button @onclick=""RefreshMetrics"" class=""btn btn-primary"">Refresh</button>
    </div>
</div>

@code {
    private System.Timers.Timer? _timer;

    protected override void OnInitialized()
    {
     // Rafra�chir toutes les 5 secondes
   _timer = new System.Timers.Timer(5000);
        _timer.Elapsed += async (sender, e) => await InvokeAsync(StateHasChanged);
        _timer.Start();
    }

    private void ResetMetrics()
    {
LoaderService.ResetMetrics();
    }

private void CancelAllRequests()
    {
  LoaderService.CancelAllRequests();
    }

    private void RefreshMetrics()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
}
";
    }
}
