using MessagePack;

namespace BridgeRpc.Core
{
    [MessagePackObject]
    public class RpcRequest
    {
        [Key("bridgerpc")]
        public string Version { get; set; } = "1.0";
        [Key("id")]
        public string Id { get; set; }
        
        [Key("method")]
        public string Method { get; set; }
        
        [Key("data")]
        public byte[] Data { get; set; }

        public byte[] ToBinary()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public bool IsNotify()
        {
            return Id == null;
        }
        
        public T GetParameterFromData<T>(string name)
        {
            dynamic data = MessagePackSerializer.Typeless.Deserialize(Data);
            return data[name];
        }
        
        public T GetData<T>()
        {
            return MessagePackSerializer.Deserialize<T>(Data);
        }
    }
}