using Grpc.Common.Requests;
using Grpc.Common.Responses;
using Grpc.Core;

namespace Grpc.Common;

public interface ICustomService
{
    Task ProcessRequestAsync(IAsyncStreamReader<CustomRequest> requestStream, 
        IServerStreamWriter<CustomResponse> responseStream, 
        ServerCallContext context);
}