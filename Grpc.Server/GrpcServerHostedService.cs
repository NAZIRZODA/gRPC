using Grpc.Common;
using Grpc.Common.Requests;
using Grpc.Common.Responses;
using Grpc.Core;

namespace Grpc.Server;

public class GrpcServerHostedService : IHostedService
{
    private Core.Server _server;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GrpcServerHostedService> _logger;

    public GrpcServerHostedService(
        IServiceProvider serviceProvider,
        ILogger<GrpcServerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var messagePackMarshaller = new Marshaller<CustomRequest, CustomResponse>(
            MessagePackSerializer<CustomRequest>.ToBytes,
            MessagePackSerializer<CustomRequest>.FromBytes,
            MessagePackSerializer<CustomResponse>.ToBytes,
            MessagePackSerializer<CustomResponse>.FromBytes
        );

        var method = new Method<CustomRequest, CustomResponse>(
            type: MethodType.DuplexStreaming,
            serviceName: "CustomService",
            name: "CustomMethod",
            requestMarshaller: Marshallers.Create(
                serializer: messagePackMarshaller.RequestSerializer,
                deserializer: messagePackMarshaller.RequestDeserializer),
            responseMarshaller: Marshallers.Create(
                serializer: messagePackMarshaller.ResponseSerializer,
                deserializer: messagePackMarshaller.ResponseDeserializer)
        );

        _server = new Core.Server
        {
            Ports = { new ServerPort("127.0.0.1", 5000, ServerCredentials.Insecure) },
            Services =
            {
                ServerServiceDefinition.CreateBuilder()
                    .AddMethod(method, HandleCallAsync)
                    .Build()
            }
        };

        _server.Start();
        _logger.LogInformation("gRPC server started on port 5000");

        return Task.CompletedTask;
    }

    private async Task HandleCallAsync(
        IAsyncStreamReader<CustomRequest> requestStream,
        IServerStreamWriter<CustomResponse> responseStream,
        ServerCallContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var customService = scope.ServiceProvider.GetRequiredService<ICustomService>();
        
        await customService.ProcessRequestAsync(requestStream, responseStream, context);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_server != null)
        {
            await _server.ShutdownAsync();
            _logger.LogInformation("gRPC server stopped");
        }
    }
}