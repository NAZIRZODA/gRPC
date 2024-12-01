using Grpc.Net.Client;
using Grpc.Server;
using Grpc.Server.Requests;
using Grpc.Server.Responses;
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
    public async Task<CustomResponse> Get()
    {
 
        var channel = GrpcChannel.ForAddress("https://localhost:5001");

        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var test = MagicOnionClient.Create<ITestService>(channel);

        var result= await test.TestAsync(new CustomRequest
        {
            ByteCollection = [1, 2, 3, 4],  
            NullableInt = null,               
            NullableIntCollection = null, 
            Text = "This is an example text",
            GenericInt = new Interval<int>(){Start = 1, End = 10},
            NullableGenericInt = null,
            DateTime = DateTime.Now,
            NullableDateTime = null
        });
        foreach (var item in result.ByteCollection)
        {
            Console.WriteLine(item);
        }
        return result;
    }
}