using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Protobuf.Server.Proto;

namespace Grpc.Protobuf.Server.Grpc;

public class GrpcService : MyFirstServiceGrpc.MyFirstServiceGrpcBase
{
    public override async Task<CustomResponse> TestAsync(CustomRequest request, ServerCallContext context)
    {
        return new CustomResponse()
        {
            // ByteCollection = { new []{1,2,3} },
            NullableInt = 5,
            IntCollection = { new []{1,2,3} },
            Text = "This is an example text",
            GenericInt = new Interval() { Start = 1, End = 10 },
            NullableGenericInt = new()
            {
                Start = 1,
                End = 60
            },
            DateTime = Timestamp.FromDateTime(DateTime.UtcNow),
            NullableDateTime = Timestamp.FromDateTime(DateTime.UtcNow)
        };
    }
}