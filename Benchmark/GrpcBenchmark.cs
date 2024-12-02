using BenchmarkDotNet.Attributes;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Grpc.Protobuf.Server.Proto;
using Grpc.Server;
using Grpc.Server.Requests;
using MagicOnion.Client;

[MemoryDiagnoser]
public class GrpcBenchmark
{
    private GrpcChannel magicOnionChannel;
    private GrpcChannel protobufChannel;

    private ITestService magicOnionClient;
    private MyFirstServiceGrpc.MyFirstServiceGrpcClient _grpcClient;

    [GlobalSetup]
    public void Setup()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        // Set up channels for both services
        magicOnionChannel = GrpcChannel.ForAddress("https://localhost:5001"); // MagicOnion service
        protobufChannel = GrpcChannel.ForAddress("https://localhost:5003", new GrpcChannelOptions
        {
            HttpHandler = handler
        }); // Protobuf service

        // Create clients
        magicOnionClient = MagicOnionClient.Create<ITestService>(magicOnionChannel);
        _grpcClient = new MyFirstServiceGrpc.MyFirstServiceGrpcClient(protobufChannel);
    }

    [Benchmark]
    public async Task TestMagicOnionService()
    {
        var response = await magicOnionClient.TestAsync(new Grpc.Server.Requests.CustomRequest()
        {
            ByteCollection = [1, 2, 3, 4],
            NullableInt = null,
            NullableIntCollection = null,
            Text = "This is an example text",
            GenericInt = new Interval<int> { Start = 1, End = 10 },
            NullableGenericInt = null,
            DateTime = DateTime.Now,
            NullableDateTime = null
        });
    }

    [Benchmark]
    public async Task TestProtobufService()
    {
        var response = await _grpcClient.TestAsyncAsync(new Grpc.Protobuf.Server.Proto.CustomRequest()
        {
            // ByteCollection = [1, 2, 3, 4],  
            NullableInt = 0,
            IntCollection = { },
            Text = "This is an example text",
            GenericInt = new Interval() { Start = 1, End = 10 },
            NullableGenericInt = null,
            DateTime = Timestamp.FromDateTime(DateTime.UtcNow),
            NullableDateTime = null
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        magicOnionChannel.Dispose();
        protobufChannel.Dispose();
    }
}