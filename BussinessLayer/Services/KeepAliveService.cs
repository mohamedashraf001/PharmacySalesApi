using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class KeepAliveService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KeepAliveService> _logger;

    public KeepAliveService(IHttpClientFactory httpClientFactory, ILogger<KeepAliveService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Send an HTTP request to your app to keep it alive
                var response = await client.GetAsync("https://pharmacy1api20241126012046.azurewebsites.net/");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Ping successful at {Time}", DateTimeOffset.Now);
                }
                else
                {
                    _logger.LogWarning("Ping failed with status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending ping request.");
            }

            // Wait for 10 minutes before sending the next request
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
