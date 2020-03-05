using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcError
    {
        public int Code
        { 
            get => RawObject["code"].ToObject<int>();
            set => RawObject["code"] = value;
        }
        
        public string Message
        { 
            get => RawObject["message"].ToString();
            set => RawObject["message"] = value;
        }
        
        public object Data => RawObject["data"];
        
        protected JObject RawObject { get; set; }

        public RpcError()
        {
            RawObject = new JObject {["code"] = 0, ["message"] = "", ["data"] = null};
        }
        
        public RpcError(JObject jObject)
        {
            RawObject = jObject;
        }

        public RpcError(int code, string message, object data)
        {
            RawObject = new JObject {["code"] = 0, ["message"] = "", ["data"] = null};
            Code = code;
            Message = message;
            SetData(data);
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