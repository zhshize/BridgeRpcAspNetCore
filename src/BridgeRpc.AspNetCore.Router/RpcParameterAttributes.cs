using System;

namespace BridgeRpc.AspNetCore.Router
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcParamAttribute : Attribute
    {
        /// <summary>
        /// Specified parameter name.
        /// </summary>
        public string ParameterName { get; }

        public RpcParamAttribute(string name = null)
        {
            ParameterName = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RpcDataAttribute : Attribute
    {
        /// <summary>
        /// Specified type to deserialize object from data.
        /// </summary>
        public Type TypeRequired { get; }

        public RpcDataAttribute(Type need)
        {
            TypeRequired = need;
        }
        
        public RpcDataAttribute()
        {
            TypeRequired = typeof(byte[]);
        }
    }
}