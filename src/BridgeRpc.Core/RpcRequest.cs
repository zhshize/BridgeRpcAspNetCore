using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcRequest
    {
        [JsonProperty("bridgerpc")]
        public string Version { get; set; } = "1.0";
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("method")]
        public string Method { get; set; }
        
        [JsonProperty("data")]
        public JRaw Data { get; set; }
        
        public RpcRequest()
        {
            
        }

        public RpcRequest(string json)
        {
            try
            {
                var request = FromJsonString(json);
                Version = request.Version;
                Id = request.Id;
                Method = request.Method;
                Data = request.Data;
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

        public T GetParameterFromData<T>(string name)
        {
            return Data.Type == JTokenType.Object ? Data[name].ToObject<T>() : (T) Data.Value;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public bool IsNotify()
        {
            return Id == null;
        }

        public void SetData<T>(T obj)
        {
            Data = new JRaw(obj);
        }

        public T GetData<T>()
        {
            return Data.ToObject<T>();
        }
        
        public static RpcRequest FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<RpcRequest>(json);
        }
    }
}