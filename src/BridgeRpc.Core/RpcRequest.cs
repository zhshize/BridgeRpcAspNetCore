using MessagePack;
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
        public byte[] Data { get; set; }

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
            var dynamicModel = 
                MessagePackSerializer.Deserialize<dynamic>(Data, ContractlessStandardResolver.Instance);

            return dynamicModel[name];
        }
        
        public T GetData<T>()
        {
            return MessagePackSerializer.Deserialize<T>(Data);
        }
    }
}