using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace SimpleUsingObservation;
internal class Program
{
    private const string AppName = "SimpleUsingObservation";
    private const string Version = "1.0.0.0";
    
    private static readonly HttpClient s_httpClient = new();
    private static readonly ActivitySource s_activitySource = new(AppName, Version);
    
    private static async Task Main(string[] args)
    {
        string url = args.Length > 0 ? args[0] : "http://numbersapi.com/random/math?json";

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(AppName, Version);

        // Initialize the OpenTelemetry TracerProvider.
        _ = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(AppName)
            .AddConsoleExporter()
            .Build();

        // This doesn't work with the simple using block but works with the scoped using block. Removing using also doesn't work. 
        using var activity = s_activitySource.StartActivity();
        activity?.SetTag("url", url);
        var response = await GetDataAsync(url);
        Console.ReadLine();
    }

    private static async Task<string> GetDataAsync(string url)
    {  
        var response = await s_httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}