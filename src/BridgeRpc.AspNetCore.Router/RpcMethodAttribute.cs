using System;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    ///     Attribute to decorate a PRC method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcMethodAttribute : Attribute
    {
        /// <summary>
        ///     Override the method name.
        /// </summary>
        /// <param name="methodName">(Optional) Name of the method to be used in the router.</param>
        public RpcMethodAttribute(string methodName)
        {
            MethodName = methodName.Trim();
        }

        /// <summary>
        ///     Name of the method to be used in the router.
        /// </summary>
        public string MethodName { get; }
    }
}