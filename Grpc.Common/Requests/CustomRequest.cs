using MessagePack;

namespace Grpc.Common.Requests;

    [MessagePackObject]
    public class CustomRequest
    {
        [Key(0)]
        public byte[] Payload { get; set; }
        
        [Key(1)]
        public int? NullablePayload { get; set; }
        
        [Key(2)]
        public int[]? NullableCollection { get; set; }
        
        [Key(3)]
        public string Text { get; set; }
    }