using MessagePack;

namespace Grpc.Common;

public static class MessagePackSerializer<T>
{
    public static byte[] ToBytes(T message)
    {
        try
        {
            return MessagePackSerializer.Serialize(message);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to serialize {typeof(T).Name}", ex);
        }
    }

    public static T FromBytes(byte[] bytes)
    {
        try
        {
            return MessagePackSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to deserialize {typeof(T).Name}", ex);
        }
    }
}
    
public class Marshaller<TRequest, TResponse>
{
    public Func<TRequest, byte[]> RequestSerializer { get; }
    public Func<byte[], TRequest> RequestDeserializer { get; }
    public Func<TResponse, byte[]> ResponseSerializer { get; }
    public Func<byte[], TResponse> ResponseDeserializer { get; }

    public Marshaller(
        Func<TRequest, byte[]> requestSerializer,
        Func<byte[], TRequest> requestDeserializer,
        Func<TResponse, byte[]> responseSerializer,
        Func<byte[], TResponse> responseDeserializer)
    {
        RequestSerializer = requestSerializer;
        RequestDeserializer = requestDeserializer;
        ResponseSerializer = responseSerializer;
        ResponseDeserializer = responseDeserializer;
    }
}