using System;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.AspNetCore.Router
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcParamAttribute : Attribute
    {
        /// <summary>
        ///     Specify the parameter name to be retrieved
        /// </summary>
        /// <param name="name">Parameter name in request data</param>
        public RpcParamAttribute(string name = null)
        {
            ParameterName = name;
        }

        /// <summary>
        ///     Specified parameter name.
        /// </summary>
        public string ParameterName { get; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcDataAttribute : Attribute
    {
        /// <summary>
        ///     Specify the type that the data in request will be deserialized
        /// </summary>
        /// <param name="need">Type to be deserialized</param>
        public RpcDataAttribute(Type need)
        {
            TypeRequired = need;
        }

        /// <summary>
        ///     Specify that the data will be passed as a <see cref="JToken" /> object
        /// </summary>
        public RpcDataAttribute()
        {
            TypeRequired = null;
        }

        /// <summary>
        ///     Specified type to deserialize object from data.
        /// </summary>
        public Type TypeRequired { get; }
    }
}