using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    /// <summary>
    /// Represent a RPC Response object.
    /// This class is an accessing interface of raw JObject.
    /// </summary>
    public class RpcResponse
    {
        /// <summary>
        /// RPC protocol version of this response object.
        /// </summary>
        public string Version
        {
            get => RawObject["bridgerpc"].ToString();
            set => RawObject["bridgerpc"] = value;
        }
        
        /// <summary>
        /// Identification of this response equals to the corresponding request.
        /// </summary>
        public string Id
        {
            get => RawObject["id"].ToString();
            set => RawObject["id"] = value;
        }
        
        /// <summary>
        /// Execution result by the request handler.
        /// If you want to set result, use <see cref="SetResult{T}"/>
        /// </summary>
        public object Result => RawObject["result"];
        
        /// <summary>
        /// Presets when error occurred in request handling pipeline.
        /// </summary>
        public RpcError Error { get; private set; }
        
        /// <summary>
        /// Raw JSON Object.
        /// </summary>
        protected JObject RawObject { get; set; }

        /// <summary>
        /// Create a new RpcResponse object with default protocol version, empty id, null result, null error.
        /// </summary>
        public RpcResponse()
        {
            RawObject = new JObject {["bridgerpc"] = "1.0", ["id"] = "", ["result"] = null, ["error"] = null};
        }

        /// <summary>
        /// Create a new RpcResponse from JSON string.
        /// </summary>
        /// <param name="json">JSON string represent a rpc response</param>
        /// <exception cref="RpcException">Parsing error or internal error occurred when parsing JSON string</exception>
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

        /// <summary>
        /// Serialize this response to JSON.
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJson()
        {
            return RawObject.ToString();
        }

        /// <summary>
        /// Set result to be responded.
        /// </summary>
        /// <param name="obj">Object to be responded</param>
        /// <typeparam name="T">Type of responded data</typeparam>
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

        /// <summary>
        /// Get result in specified type
        /// </summary>
        /// <typeparam name="T">The type that the result will be deserialized and returned</typeparam>
        /// <returns>The result object</returns>
        public T GetResult<T>()
        {
            return RawObject["result"].ToObject<T>();
        }
        
        /// <summary>
        /// Get result in <see cref="JToken"/> type
        /// </summary>
        /// <returns>The result object</returns>
        public JToken GetResult()
        {
            return RawObject["result"];
        }

        /// <summary>
        /// Set the error status.
        /// </summary>
        /// <param name="code">An integer to indicate the error</param>
        /// <param name="message">A string to describe the error</param>
        /// <param name="data">Data is a custom field for more information will be used</param>
        /// <typeparam name="T">Type of data field</typeparam>
        public void SetError<T>(int code, string message, T data)
        {
            var err = new JObject {{"code", code}, {"message", message}, {"data", JToken.FromObject(data)}};
            RawObject["error"] = err;
            SyncErrorObject();
        }
        
        /// <summary>
        /// Clear the error status
        /// </summary>
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