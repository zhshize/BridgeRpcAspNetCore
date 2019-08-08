using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace BridgeRpc
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
        public object Data { get; set; }

        public byte[] ToBinary()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public bool IsNotify()
        {
            return Id == null;
        }
        
        public object GetParameterFromData(string name)
        {
            dynamic data = Data;
            return data[name];
        }
        
        public T GetData<T>()
        {
            return (T) Data;
        }
    }
}