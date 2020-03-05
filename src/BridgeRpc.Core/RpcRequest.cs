using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcRequest
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
        
        public string Method
        {
            get => RawObject["method"].ToString();
            set => RawObject["method"] = value;
        }
        
        public object Data => RawObject["data"];

        protected JObject RawObject { get; set; }
        
        public RpcRequest()
        {
            RawObject = new JObject {["bridgerpc"] = "1.0", ["id"] = "", ["method"] = "", ["data"] = null};
        }

        public RpcRequest(string json)
        {
            try
            {
                RawObject = JObject.Parse(json);
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
            return RawObject["data"].Type == JTokenType.Object ? RawObject["data"][name].ToObject<T>() : RawObject["data"].Value<T>();
        }

        public string ToJson()
        {
            return RawObject.ToString();
        }

        public bool IsNotify()
        {
            return Id == null;
        }

        public void SetData<T>(T obj)
        {
            if (obj is JToken token)
            {
                RawObject["data"] = token;
            }
            else if (obj is string str)
            {
                RawObject["data"] = str;
            }
            else
            {
                RawObject["data"] = JObject.FromObject(obj);
            }
        }

        public T GetData<T>()
        {
            return RawObject["data"].ToObject<T>();
        }
        
        public JToken GetData()
        {
            return RawObject["data"];
        }
    }
}