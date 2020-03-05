using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    /// <summary>
    /// Represent a RPC Error object.
    /// This class is an accessing interface of raw JObject.
    /// Normally you don't need to construct this object by yourself, calling SetError() in <see cref="RpcResponse"/>
    /// is good.
    /// </summary>
    public class RpcError
    {
        /// <summary>
        /// An integer to indicate the error
        /// </summary>
        public int Code
        { 
            get => RawObject["code"].ToObject<int>();
            set => RawObject["code"] = value;
        }
        
        /// <summary>
        /// A string to describe the error
        /// </summary>
        public string Message
        { 
            get => RawObject["message"].ToString();
            set => RawObject["message"] = value;
        }
        
        /// <summary>
        /// Data is a custom field for more information will be used
        /// </summary>
        public object Data => RawObject["data"];
        
        /// <summary>
        /// Data (or parameters) to be sent
        /// </summary>
        protected JObject RawObject { get; set; }

        /// <summary>
        /// Create a new RpcError object with code = 0, empty message, null data.
        /// Normally you don't need to construct this object by yourself, calling SetError() in
        /// <see cref="RpcResponse"/> is good.
        /// </summary>
        public RpcError()
        {
            RawObject = new JObject {["code"] = 0, ["message"] = "", ["data"] = null};
        }
        
        /// <summary>
        /// Create a new RpcError object from JObject.
        /// Normally you don't need to construct this object by yourself, calling SetError() in
        /// <see cref="RpcResponse"/> is good.
        /// </summary>
        /// <param name="jObject">The error in <see cref="RpcResponse"/></param>
        public RpcError(JObject jObject)
        {
            RawObject = jObject;
        }

        /// <summary>
        /// Create a new RpcError object from JObject.
        /// Normally you don't need to construct this object by yourself, calling SetError() in
        /// <see cref="RpcResponse"/> is good.
        /// </summary>
        /// <param name="code">An integer to indicate the error</param>
        /// <param name="message">A string to describe the error</param>
        /// <param name="data">Data is a custom field for more information will be used</param>
        public RpcError(int code, string message, object data)
        {
            RawObject = new JObject {["code"] = 0, ["message"] = "", ["data"] = null};
            Code = code;
            Message = message;
            SetData(data);
        }
        
        /// <summary>
        /// Set data to be sent.
        /// </summary>
        /// <param name="obj">Object to be sent</param>
        /// <typeparam name="T">Type of sent data</typeparam>
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

        /// <summary>
        /// Get data in specified type
        /// </summary>
        /// <typeparam name="T">The type that the data will be deserialized and returned</typeparam>
        /// <returns>The data object</returns>
        public T GetData<T>()
        {
            return RawObject["data"].ToObject<T>();
        }
        
        /// <summary>
        /// Get data in <see cref="JToken"/> type
        /// </summary>
        /// <returns>The data object</returns>
        public JToken GetData()
        {
            return RawObject["data"];
        }
    }
}