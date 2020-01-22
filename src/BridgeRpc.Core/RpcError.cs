using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("data")]
        public JRaw Data { get; set; }

        public RpcError()
        {
            
        }

        public RpcError(int code, string message, JRaw data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        
        public T GetData<T>()
        {
            return Data.ToObject<T>();
        }
    }
}