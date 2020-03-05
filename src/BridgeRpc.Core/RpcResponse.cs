using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcResponse
    {
        public string Version
        {
            get => RawObject["bridgerpc"].ToString();
            set => RawObject["bridgerpc"] = value;
        }
        
        public string Id
        {
            get => RawObject["id"].ToString();
            set => RawObject["id"] = value;
        }
        
        public object Result => RawObject["result"];
        
        public RpcError Error { get; private set; }
        
        protected JObject RawObject { get; set; }

        public RpcResponse()
        {
            RawObject = new JObject {["bridgerpc"] = "1.0", ["id"] = "", ["result"] = null, ["error"] = null};
        }

        public RpcResponse(string json)
        {
            try
            {
                RawObject = JObject.Parse(json);
                SyncErrorObject();
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
            return RawObject.ToString();
        }

        public void SetResult<T>(T obj)
        {
            if (obj is JToken token)
            {
                RawObject["result"] = token;
            }
            else if (obj is string str)
            {
                RawObject["result"] = str;
            }
            else
            {
                RawObject["result"] = JObject.FromObject(obj);
            }
        }

        public T GetResult<T>()
        {
            return RawObject["result"].ToObject<T>();
        }
        
        public JToken GetResult()
        {
            return RawObject["result"];
        }

        public void SetError<T>(int code, string message, T data)
        {
            var err = new JObject {{"code", code}, {"message", message}, {"data", JToken.FromObject(data)}};
            RawObject["error"] = err;
            SyncErrorObject();
        }
        
        public void ClearError()
        {
            RawObject["error"] = null;
            SyncErrorObject();
        }

        private void SyncErrorObject()
        {
            Error = RawObject.Property("error").Value.Type == JTokenType.Null ? 
                null : new RpcError(RawObject.Property("error").Value as JObject);
        }
    }
}