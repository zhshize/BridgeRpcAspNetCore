using MessagePack;

namespace BridgeRpc.Core
{
    [MessagePackObject]
    public class RpcResponse
    {
        [Key("bridgerpc")]
        public string Version { get; set; } = "1.0";
        [Key("id")]
        public string Id { get; set; }
        
        [Key("result")]
        public object Result { get; set; }
        [Key("error")]
        public RpcError Error { get; set; }
        
        public byte[] ToBinary()
        {
            return MessagePackSerializer.Serialize(this);
        }
        
        public object GetParameterFromResult(string name)
        {
            /*var dynamicModel = 
                MessagePackSerializer.Deserialize<dynamic>(Result, ContractlessStandardResolver.Instance);

            return dynamicModel[name];*/
            dynamic result = Result;
            return result[name];
        }
        
        public T GetResult<T>()
        {
            //return MessagePackSerializer.Deserialize<T>(Result);
            return (T) Result;
        }
    }
}