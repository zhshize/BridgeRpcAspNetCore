using MessagePack;
using MessagePack.Formatters;

namespace BridgeRpc
{
    [MessagePackObject]
    public class RpcError
    {
        [Key("code")]
        public int Code { get; set; }
        [Key("message")]
        public string Message { get; set; }
        
        [MessagePackFormatter(typeof(TypelessFormatter))]
        [Key("data")]
        public object Data { get; set; }

        public RpcError(int code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
    }
}