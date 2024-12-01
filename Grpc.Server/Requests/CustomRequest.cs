using MessagePack;

namespace Grpc.Server.Requests;

[MessagePackObject]
public class CustomRequest
{
    [Key(0)] public byte[] ByteCollection { get; set; }

    [Key(1)] public int? NullableInt { get; set; }

    [Key(2)] public int[]? NullableIntCollection { get; set; }

    [Key(3)] public string Text { get; set; }
    
    [Key(4)] public Interval<int> GenericInt { get; set; }
    
    [Key(5)] public Interval<int>? NullableGenericInt { get; set; }
    
    [Key(6)] public DateTime DateTime { get; set; }
    
    [Key(7)] public DateTime? NullableDateTime { get; set; }
}

[MessagePackObject]
public class Interval<T>
{
    [Key(0)]
    public T Start { get; set; }

    [Key(1)]
    public T End { get; set; }
}