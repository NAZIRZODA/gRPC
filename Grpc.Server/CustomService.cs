using Grpc.Common;
using Grpc.Common.Requests;
using Grpc.Common.Responses;
using Grpc.Core;
using Grpc.Core.Utils;

namespace Grpc.Server;

public class CustomService : ICustomService
{
    private readonly ILogger<CustomService> _logger;

    public CustomService(ILogger<CustomService> logger)
    {
        _logger = logger;
    }

    public async Task ProcessRequestAsync(
        IAsyncStreamReader<CustomRequest> requestStream,
        IServerStreamWriter<CustomResponse> responseStream,
        ServerCallContext context)
    {
        try
        {
            await requestStream.ForEachAsync(async request =>
            {
                await responseStream.WriteAsync(new CustomResponse
                {
                    Payload = request.Payload,
                    NullablePayload = null,
                    NullableCollection = [1, 2, 3],
                    Text = "Test string"
                });
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request stream");
            throw;
        }
    }
}