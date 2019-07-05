using System;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    /// Attribute to decorate a PRC method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcMethodAttribute : Attribute
    {
        /// <summary>
        /// Name of the method to be used in the router.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName">(Optional) Name of the method to be used in the router.</param>
        public RpcMethodAttribute(string methodName)
        {
            this.MethodName = methodName.Trim();
        }
    }
}