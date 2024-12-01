using Grpc.Server.Requests;
using Grpc.Server.Responses;
using MagicOnion;
using MagicOnion.Server;

namespace Grpc.Server;

public interface IMyFirstService : IService<IMyFirstService>
{
    UnaryResult<int> SumAsync(int x, int y);
}
    
public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
{
    public MyFirstService()
    {
        Console.WriteLine("MyFirstService instantiated");
    }
    public async UnaryResult<int> SumAsync(int x, int y)
    {
        Console.WriteLine($"Received:{x}, {y}");
        return x + y;
    }
}

public interface ITestService : IService<ITestService>
{
    UnaryResult<CustomResponse> TestAsync(CustomRequest request);
}

public class TestService: ServiceBase<ITestService>, ITestService
{
    public async UnaryResult<CustomResponse> TestAsync(CustomRequest request)
    {
        return new CustomResponse()
        {
            ByteCollection = [1, 2, 3, 4],  
            NullableInt = 5,               
            NullableIntCollection = [2,3,4], 
            IntCollection = [],
            Text = "This is an example text",
            GenericInt = new Interval<int>(){Start = 1, End = 10},
            NullableGenericInt = new Interval<float>()
            {
                Start = 1,
                End = 60
            },
            DateTime = DateTime.Now,
            NullableDateTime = DateTime.Today.AddDays(-2)
        };
    }
}