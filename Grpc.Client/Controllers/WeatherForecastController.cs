using Grpc.Net.Client;
using Grpc.Server;
using MagicOnion.Client;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Client.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    public WeatherForecastController()
    {
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task Get()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7012");

        var client = MagicOnionClient.Create<IMyFirstService>(channel);

        var result = await client.SumAsync(123, 456);
        Console.WriteLine($"Result: {result}");

    }
}