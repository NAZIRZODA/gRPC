using Grpc.Common;
using Grpc.Common.Requests;
using Grpc.Common.Responses;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Grpc.Client;

 public interface IGrpcClient
    {
        Task SendMessagesAsync(IEnumerable<CustomRequest> requests, 
            Func<CustomResponse, Task> responseHandler, 
            CancellationToken cancellationToken = default);
    }

    public class GrpcClient : IGrpcClient, IDisposable
    {
        private readonly Channel _channel;
        private readonly Method<CustomRequest, CustomResponse> _method;
        private readonly ILogger<GrpcClient> _logger;
        private bool _disposed;

        public GrpcClient(
            IOptions<GrpcClientOptions> options,
            ILogger<GrpcClient> logger)
        {
            _logger = logger;
            var config = options.Value;
            
            _channel = new Channel(config.Host, config.Port, ChannelCredentials.Insecure);
            
            var messagePackMarshaller = new Marshaller<CustomRequest, CustomResponse>(
                MessagePackSerializer<CustomRequest>.ToBytes,
                MessagePackSerializer<CustomRequest>.FromBytes,
                MessagePackSerializer<CustomResponse>.ToBytes,
                MessagePackSerializer<CustomResponse>.FromBytes
            );

            _method = new Method<CustomRequest, CustomResponse>(
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
        }

        public async Task SendMessagesAsync(
            IEnumerable<CustomRequest> requests,
            Func<CustomResponse, Task> responseHandler,
            CancellationToken cancellationToken = default)
        {
            try
            {
                CustomResponse test = new CustomResponse();
                var callInvoker = new DefaultCallInvoker(_channel);
                using var call = callInvoker.AsyncDuplexStreamingCall(_method, null, new CallOptions(cancellationToken: cancellationToken));

                // Start receiving responses in a separate task
                var responseTask = HandleResponsesAsync(call.ResponseStream, responseHandler, cancellationToken);

                // Send all requests
                foreach (var request in requests)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    await call.RequestStream.WriteAsync(request, cancellationToken);
                    _logger.LogInformation("Sent request with text: {Text}", request.Text);
                }

                // Complete the request stream
                await call.RequestStream.CompleteAsync();
                _logger.LogInformation("Completed sending requests");

                // Wait for all responses
                await responseTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC communication");
                throw;
            }
        }

        private async Task HandleResponsesAsync(
            IAsyncStreamReader<CustomResponse> responseStream,
            Func<CustomResponse, Task> responseHandler,
            CancellationToken cancellationToken)
        {
            try
            {
                while (await responseStream.MoveNext(cancellationToken))
                {
                    var response = responseStream.Current;
                    await responseHandler(response);
                    _logger.LogInformation("Handled response with text: {Text}", response.Text);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("Stream cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling responses");
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _channel?.ShutdownAsync().Wait();
            }

            _disposed = true;
        }
    }

    public class GrpcClientOptions
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5000;
    }