using Grpc.Common.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Client.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IGrpcClient _grpcClient;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IGrpcClient grpcClient)
    {
        _logger = logger;
        _grpcClient = grpcClient;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task Get()
    {
        var requests = new List<CustomRequest>
        {
            new() { Text = "Message 1", Payload = new byte[] { 1, 2, 3 } },
            new() { Text = "Message 2", Payload = new byte[] { 4, 5, 6 } },
            new() { Text = "Message 3", Payload = new byte[] { 7, 8, 9 } }
        };

        try
        {
            await _grpcClient.SendMessagesAsync(
                requests,
                async response =>
                {
                    // Handle each response
                    await Task.CompletedTask;
                });
        }
        catch
        {
        }
    }
}