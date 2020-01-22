using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcResponse
    {
        [JsonProperty("bridgerpc")]
        public string Version { get; set; } = "1.0";
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("result")]
        public JRaw Result { get; set; }
        
        [JsonProperty("error")]
        public RpcError Error { get; set; }

        public RpcResponse()
        {
            
        }

        public RpcResponse(string json)
        {
            try
            {
                var response = FromJsonString(json);
                Version = response.Version;
                Id = response.Id;
                Result = response.Result;
                Error = response.Error;
            }
            catch (JsonException je)
            {
                throw new RpcException(RpcErrorCode.ParseError, "Json paring error in RpcResponse(string json).", je);
            }
            catch (Exception e)
            {
                throw new RpcException(RpcErrorCode.InternalError, "Internal error in RpcResponse(string json).", e);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public T GetResult<T>()
        {
            return Result.ToObject<T>();
        }
        
        public static RpcResponse FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<RpcResponse>(json);
        }
    }
}