using Grpc.Server.Requests;
using MessagePack;

namespace Grpc.Server.Responses;

[MessagePackObject]
public class CustomResponse
{
    [Key(0)] public byte[] ByteCollection { get; set; }

    [Key(1)] public int? NullableInt { get; set; }

    [Key(2)] public int[]? NullableIntCollection { get; set; }

    [Key(8)] public int[] IntCollection { get; set; }
    [Key(3)] public string Text { get; set; }
    
    [Key(4)] public Interval<int> GenericInt { get; set; }
    
    [Key(5)] public Interval<float>? NullableGenericInt { get; set; }
    
    [Key(6)] public DateTime DateTime { get; set; }
    
    [Key(7)] public DateTime? NullableDateTime { get; set; }
}