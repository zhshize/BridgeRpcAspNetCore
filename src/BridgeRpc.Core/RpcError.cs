using MessagePack;

namespace BridgeRpc.Core
{
    [MessagePackObject]
    public class RpcError
    {
        [Key("code")]
        public int Code { get; set; }
        [Key("message")]
        public string Message { get; set; }
        
        [Key("data")]
        public byte[] Data { get; set; }

        public RpcError(int code, string message, byte[] data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}