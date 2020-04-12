using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    /// <summary>
    ///     Represent a RPC Request object.
    ///     This class is an accessing interface of raw JObject.
    /// </summary>
    public class RpcRequest
    {
        /// <summary>
        ///     Create a new RpcResponse object with default protocol version, empty id, empty method, null data.
        /// </summary>
        public RpcRequest()
        {
            RawObject = new JObject {["bridgerpc"] = "1.0", ["id"] = "", ["method"] = "", ["data"] = null};
        }

        /// <summary>
        ///     Create a new RpcRequest from JSON string.
        /// </summary>
        /// <param name="json">JSON string represent a rpc request</param>
        /// <exception cref="RpcException">Parsing error or internal error occurred when parsing JSON string</exception>
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

        /// <summary>
        ///     RPC protocol version of this request object.
        /// </summary>
        public string Version
        {
            get => RawObject["bridgerpc"].ToString();
            set => RawObject["bridgerpc"] = value;
        }

        /// <summary>
        ///     Identification of this request, corresponding response will present this field with same id.
        /// </summary>
        public string Id
        {
            get => RawObject["id"].ToString();
            set => RawObject["id"] = value;
        }

        /// <summary>
        ///     Method will be called.
        /// </summary>
        public string Method
        {
            get => RawObject["method"].ToString();
            set => RawObject["method"] = value;
        }

        /// <summary>
        ///     Data (or parameters) to be sent
        /// </summary>
        public object Data => RawObject["data"];

        /// <summary>
        ///     Raw JSON Object.
        /// </summary>
        protected JObject RawObject { get; set; }

        /// <summary>
        ///     Get parameter in data field by name.
        /// </summary>
        /// <param name="name">Name of the parameter (JSON property name) in data object</param>
        /// <typeparam name="T">The type of the parameter will be deserialized to</typeparam>
        /// <returns>The parameter in type T</returns>
        public T GetParameterFromData<T>(string name)
        {
            return RawObject["data"].Type == JTokenType.Object
                ? RawObject["data"][name].ToObject<T>()
                : RawObject["data"].Value<T>();
        }

        /// <summary>
        ///     Serialize this request to JSON.
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            return RawObject.ToString();
        }

        /// <summary>
        ///     Indicate whether this request is a normal request or a notification
        /// </summary>
        /// <returns></returns>
        public bool IsNotify()
        {
            return Id == null;
        }

        /// <summary>
        ///     Set data to be sent.
        /// </summary>
        /// <param name="obj">Object to be sent</param>
        /// <typeparam name="T">Type of sent data</typeparam>
        public void SetData<T>(T obj)
        {
            RawObject["data"] = Util.Util.ToJToken(obj);
        }

        /// <summary>
        ///     Get data in specified type
        /// </summary>
        /// <typeparam name="T">The type that the data will be deserialized and returned</typeparam>
        /// <returns>The data object</returns>
        public T GetData<T>()
        {
            if (typeof(T) == typeof(JToken))
                return (T) (object) RawObject.Property("data").Value;
            return RawObject["data"].ToObject<T>();
        }

        /// <summary>
        ///     Get data in <see cref="JToken" /> type
        /// </summary>
        /// <returns>The data object</returns>
        public JToken GetData()
        {
            return RawObject["data"];
        }
    }
}